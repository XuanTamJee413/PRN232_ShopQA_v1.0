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
    }
}
