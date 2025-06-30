using Business.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using ShopQaMVC.Models;
using System.Diagnostics;
using System.Text.Json;

namespace ShopQaMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        //tamnx Home customer side
        public async Task<IActionResult> Index()
        {
            var products = new List<ProductVM>();
            var categories = new List<CategoryVM>();

            try
            {
                var client = _httpClientFactory.CreateClient();

                var productResponse = await client.GetAsync("https://localhost:7101/api/Home/productlist");
                if (productResponse.IsSuccessStatusCode)
                    products = await productResponse.Content.ReadFromJsonAsync<List<ProductVM>>() ?? new();

                var categoryResponse = await client.GetAsync("https://localhost:7101/api/Category");
                if (categoryResponse.IsSuccessStatusCode)
                    categories = await categoryResponse.Content.ReadFromJsonAsync<List<CategoryVM>>() ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể gọi API ở trang chủ.");
                ViewBag.Error = "Không thể tải dữ liệu từ API.";
            }

            var vm = new HomeIndexVM
            {
                Products = products,
                Categories = categories
            };

            return View(vm);
        }

        // Tamnx ViewProduct list customer side
        public async Task<IActionResult> Shop(int? categoryId, int? brandId)
        {
            var client = _httpClientFactory.CreateClient();

            var filters = new List<string>();
            if (categoryId.HasValue)
                filters.Add($"CategoryId eq {categoryId.Value}");
            if (brandId.HasValue)
                filters.Add($"BrandId eq {brandId.Value}");

            var filterQuery = filters.Any() ? $"&$filter={string.Join(" and ", filters)}" : "";
            var productUrl = $"https://localhost:7101/odata/Product?$expand=Variants,Category,Brand{filterQuery}";

            var products = new List<ProductDTO>();
            var categories = new List<CategoryDTO>();
            var brands = new List<BrandDTO>();

            try
            {
                var response = await client.GetAsync(productUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    var productJson = root.GetProperty("value").GetRawText();
                    var odataProducts = JsonSerializer.Deserialize<List<ProductODataDTO>>(productJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new();

                    products = odataProducts.Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        CategoryId = p.CategoryId,
                        BrandId = p.BrandId,
                        ImageUrl = p.ImageUrl,
                        Category = p.Category,
                        Brand = p.Brand,
                        Variants = p.Variants
                    }).ToList();
                }
                else
                {
                    ViewBag.ProductError = "Không thể tải sản phẩm";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ProductError = "Lỗi sản phẩm: " + ex.Message;
            }

            try
            {
                var response = await client.GetAsync("https://localhost:7101/api/Category");
                if (response.IsSuccessStatusCode)
                    categories = await response.Content.ReadFromJsonAsync<List<CategoryDTO>>() ?? new();
                else
                    ViewBag.CategoryError = "Không thể tải danh mục";
            }
            catch (Exception ex)
            {
                ViewBag.CategoryError = "Lỗi danh mục: " + ex.Message;
            }

            try
            {
                var response = await client.GetAsync("https://localhost:7101/api/Brand");
                if (response.IsSuccessStatusCode)
                    brands = await response.Content.ReadFromJsonAsync<List<BrandDTO>>() ?? new();
                else
                    ViewBag.BrandError = "Không thể tải nhãn hiệu";
            }
            catch (Exception ex)
            {
                ViewBag.BrandError = "Lỗi nhãn hiệu: " + ex.Message;
            }

            ViewBag.Categories = categories;
            ViewBag.Brands = brands;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SelectedBrand = brandId;

            return View(products);
        }

        public async Task<IActionResult> SingleProduct(int id)
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



        public IActionResult Privacy()
        {
            return View();
        }
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult NotFoundPage()
        {
            return View("404NotFound");
        }

    }
}
