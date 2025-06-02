using Microsoft.AspNetCore.Mvc;

namespace ShopQaMVC.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult ProductList()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
    }
}
