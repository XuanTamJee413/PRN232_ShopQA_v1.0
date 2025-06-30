namespace ShopQaMVC.Models
{
    public class AddToCartVM
    {
        public int UserId { get; set; }
        public int? SelectedCartId { get; set; } // null nếu tạo mới
        public bool CreateNewCart { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }

    }

}
