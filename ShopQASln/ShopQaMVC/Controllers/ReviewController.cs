using Microsoft.AspNetCore.Mvc;

namespace ShopQaMVC.Controllers
{
    public class ReviewController : Controller
    {
        public IActionResult ReviewList()
        {
            return View(); // sẽ tìm file Views/Review/ReviewList.cshtml
        }
    }

}
