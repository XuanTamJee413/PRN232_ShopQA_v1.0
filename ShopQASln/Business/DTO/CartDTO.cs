using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class CartDTO
    {
        public int Id { get; set; } 
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();

        public DateTime CreatedAt { get; set; }
        public string? Status { get; set; }
    }
}
