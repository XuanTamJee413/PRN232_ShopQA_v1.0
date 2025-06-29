using Microsoft.AspNetCore.Mvc;

namespace ShopQaMVC.Controllers
{
    public class ReviewController : Controller
    {
        public IActionResult ReviewList()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Token = token;
            return View(); // sẽ tìm file Views/Review/ReviewList.cshtml
        }
    }

}
