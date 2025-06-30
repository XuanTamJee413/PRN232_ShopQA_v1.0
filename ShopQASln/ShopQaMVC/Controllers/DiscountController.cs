using Microsoft.AspNetCore.Mvc;
using Business.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopQaMVC.Models;
using System.Text.Json;
using System.Web;

namespace ShopQaMVC.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class DiscountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7101/odata/Discount";
        private readonly string _productVariantApiUrl = "https://localhost:7101/odata/ProductVariant";

        public DiscountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }



        public async Task<IActionResult> Index(string searchProduct, string sortBy, int page = 1)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            var pageSize = 5;

            var queryParams = new List<string>
    {
        "$count=true",
        $"$top={pageSize}",
        $"$skip={(page - 1) * pageSize}",
        
        "$expand=ProductVariant($expand=Product)"
    };

            
            if (!string.IsNullOrWhiteSpace(searchProduct))
            {
                var encodedKeyword = HttpUtility.UrlEncode(searchProduct.ToLower().Trim());
               
                queryParams.Add($"$filter=contains(tolower(ProductVariant/Product/Name), '{encodedKeyword}')");
            }

           
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "product_asc":
                        queryParams.Add("$orderby=ProductVariant/Product/Name asc");
                        break;
                    case "product_desc":
                        queryParams.Add("$orderby=ProductVariant/Product/Name desc");
                        break;
                    case "amount_asc":
                        queryParams.Add("$orderby=Amount asc");
                        break;
                    case "amount_desc":
                        queryParams.Add("$orderby=Amount desc");
                        break;
                    default:
                        queryParams.Add("$orderby=EndDate desc"); 
                        break;
                }
            }
            else
            {
                queryParams.Add("$orderby=EndDate desc"); 
            }

            var apiEndpoint = $"{_apiBaseUrl}?{string.Join("&", queryParams)}";
            List<DiscountVM> discounts = new List<DiscountVM>();
            int totalRecords = 0;

            var response = await client.GetAsync(apiEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    JsonElement root = doc.RootElement;
                    if (root.TryGetProperty("@odata.count", out JsonElement countElement))
                    {
                        totalRecords = countElement.GetInt32();
                    }
                    if (root.TryGetProperty("value", out JsonElement valueElement))
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var discountList = JsonSerializer.Deserialize<List<DiscountVM>>(valueElement.GetRawText(), options);
                        if (discountList != null) discounts = discountList;
                    }
                }
            }
            else
            {
                TempData["Error"] = $"Lỗi API: {response.StatusCode}";
            }

           
            ViewBag.CurrentSearchProduct = searchProduct;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling((double)totalRecords / pageSize);

            return View(discounts);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new DiscountCreateVM();
            await PopulateProductVariantsDropdown(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiscountCreateVM model)
        {
            if (model.StartDate >= model.EndDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateProductVariantsDropdown(model);
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("IgnoreSSL");

           
            var discountDto = new DiscountCreateDTO
            {
                Amount = model.Amount,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = model.Status,
                ProductVariantId = model.ProductVariantId
            };

            var response = await client.PostAsJsonAsync(_apiBaseUrl, discountDto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Tạo khuyến mãi thành công.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Lỗi từ API: {errorContent}";
                await PopulateProductVariantsDropdown(model);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            var response = await client.GetAsync($"{_apiBaseUrl}({id})");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var discount = await response.Content.ReadFromJsonAsync<DiscountVM>();
            var model = new DiscountCreateVM
            {
                Id = discount.Id,
                Amount = discount.Amount,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                Status = discount.Status,
                ProductVariantId = discount.ProductVariantId
            };

            await PopulateProductVariantsDropdown(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DiscountCreateVM model)
        {
            if (id != model.Id) return BadRequest();

            if (model.StartDate >= model.EndDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateProductVariantsDropdown(model);
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("IgnoreSSL");

            var discountDto = new DiscountUpdateDTO
            {
                Id = model.Id,
                Amount = model.Amount,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = model.Status,
                ProductVariantId = model.ProductVariantId
            };

            var response = await client.PutAsJsonAsync($"{_apiBaseUrl}({id})", discountDto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Cập nhật khuyến mãi thành công.";
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            TempData["Error"] = $"Lỗi từ API: {errorContent}";
            await PopulateProductVariantsDropdown(model);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            var response = await client.DeleteAsync($"{_apiBaseUrl}({id})");

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Xóa khuyến mãi thành công.";
            }
            else
            {
                TempData["Error"] = $"Không thể xóa khuyến mãi. Lỗi: {response.StatusCode}";
            }
            return RedirectToAction(nameof(Index));
        }


        private async Task PopulateProductVariantsDropdown(DiscountCreateVM model)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            var selectListItems = new List<SelectListItem>();

          
            var productApiUrl = "https://localhost:7101/odata/Product";
            var apiEndpoint = $"{productApiUrl}?$expand=Variants&$orderby=Name";

            var response = await client.GetAsync(apiEndpoint);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        JsonElement root = doc.RootElement;
                        if (root.TryGetProperty("value", out JsonElement valueElement))
                        {
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                           
                            var productList = JsonSerializer.Deserialize<List<ProductVM>>(valueElement.GetRawText(), options);

                            if (productList != null)
                            {
                               
                                selectListItems = productList
                                    .Where(p => p.Variants != null) // Chỉ lấy các sản phẩm có biến thể
                                    .SelectMany(p => p.Variants.Select(v => new SelectListItem
                                    {
                                        Value = v.Id.ToString(),
                                        Text = $"{p.Name} | Màu: {v.Color} | Cỡ: {v.Size}"
                                    }))
                                    .ToList();
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"JSON Deserialization Error: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"API Call Failed: {response.StatusCode}");
            }

           
            model.ProductVariants = selectListItems;
        }
    }
}