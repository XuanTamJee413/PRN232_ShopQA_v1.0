using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShopQaMVC.Models;

namespace ShopQaMVC.Controllers
{
    public class AdminController : Controller
    {
        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Dashboard";
            // Lấy token từ Session, Cookie hoặc hardcode để test
            string token = HttpContext.Session.GetString("AccessToken") ?? "";
            if (string.IsNullOrEmpty(token))
            {
                // Nếu chưa có token, có thể hardcode tạm để test
                token = "YOUR_ADMIN_JWT_TOKEN_HERE"; // Thay bằng token thật
            }

            var totalOrderCount = await GetTotalOrderCountAsync(token);
            var totalRevenue = await GetTotalRevenueAsync(token);
            var productVariantSales = await GetProductVariantSalesAsync(token);

            ViewBag.TotalOrderCount = totalOrderCount;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.ProductVariantSales = productVariantSales;

            return View();
        }

        public async Task<int> GetTotalOrderCountAsync(string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7101/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/Order/total-count");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return int.Parse(result);
            }
        }

        public async Task<decimal> GetTotalRevenueAsync(string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7101/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/Order/total-revenue");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return decimal.Parse(result);
            }
        }

        public async Task<List<ProductVariantSalesDto>> GetProductVariantSalesAsync(string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7101/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/Order/product-variant-sales");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ProductVariantSalesDto>>(result);
            }
        }

    
    }
}
