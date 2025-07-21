using Business.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using ShopQaMVC.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
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
        public IActionResult Shop()
        {
            return View();
        }

        public async Task<IActionResult> SingleProduct(int id)
        {
            ViewBag.ProductId = id;
            return View();
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
        public IActionResult ErrorPage(string error)
        {
            ViewBag.ErrorMessage = error;
            return View("404NotFound");
        }

    }
}
