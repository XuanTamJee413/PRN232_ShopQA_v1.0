using Microsoft.AspNetCore.Mvc;
using ShopQaMVC.Models;
using System.Diagnostics;

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

        //tamnx Home
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();

            // Lấy danh sách sản phẩm
            var productResponse = await client.GetAsync("https://localhost:7101/api/Product");
            var products = new List<ProductVM>();
            if (productResponse.IsSuccessStatusCode)
                products = await productResponse.Content.ReadFromJsonAsync<List<ProductVM>>() ?? new List<ProductVM>();

            // Lấy danh sách danh mục
            var categoryResponse = await client.GetAsync("https://localhost:7101/api/Category");
            var categories = new List<CategoryVM>();
            if (categoryResponse.IsSuccessStatusCode)
                categories = await categoryResponse.Content.ReadFromJsonAsync<List<CategoryVM>>() ?? new List<CategoryVM>();

            // Gộp vào ViewModel
            var vm = new HomeIndexVM
            {
                Products = products,
                Categories = categories
            };

            return View(vm);
        }

        // Tamnx ViewProduct list customer side
        public async Task<IActionResult> Shop()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var response = await client.GetAsync("https://localhost:7101/api/Product");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Không thể lấy dữ liệu sản phẩm từ API.";
                    return View(new List<ProductDTO>());
                }

                var products = await response.Content.ReadFromJsonAsync<List<ProductDTO>>();
                return View(products ?? new List<ProductDTO>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi gọi API: " + ex.Message;
                return View(new List<ProductDTO>());
            }
        }


        public IActionResult SingleProduct()
        {
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

    }
}
