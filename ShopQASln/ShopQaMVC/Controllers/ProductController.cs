using Microsoft.AspNetCore.Mvc;

namespace ShopQaMVC.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult ProductList()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Token = token;
            return View();
        }

        public IActionResult Create()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Token = token;
            return View();
        }
        public IActionResult Update(int id)
        {
            return View();
        }
        public IActionResult Detail(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Token = token;
            return View();
        }
    }
}
