using Domain.Models;

namespace ShopQaMVC.Models
{
    public class DiscountVM
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
        public int ProductVariantId { get; set; }

       
        public ProductVariantVM? ProductVariant { get; set; }
    }
}
