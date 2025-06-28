using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProductId { get; set; }
        public string Username { get; set; } = default!;
        public string ProductName { get; set; } = default!;
    }

    public class ReviewCreateDto
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
    }
    public class ReviewUpdateDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
    public class ReviewSingleDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProductId { get; set; }
        public string Username { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public int UserNameId { get; set; }
    }
}
