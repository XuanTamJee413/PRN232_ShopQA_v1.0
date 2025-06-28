using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShopQaMVC.Pages.Product
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
