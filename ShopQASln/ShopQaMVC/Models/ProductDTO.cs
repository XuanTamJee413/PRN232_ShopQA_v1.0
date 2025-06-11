namespace ShopQaMVC.Models
{


    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string ImageUrl { get; set; }
        public CategoryDTO? Category { get; set; }
        public BrandDTO? Brand { get; set; }
        public List<VariantDTO>? Variants { get; set; }
    }
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class BrandDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class VariantDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
    }
}

