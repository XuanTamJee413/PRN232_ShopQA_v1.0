using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ShopQaMVC.Models;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Web;

namespace ShopQaMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        // Quan trọng: URL đã trỏ đến endpoint "odata"
        private readonly string _apiBaseUrl = "https://localhost:7101/odata/Category";

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Action Index xây dựng truy vấn OData
        public async Task<IActionResult> Index(string searchKeyword, string sortBy, int page = 1)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            var pageSize = 5;

            // Phần xây dựng queryParams giữ nguyên, không thay đổi
            var queryParams = new List<string>
            {
                "$count=true",
                $"$top={pageSize}",
                $"$skip={(page - 1) * pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                var encodedKeyword = HttpUtility.UrlEncode(searchKeyword.ToLower().Trim());
                queryParams.Add($"$filter=contains(tolower(Name), '{encodedKeyword}')");
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("name_asc", System.StringComparison.OrdinalIgnoreCase))
                {
                    queryParams.Add("$orderby=Name asc");
                }
                else if (sortBy.Equals("name_desc", System.StringComparison.OrdinalIgnoreCase))
                {
                    queryParams.Add("$orderby=Name desc");
                }
            }

            var apiEndpoint = $"{_apiBaseUrl}?{string.Join("&", queryParams)}";

            List<CategoryVM> categories = new List<CategoryVM>();
            int totalRecords = 0;

            var response = await client.GetAsync(apiEndpoint);

            if (response.IsSuccessStatusCode)
            {
                // --- BẮT ĐẦU PHẦN THAY ĐỔI ---

                // 1. Đọc nội dung JSON vào một chuỗi
                var jsonString = await response.Content.ReadAsStringAsync();

                // 2. Dùng JsonDocument để phân tích (parse) chuỗi JSON
                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    // 3. Lấy ra phần tử gốc của tài liệu JSON
                    JsonElement root = doc.RootElement;

                    // 4. Lấy giá trị của @odata.count (nếu có)
                    if (root.TryGetProperty("@odata.count", out JsonElement countElement))
                    {
                        totalRecords = countElement.GetInt32();
                    }

                    // 5. Lấy mảng 'value' và deserialize nó thành List<CategoryVM>
                    if (root.TryGetProperty("value", out JsonElement valueElement))
                    {
                        // Lấy chuỗi JSON của riêng mảng 'value' và deserialize
                        var categoriesList = JsonSerializer.Deserialize<List<CategoryVM>>(valueElement.GetRawText());
                        if (categoriesList != null)
                        {
                            categories = categoriesList;
                        }
                    }
                }
                // --- KẾT THÚC PHẦN THAY ĐỔI ---
            }
            else
            {
                TempData["Error"] = $"Lỗi API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
            }

            ViewBag.CurrentSearchKeyword = searchKeyword;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling((double)totalRecords / pageSize);

            return View(categories);
        }

        // --- CÁC HÀNH ĐỘNG CRUD ---

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryVM category)
        {
            if (!ModelState.IsValid) return View(category);

            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            // URL cho POST là URL của collection
            var response = await client.PostAsJsonAsync(_apiBaseUrl, category);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Tạo danh mục thành công.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Đã có lỗi xảy ra khi tạo danh mục.");
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            // OData dùng cú pháp dấu ngoặc đơn cho key: /odata/Category(1)
            var response = await client.GetAsync($"{_apiBaseUrl}({id})");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không tìm thấy danh mục.";
                return RedirectToAction(nameof(Index));
            }

            var category = await response.Content.ReadFromJsonAsync<CategoryVM>();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryVM category)
        {
            if (id != category.Id) return BadRequest();
            if (!ModelState.IsValid) return View(category);

            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            var response = await client.PutAsJsonAsync($"{_apiBaseUrl}({id})", category);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Cập nhật danh mục thành công.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Đã có lỗi xảy ra khi cập nhật.");
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            var response = await client.DeleteAsync($"{_apiBaseUrl}({id})");

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Xóa danh mục thành công.";
            }
            else
            {
                TempData["Error"] = $"Không thể xóa danh mục. Lỗi: {response.StatusCode}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}