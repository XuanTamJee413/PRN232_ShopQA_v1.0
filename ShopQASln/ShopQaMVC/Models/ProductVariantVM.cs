namespace ShopQaMVC.Models
{
    public class ProductVariantVM
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public int ProductId { get; set; }

        
        public ProductVM? Product { get; set; }

    }
}
