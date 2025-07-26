using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }
        public string? Status { get; set; } = "Pending";
        public User User { get; set; } = default!;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

}
