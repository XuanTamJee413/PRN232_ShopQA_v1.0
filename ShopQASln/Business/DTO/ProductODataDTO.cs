using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class ProductODataDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string? ImageUrl { get; set; }

        public CategoryDTO? Category { get; set; }
        public BrandDTO? Brand { get; set; }
        public List<ProductVariantDTO> Variants { get; set; } = new();
    }

}
