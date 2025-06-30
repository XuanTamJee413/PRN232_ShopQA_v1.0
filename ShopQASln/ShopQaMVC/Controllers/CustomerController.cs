using Business.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShopQaMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CustomerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Cart()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var userJson = HttpContext.Session.GetString("User");
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonSerializer.Deserialize<UserDTO>(userJson);
                Console.WriteLine("User: " + user.Username);
                Console.WriteLine("Token: " + token);
            }
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Login", "Account");
            }

            var userDto = JsonSerializer.Deserialize<UserDTO>(userJson);
            var userId = userDto?.Id ?? 0;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"https://localhost:7101/api/cart/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<CartDTO>());
            }

            var cartItems = await response.Content.ReadFromJsonAsync<List<CartDTO>>();
            return View(cartItems);
        }
        public IActionResult Checkout()
        {
            return View();
        }
        public async Task<IActionResult> FeedBack(int id)
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
    }
}
