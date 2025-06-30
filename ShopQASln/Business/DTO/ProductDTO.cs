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
        public int? Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        
    }
    public class ProductCreateReqDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }
    public class ProductResponseDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public Brand Brand { get; set; } = default!;

        public ICollection<ProductVariantResponseDTO> Variants { get; set; } = new List<ProductVariantResponseDTO>();
    }

}
