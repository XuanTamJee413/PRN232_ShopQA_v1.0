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
                return Content("Bạn cần đăng nhập để sử dụng chức năng này.");
            var userDto = JsonConvert.DeserializeObject<UserDTO>(userJson);
            var userId = userDto?.Id ?? 0;
            var client = _httpClientFactory.CreateClient();
            // Kiểm tra sản phẩm đã tồn tại trong wishlist chưa
            var checkRes = await client.GetAsync($"{_apiBaseUrl}/list?userId={userId}");
            if (checkRes.IsSuccessStatusCode)
            {
                var json = await checkRes.Content.ReadAsStringAsync();
                var wishlist = JsonConvert.DeserializeObject<WishlistDTO>(json);
                if (wishlist?.Items != null && wishlist.Items.Any(i => i.ProductId == productId))
                {
                    return Content("Sản phẩm đã tồn tại trong danh sách yêu thích");
                }
            }
            // Nếu chưa có thì thêm mới
            var response = await client.PostAsync($"{_apiBaseUrl}/add?userId={userId}&productId={productId}", null);
            if (response.IsSuccessStatusCode)
                return Content("Đã thêm vào danh sách yêu thích!");
            else
                return Content("Lỗi khi thêm vào danh sách yêu thích");
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
