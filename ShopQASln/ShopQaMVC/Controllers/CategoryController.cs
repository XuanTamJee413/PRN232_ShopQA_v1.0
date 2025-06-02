using Microsoft.AspNetCore.Mvc;
using ShopQaMVC.Models;
using System.Security.Claims;
namespace ShopQaMVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");

            var response = await client.GetAsync("https://localhost:7101/api/Category");

            if (!response.IsSuccessStatusCode)
                return View(new List<CategoryVM>());

            var categories = await response.Content.ReadFromJsonAsync<List<CategoryVM>>();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CategoryVM category)
        {
            if (!ModelState.IsValid) return View(category);

            var client = _httpClientFactory.CreateClient("IgnoreSSL");

            var response = await client.PostAsJsonAsync("https://localhost:7101/api/Category", category);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Lỗi khi tạo danh mục");
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");

            var response = await client.GetAsync($"https://localhost:7101/api/Category/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var category = await response.Content.ReadFromJsonAsync<CategoryVM>();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryVM category)
        {
            if (!ModelState.IsValid) return View(category);

            var client = _httpClientFactory.CreateClient("IgnoreSSL");

            var response = await client.PutAsJsonAsync($"https://localhost:7101/api/Category/{category.Id}", category);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Lỗi khi cập nhật danh mục");
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");

            var response = await client.DeleteAsync($"https://localhost:7101/api/Category/{id}");

            return RedirectToAction("Index");
        }
    }
}
