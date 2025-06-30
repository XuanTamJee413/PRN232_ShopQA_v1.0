using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
        public DateTime CreatedAt { get; set; }
    }
    public class WishlistItem
    {
        public int Id { get; set; }
        public int WishlistId { get; set; }
        [JsonIgnore]
        public virtual Wishlist? Wishlist { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
