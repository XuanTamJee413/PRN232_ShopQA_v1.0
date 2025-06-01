namespace ShopQaMVC.Models
{
    public class HomeIndexVM
    {
        public List<ProductVM> Products { get; set; } = new();
        public List<CategoryVM> Categories { get; set; } = new();
    }
}
