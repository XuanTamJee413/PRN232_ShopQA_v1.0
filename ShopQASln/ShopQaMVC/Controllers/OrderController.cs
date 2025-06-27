using Microsoft.AspNetCore.Mvc;

namespace ShopQaMVC.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult OrderList()
        {
            return View(); // trả về view Razor là OrderList.cshtml
        }
    }
}
