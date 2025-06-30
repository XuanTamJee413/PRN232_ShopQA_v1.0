using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShopQaMVC.Models
{
    public class DiscountCreateVM
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Mức giảm giá (%)")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(7);

        [Display(Name = "Trạng thái (Kích hoạt)")]
        public bool Status { get; set; }

        [Required]
        [Display(Name = "Áp dụng cho sản phẩm")]
        public int ProductVariantId { get; set; }

        
        public IEnumerable<SelectListItem>? ProductVariants { get; set; }
    }
}
