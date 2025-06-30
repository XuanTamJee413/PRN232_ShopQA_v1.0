namespace ShopQaMVC.Models
{
    public class CartVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CartItemVM>? Items { get; set; }
    }

    public class CartItemVM
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public PVariantVM? ProductVariant { get; set; }
    }

    public class PVariantVM
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
        public ProductNameVM? Product { get; set; }
    }
    public class ProductNameVM
    {
        public string Name { get; set; } = "";
    }
}
