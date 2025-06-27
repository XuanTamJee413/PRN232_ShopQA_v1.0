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

        
        public async Task<IActionResult> Index(string searchQuery, string sortBy)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            List<BrandVM> brands = new List<BrandVM>();
            HttpResponseMessage response;

           
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
              
                response = await client.GetAsync($"{_apiBaseUrl}/search?name={Uri.EscapeDataString(searchQuery)}");
            }
            else if (!string.IsNullOrWhiteSpace(sortBy))
            {
                
                bool sendDesc = false; 
                if (sortBy.EndsWith("Desc")) 
                {
                    sendDesc = true;
                }
               
                response = await client.GetAsync($"{_apiBaseUrl}/sort?desc={sendDesc}");
            }
            else
            {
              
                response = await client.GetAsync(_apiBaseUrl);
            }

            if (response.IsSuccessStatusCode)
            {
                brands = await response.Content.ReadFromJsonAsync<List<BrandVM>>() ?? new List<BrandVM>();
            }
            else
            {
                TempData["Error"] = $"Failed to retrieve brands from API. Status: {response.StatusCode}. Check Brand API's /search or /sort endpoint and server logs: {await response.Content.ReadAsStringAsync()}";
               
            }

           
            ViewBag.CurrentSearchQuery = searchQuery;

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
