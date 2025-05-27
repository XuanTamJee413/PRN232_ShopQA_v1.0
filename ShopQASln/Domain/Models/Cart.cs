using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public DateTime CreatedAt { get; set; }
    }
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public virtual Cart? Cart { get; set; }
        public Guid ProductVariantId { get; set; }
        public virtual ProductVariant? ProductVariant { get; set; }
        public int Quantity { get; set; }
    }
}
