using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ShopQaMVC.Models;
using System.Security.Claims;
using System.Text.Json;
using Business.DTO;

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
            var response = await client.PostAsJsonAsync("https://localhost:7101/api/Auth/login", new
            {
                UsernameOrEmail = model.UsernameOrEmail,
                Password = model.Password
            });

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                string errorMessage = "Đăng nhập thất bại";

                try
                {
                    using var doc = JsonDocument.Parse(errorContent);
                    if (doc.RootElement.TryGetProperty("message", out var msgProp))
                    {
                        errorMessage = msgProp.GetString() ?? errorMessage;
                    }
                }
                catch { }

                ModelState.AddModelError("", errorMessage);
                return View(model);
            }

            var userDto = await response.Content.ReadFromJsonAsync<UserDTO>();
            Console.WriteLine(">>> userDto nhan duoc:");
            Console.WriteLine($"Username: {userDto.Username}");
            Console.WriteLine($"Email: {userDto.Email}");
            Console.WriteLine($"Role: {userDto.Role}");
            Console.WriteLine($"Token: {userDto.Token}");
            if (userDto == null)
            {
                ModelState.AddModelError("", "Dữ liệu trả về không hợp lệ");
                return View(model);
            }

            // Lưu user + token vào Session
            HttpContext.Session.SetString("User", JsonSerializer.Serialize(userDto));
            HttpContext.Session.SetString("JwtToken", userDto.Token ?? "");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userDto.Username),
                new Claim(ClaimTypes.Email, userDto.Email),
                new Claim(ClaimTypes.Role, userDto.Role),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProps = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProps);

            Console.WriteLine(">>> Dang nhap thanh cong:");
            Console.WriteLine("Username: " + userDto.Username);
            Console.WriteLine("Role: " + userDto.Role);
            Console.WriteLine("IsAuthenticated sau SignIn: " + (HttpContext.User.Identity?.IsAuthenticated ?? false));
            Console.WriteLine("User.Identity.Name: " + HttpContext.User.Identity?.Name);
            ViewBag.UserJson = JsonSerializer.Serialize(userDto);
            ViewBag.Token = userDto.Token;
            ViewBag.Role = userDto.Role;
            ViewBag.LoginSuccess = true;
            return View("Login");

            //return userDto.Role switch
            //{
            //    "Customer" => RedirectToAction("Index", "Home"),
            //    "Staff" => RedirectToAction("ProductList", "Product"),
            //    "Admin" => RedirectToAction("Dashboard", "Admin"),
            //    _ => RedirectToAction("NotFoundPage", "Home")
            //};
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
