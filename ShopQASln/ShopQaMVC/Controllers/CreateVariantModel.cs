using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace ShopQaMVC.Controllers
{
    public class CreateVariantModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int ProductId { get; set; }

        public void OnGet(int productId)
        {
            ProductId = productId;
        }
    }
}
