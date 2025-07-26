using Microsoft.AspNetCore.Mvc;

namespace ShopQaMVC.Controllers
{
    public class OrderController : Controller
    {
        // thêm môtt phương thức khác: Index để trả về view Razor là Customer/Order.cshtml
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Token = token;
            return View("~/Views/Customer/Order.cshtml");
        }

        public IActionResult OrderList()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Token = token;
            return View(); // trả về view Razor là OrderList.cshtml
        }
    }
}
