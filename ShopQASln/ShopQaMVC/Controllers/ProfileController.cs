using Business.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShopQaMVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7101/api/profile";

        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Hiển thị profile
        public async Task<IActionResult> Profile()
        {
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Login", "Account");
            }
            var userDto = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDTO>(userJson);
            var userId = userDto?.Id ?? 0;
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/{userId}");
            if (!response.IsSuccessStatusCode) return View(null);
            var json = await response.Content.ReadAsStringAsync();
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDTO>(json);
            return View(user);
        }

        // GET: Edit profile
        public async Task<IActionResult> Edit()
        {
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Login", "Account");
            }
            var userDto = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDTO>(userJson);
            var userId = userDto?.Id ?? 0;
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/{userId}");
            if (!response.IsSuccessStatusCode) return View(null);
            var json = await response.Content.ReadAsStringAsync();
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDTO>(json);
            return View(user);
        }

        // POST: Edit profile
         [HttpPost]
        public async Task<IActionResult> Edit(UserDTO userDto)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(userDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiBaseUrl}/{userDto.Id}", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["EditProfileSuccess"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Profile");
            }
            TempData["EditProfileError"] = "Cập nhật thông tin thất bại!";
            return RedirectToAction("Profile");
        }

        // GET: Change password
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Change password
        [HttpPost]
        public async Task<IActionResult> ChangePassword(int id, string oldPassword, string newPassword)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(new { oldPassword, newPassword });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiBaseUrl}/{id}/change-password", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["ChangePasswordSuccess"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("Profile");
            }
            TempData["ChangePasswordError"] = "Đổi mật khẩu thất bại. Mật khẩu cũ không đúng hoặc có lỗi.";
            return RedirectToAction("Profile");
        }
    }
}
