using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ShopQaMVC.Models;
using System.Globalization;
using System.Net;
using System.Text.Json; 

namespace ShopQaMVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7101/api/Category";
        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(string searchKeyword, string sortBy, int page = 1)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            List<CategoryVM> categories = new();

           
            var queryBuilder = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryBuilder["page"] = page.ToString();
            queryBuilder["pageSize"] = "5"; 

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                queryBuilder["keyword"] = searchKeyword;
            }

           
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("name_asc", StringComparison.OrdinalIgnoreCase))
                {
                    queryBuilder["sortAsc"] = "true";
                }
                else if (sortBy.Equals("name_desc", StringComparison.OrdinalIgnoreCase))
                {
                    queryBuilder["sortAsc"] = "false";
                }
            }

            var apiEndpoint = $"{_apiBaseUrl}/paged?{queryBuilder.ToString()}";

            var response = await client.GetAsync(apiEndpoint);

            if (response.IsSuccessStatusCode)
            {
                categories = await response.Content.ReadFromJsonAsync<List<CategoryVM>>() ?? new();
            }
            else
            {
                TempData["Error"] = $"Lỗi khi gọi API: {response.StatusCode}. Server trả: {await response.Content.ReadAsStringAsync()}";
            }

            ViewBag.CurrentSearchKeyword = searchKeyword;
            ViewBag.CurrentSort = sortBy; 
            ViewBag.CurrentPage = page;
            ViewBag.HasMoreCategories = categories.Count == 5; 

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
            {
                TempData["Message"] = "Tạo danh mục thành công.";
                return RedirectToAction("Index");
            }

           
            if (response.StatusCode == HttpStatusCode.Conflict) 
            {
                
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                  
                    using var doc = JsonDocument.Parse(errorContent);
                    var errorMessage = doc.RootElement.GetProperty("message").GetString();
                    ModelState.AddModelError("Name", errorMessage ?? "Tên danh mục này đã tồn tại.");
                }
                catch
                {
                    
                    ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Đã có lỗi xảy ra khi tạo danh mục.");
            }

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
            {
                TempData["Message"] = "Cập nhật danh mục thành công.";
                return RedirectToAction("Index");
            }

           
            if (response.StatusCode == HttpStatusCode.Conflict) 
            {
                
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    
                    using var doc = JsonDocument.Parse(errorContent);
                    var errorMessage = doc.RootElement.GetProperty("message").GetString();
                    ModelState.AddModelError("Name", errorMessage ?? "Tên danh mục này đã tồn tại.");
                }
                catch
                {
                    
                    ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Đã có lỗi xảy ra khi cập nhật danh mục.");
            }

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            var response = await client.DeleteAsync($"https://localhost:7101/api/Category/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Xóa danh mục thành công.";
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["Error"] = string.IsNullOrWhiteSpace(errorMessage)
                    ? "Không thể xóa danh mục."
                    : errorMessage;
            }
            return RedirectToAction("Index");
        }
    }
}
