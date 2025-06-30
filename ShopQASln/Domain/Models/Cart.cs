using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        [JsonIgnore]
        public virtual Cart? Cart { get; set; }
        public int ProductVariantId { get; set; }
        public virtual ProductVariant? ProductVariant { get; set; }
        public int Quantity { get; set; }
    }
}
