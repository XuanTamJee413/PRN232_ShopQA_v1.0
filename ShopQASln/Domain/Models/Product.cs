using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string? ImageUrl { get; set; }

        public Category Category { get; set; } = default!;
        public Brand Brand { get; set; } = default!;

        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }

}
