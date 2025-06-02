using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ShopQaMVC.Models;
using System.Text.RegularExpressions;

namespace ShopQaMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly string _apiBaseUrl;

        public UserController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _apiBaseUrl = _config["ApiSettings:BaseUrl"] ?? "https://localhost:7101/api/User";
        }

        // GET: User
        public async Task<IActionResult> UserList()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}");

            List<UserVM> users = new();
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                users = JsonSerializer.Deserialize<List<UserVM>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return View(users);
        }

        // POST: User/Create (From modal)
        [HttpPost]
        public async Task<IActionResult> Create(UserVM user)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "ME003: This field is required.";
                return RedirectToAction(nameof(UserList));
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var jsonData = JsonSerializer.Serialize(user);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_apiBaseUrl}", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "User created successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to create user.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(UserList));
        }

        // POST: User/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserVM user)
        {
          

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "ME003: This field is required.";
                return RedirectToAction(nameof(UserList));
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var jsonData = JsonSerializer.Serialize(user);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"{_apiBaseUrl}/{user.Id}", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "User updated successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to update user.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(UserList));
        }


        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "User deleted successfully.";
                return RedirectToAction(nameof(UserList));
            }

            TempData["Error"] = "Failed to delete user.";
            return RedirectToAction(nameof(UserList));
        }
    }
}
