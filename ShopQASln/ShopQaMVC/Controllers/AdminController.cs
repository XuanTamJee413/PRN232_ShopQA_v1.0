using Microsoft.AspNetCore.Mvc;

namespace ShopQaMVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard"; 
            return View();
        }
    }
}
