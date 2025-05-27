using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
