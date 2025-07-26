using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopQaMVC.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers; 
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace ShopQaMVC.Controllers
{
    // [Authorize(Roles = "Admin")] 
    public class BrandController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
       
        private readonly string _apiBaseUrl = "https://localhost:7101/odata/Brand";

        public BrandController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        
        public async Task<IActionResult> Index(string searchKeyword, string sortBy, int page = 1)
        {
          

            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var pageSize = 5;

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

            List<BrandVM> brands = new List<BrandVM>();
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
                        var brandsList = JsonSerializer.Deserialize<List<BrandVM>>(valueElement.GetRawText());
                        if (brandsList != null)
                        {
                            brands = brandsList;
                        }
                    }
                }
            }
            else
            {
                TempData["Error"] = $"Lỗi API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
            }

            ViewBag.CurrentSearchKeyword = searchKeyword;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling((double)totalRecords / pageSize);

            return View(brands);
        }

      

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandVM brand)
        {
            if (!ModelState.IsValid) return View(brand);

            var client = _httpClientFactory.CreateClient("IgnoreSSL");
           
            // var token = HttpContext.Session.GetString("JWToken");
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync(_apiBaseUrl, brand);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Tạo thương hiệu thành công.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Đã có lỗi xảy ra khi tạo thương hiệu.");
            return View(brand);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
         
            // var token = HttpContext.Session.GetString("JWToken");
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{_apiBaseUrl}({id})");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không tìm thấy thương hiệu.";
                return RedirectToAction(nameof(Index));
            }

            var brand = await response.Content.ReadFromJsonAsync<BrandVM>();
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BrandVM brand)
        {
            if (id != brand.Id) return BadRequest();
            if (!ModelState.IsValid) return View(brand);

            var client = _httpClientFactory.CreateClient("IgnoreSSL");
           
            // var token = HttpContext.Session.GetString("JWToken");
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsJsonAsync($"{_apiBaseUrl}({id})", brand);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Cập nhật thương hiệu thành công.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Đã có lỗi xảy ra khi cập nhật thương hiệu.");
            return View(brand);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("IgnoreSSL");
            
            // var token = HttpContext.Session.GetString("JWToken");
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"{_apiBaseUrl}({id})");

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Xóa thương hiệu thành công.";
            }
            else
            {
                TempData["Error"] = $"Không thể xóa do có sản phẩm thuộc thương hiệu này";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}