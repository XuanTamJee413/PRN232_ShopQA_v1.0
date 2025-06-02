using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ProductVariant
    {
        // biến thể của một product 
        public int Id { get; set; }
        public string Size { get; set; } = default!;
        public string Color { get; set; } = default!;
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;
    }

}
