using Microsoft.AspNetCore.Mvc;
using ShopQaMVC.Models;
using System.Net;
using System.Text.Json;
using System.Net.Http.Json; 

namespace ShopQaMVC.Controllers
{
    public class BrandController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7101/api/Brand"; 

        public BrandController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(string searchQuery, string sortBy, int page = 1)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            List<BrandVM> brands = new();
            HttpResponseMessage response;

           
            var query = $"?page={page}";

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query += $"&search={Uri.EscapeDataString(searchQuery)}";
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var desc = sortBy.EndsWith("Desc") ? "desc" : "asc";
                query += $"&sort={desc}";
            }

            response = await client.GetAsync($"{_apiBaseUrl}/paged{query}");

            if (response.IsSuccessStatusCode)
            {
                brands = await response.Content.ReadFromJsonAsync<List<BrandVM>>() ?? new();
            }
            else
            {
                TempData["Error"] = $"Lỗi khi gọi API: {response.StatusCode}. Server trả: {await response.Content.ReadAsStringAsync()}";
            }

            ViewBag.CurrentSearchQuery = searchQuery;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentPage = page;

            return View(brands);
        }


        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(BrandVM brand)
        {
            if (!ModelState.IsValid) return View(brand);

            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            var response = await client.PostAsJsonAsync(_apiBaseUrl, brand);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Tạo thương hiệu thành công.";
                return RedirectToAction("Index");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    using var doc = JsonDocument.Parse(errorContent);
                    var errorMessage = doc.RootElement.GetProperty("message").GetString();
                    ModelState.AddModelError("Name", errorMessage ?? "Tên thương hiệu đã tồn tại.");
                }
                catch
                {
                    ModelState.AddModelError("Name", "Tên thương hiệu đã tồn tại.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Đã có lỗi xảy ra khi tạo thương hiệu.");
            }

            return View(brand);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            var response = await client.GetAsync($"{_apiBaseUrl}/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var brand = await response.Content.ReadFromJsonAsync<BrandVM>();
            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BrandVM brand)
        {
            if (!ModelState.IsValid) return View(brand);

            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            var response = await client.PutAsJsonAsync($"{_apiBaseUrl}/{brand.Id}", brand);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Cập nhật thương hiệu thành công.";
                return RedirectToAction("Index");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    using var doc = JsonDocument.Parse(errorContent);
                    var errorMessage = doc.RootElement.GetProperty("message").GetString();
                    ModelState.AddModelError("Name", errorMessage ?? "Tên thương hiệu đã tồn tại.");
                }
                catch
                {
                    ModelState.AddModelError("Name", "Tên thương hiệu đã tồn tại.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Đã có lỗi xảy ra khi cập nhật thương hiệu.");
            }

            return View(brand);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            var response = await client.DeleteAsync($"{_apiBaseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Xóa thương hiệu thành công.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                try
                {
                    using var doc = JsonDocument.Parse(errorContent);
                    var errorMessage = doc.RootElement.GetProperty("message").GetString();
                    TempData["Error"] = errorMessage ?? "Không thể xóa thương hiệu.";
                }
                catch
                {
                    TempData["Error"] = "Không thể xóa thương hiệu.";
                }
            }

            return RedirectToAction("Index");
        }
    }
}
