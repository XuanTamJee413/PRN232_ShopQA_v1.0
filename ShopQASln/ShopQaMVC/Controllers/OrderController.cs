using Microsoft.AspNetCore.Mvc;

namespace ShopQaMVC.Controllers
{
    public class OrderController : Controller
    {
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
