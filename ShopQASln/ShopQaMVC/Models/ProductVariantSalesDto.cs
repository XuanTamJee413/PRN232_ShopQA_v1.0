namespace ShopQaMVC.Models
{
    public class ProductVariantSalesDto
    {
        public int ProductVariantId { get; set; }
        public string? ProductName { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int QuantitySold { get; set; }
        public decimal Price { get; set; }
    }
}
