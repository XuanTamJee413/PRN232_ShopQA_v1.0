using Business.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using ShopQaMVC.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShopQaMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CustomerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Cart()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var userJson = HttpContext.Session.GetString("User");

            UserDTO? user = null;
            if (!string.IsNullOrEmpty(userJson))
            {
                try { user = JsonSerializer.Deserialize<UserDTO>(userJson); }
                catch { user = null; }
            }

            var client = _httpClientFactory.CreateClient();
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var filter = user != null ? $"?$filter=UserId eq {user.Id}&" : "?";
            var url = $"https://localhost:7101/odata/Cart{filter}$expand=Items($expand=ProductVariant($expand=Product))";

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(url);
            }
            catch (HttpRequestException ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { error = $"Không kết nối được tới server: {ex.Message}" });
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return RedirectToAction("ErrorPage", "Home", new
                {
                    error = $"Lỗi khi gọi API: {(int)response.StatusCode} {response.ReasonPhrase} - {errorMessage}"
                });
            }

            try
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var carts = JsonSerializer.Deserialize<List<CartVM>>(doc.RootElement.GetProperty("value").GetRawText());
                return View(carts);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { error = $"Lỗi xử lý dữ liệu từ server: {ex.Message}" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartVM model)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 1. Nếu tạo giỏ mới
            if (model.CreateNewCart)
            {
                var newCart = new { UserId = model.UserId, CreatedAt = DateTime.Now };
                var cartRes = await client.PostAsJsonAsync("https://localhost:7101/odata/Cart", newCart);
                if (!cartRes.IsSuccessStatusCode) return BadRequest("Không thể tạo giỏ mới");

                var cartJson = await cartRes.Content.ReadAsStringAsync();
                var cartDoc = JsonDocument.Parse(cartJson);
                model.SelectedCartId = cartDoc.RootElement.GetProperty("Id").GetInt32();
            }

            // 2. check cart xem đã có chưa
            var cartRes2 = await client.GetAsync($"https://localhost:7101/odata/Cart({model.SelectedCartId})?$expand=Items");
            if (!cartRes2.IsSuccessStatusCode) return BadRequest("Không thể lấy giỏ hàng");

            var cartJson2 = await cartRes2.Content.ReadAsStringAsync();
            var cartDoc2 = JsonDocument.Parse(cartJson2);
            var items = cartDoc2.RootElement.GetProperty("Items");

            int currentQty = 0;
            int? existingItemId = null;

            foreach (var item in items.EnumerateArray())
            {
                if (item.GetProperty("ProductVariantId").GetInt32() == model.ProductVariantId)
                {
                    existingItemId = item.GetProperty("Id").GetInt32();
                    currentQty = item.GetProperty("Quantity").GetInt32();
                    break;
                }
            }

            // 3. check stock
            var productRes = await client.GetAsync($"https://localhost:7101/odata/Product({model.ProductId})?$expand=Variants");
            if (!productRes.IsSuccessStatusCode) return BadRequest("Không thể lấy thông tin sản phẩm");

            var productJson = await productRes.Content.ReadAsStringAsync();
            var productDoc = JsonDocument.Parse(productJson);

            var variants = productDoc.RootElement.GetProperty("Variants").EnumerateArray();
            var matchedVariant = variants.FirstOrDefault(v => v.GetProperty("Id").GetInt32() == model.ProductVariantId);
            if (matchedVariant.ValueKind == JsonValueKind.Undefined)
                return BadRequest("Không tìm thấy biến thể sản phẩm");

            int stock = matchedVariant.GetProperty("Stock").GetInt32();

            // 4. Kiểm tra tồn kho
            int totalQty = currentQty + model.Quantity;
            if (totalQty > stock)
                return BadRequest($"Số lượng vượt quá tồn kho. Tồn kho còn lại: {stock - currentQty}");

            // 5. cập nhật
            if (existingItemId.HasValue)
            {
                var patchBody = new
                {
                    Quantity = totalQty
                };

                var patchReq = new HttpRequestMessage(HttpMethod.Patch, $"https://localhost:7101/odata/CartItems({existingItemId.Value})")
                {
                    Content = JsonContent.Create(patchBody) // application/json
                };

                patchReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var patchRes = await client.SendAsync(patchReq);
                if (patchRes.IsSuccessStatusCode)
                    return Ok("Đã cập nhật số lượng sản phẩm trong giỏ hàng");
                else
                    return BadRequest("Lỗi khi cập nhật số lượng");
            }

            else
            {
                // 6. Thêm mới
                var newItem = new
                {
                    CartId = model.SelectedCartId,
                    ProductVariantId = model.ProductVariantId,
                    Quantity = model.Quantity
                };

                var postRes = await client.PostAsJsonAsync("https://localhost:7101/odata/CartItems", newItem);
                if (postRes.IsSuccessStatusCode)
                    return Ok("Đã thêm sản phẩm mới vào giỏ hàng");

                return BadRequest("Không thể thêm sản phẩm");
            }
        }



        public IActionResult Checkout()
        {
            return View();
        }
        public async Task<IActionResult> FeedBack(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JwtToken");
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Login", "Account");
            }

            var userDto = JsonSerializer.Deserialize<UserDTO>(userJson);
            var userId = userDto?.Id ?? 0;
            ViewBag.UserId = userId;
            ViewBag.Token = token;
            try
            {
                var url = $"https://localhost:7101/odata/Product({id})?$expand=Variants,Category,Brand";

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var product = JsonSerializer.Deserialize<ProductODataDTO>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (product != null)
                        return View(product);

                    ViewBag.Error = "Không thể đọc dữ liệu sản phẩm.";
                    return View(new ProductODataDTO()); // tránh null
                }
                else
                {
                    ViewBag.Error = "Không tìm thấy sản phẩm.";
                    return View(new ProductODataDTO()); // tránh null
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                return View(new ProductODataDTO()); // tránh null
            }
        }
    }
}
