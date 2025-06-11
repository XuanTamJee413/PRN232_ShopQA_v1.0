using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Discount
    {
        public int Id { get; set; }
        public decimal Amount { get; set; } // tamnx amount chỉ dùng %
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; } // tamnx bật tắt

        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = default!;
    }
}
