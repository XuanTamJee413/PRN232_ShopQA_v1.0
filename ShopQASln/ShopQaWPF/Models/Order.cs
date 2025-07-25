using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopQaWPF.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = default!;

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
