using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopQaWPF.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public virtual ProductVariant? ProductVariant { get; set; }
        public int Quantity { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
