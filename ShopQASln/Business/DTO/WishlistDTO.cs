using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class WishlistDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<WishlistItemDTO> Items { get; set; } = new List<WishlistItemDTO>();
    }

    public class WishlistItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime AddedAt { get; set; }
        // Thông tin chi tiết sản phẩm
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
        public string? ProductDescription { get; set; }
    }
}
