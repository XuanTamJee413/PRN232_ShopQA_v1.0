using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Business.DTO;

namespace ShopQaMVC.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7101/api/wishlist";
        public WishlistController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Wishlist()
        {
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
                return RedirectToAction("Login", "Account");
            var userDto = JsonConvert.DeserializeObject<UserDTO>(userJson);
            var userId = userDto?.Id ?? 0;
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/list?userId={userId}");
            if (!response.IsSuccessStatusCode) return View(new WishlistDTO());
            var json = await response.Content.ReadAsStringAsync();
            var wishlist = JsonConvert.DeserializeObject<WishlistDTO>(json);
            return View(wishlist);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
                return RedirectToAction("Login", "Account");
            var userDto = JsonConvert.DeserializeObject<UserDTO>(userJson);
            var userId = userDto?.Id ?? 0;
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync($"{_apiBaseUrl}/add?userId={userId}&productId={productId}", null);
            // Có thể xử lý thông báo ở đây
            return RedirectToAction("Wishlist");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
                return RedirectToAction("Login", "Account");
            var userDto = JsonConvert.DeserializeObject<UserDTO>(userJson);
            var userId = userDto?.Id ?? 0;
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_apiBaseUrl}/remove?userId={userId}&productId={productId}");
            var response = await client.SendAsync(request);
            // Có thể xử lý thông báo ở đây
            return RedirectToAction("Wishlist");
        }
    }
}
