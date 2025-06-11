using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ShopQaMVC.Models;
using System.Security.Claims;
using System.Text.Json;

namespace ShopQaMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7101/api/Auth/login", model);

            if (!response.IsSuccessStatusCode)
            {
                // Đọc nội dung lỗi từ API
                var errorContent = await response.Content.ReadAsStringAsync();
                string errorMessage = "Sai tài khoản hoặc mật khẩu";

                try
                {
                    // Giả sử API trả về { "message": "..." }
                    using var doc = JsonDocument.Parse(errorContent);
                    if (doc.RootElement.TryGetProperty("message", out var msgProp))
                    {
                        errorMessage = msgProp.GetString() ?? errorMessage;
                    }
                }
                catch
                {
                    // Nếu không phải JSON hợp lệ, giữ nguyên thông báo mặc định
                }

                ModelState.AddModelError("", errorMessage);
                return View(model);
            }

            var userInfo = await response.Content.ReadFromJsonAsync<UserVM>();
            HttpContext.Session.SetString("User", JsonSerializer.Serialize(userInfo));

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, (string)userInfo.Username),
        new Claim(ClaimTypes.Email, (string)userInfo.Email),
        new Claim(ClaimTypes.Role, (string)userInfo.Role)
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            if (userInfo.Role == "Admin")
                return RedirectToAction("ProductList", "Product");
            else
                return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7101/api/auth/register", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Đăng ký thất bại");
                return View(model);
            }

            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded || result.Principal == null)
                return RedirectToAction("Login");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value ?? "";
            var name = result.Principal.Identity?.Name ?? "";

            HttpContext.Session.SetString("User", $"{name} - {email}");

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }

    }

}
