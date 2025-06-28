using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class PaymentRequestModel
    {
        [Required(ErrorMessage = "Số tiền là bắt buộc.")]
        [Range(1, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Thông tin đơn hàng là bắt buộc.")]
        public string OrderInfo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã đơn hàng là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Mã đơn hàng phải lớn hơn 0.")]
        public int OrderId { get; set; }
    }
}
