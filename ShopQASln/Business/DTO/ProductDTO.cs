using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Business.DTO
{
    public class ProductDTO
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
