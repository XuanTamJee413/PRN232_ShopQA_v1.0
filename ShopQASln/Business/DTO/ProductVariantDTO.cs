using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static Business.DTO.InventoryDTO;

namespace Business.DTO
{
    public class ProductVariantDTO
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; } = default!;
        public string Color { get; set; } = default!;
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public int ProductId { get; set; }
    }
    public class ProductVariantResponseDTO
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; } = default!;
        public string Color { get; set; } = default!;
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public int ProductId { get; set; }

        public InventoryResponseDTO? Inventory { get; set; }
    }

    public class ProductVariantWithInventoryUpdateDTO
    {
        public decimal Price { get; set; }
        public string Size { get; set; } = default!;
        public string Color { get; set; } = default!;
        public int Stock { get; set; }
        public IFormFile? ImageFile { get; set; } // For optional image update
        public string? ImageUrl { get; set; } // To retain existing URL if no new image

        public int InventoryQuantity { get; set; }
    }
    public class ProductVariantWithInventoryResDTO
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; } = default!;
        public string Color { get; set; } = default!;
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }

        public InventoryResponseDTO Inventory { get; set; } = default!;
    }


public class ProductVariantCreateDTO
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Stock { get; set; } // Can be removed or set to 0 as Inventory handles quantity
        public IFormFile? ImageFile { get; set; } // <-- New property for image file
        public string? ImageUrl { get; set; } // Keep this if you want to support both file upload and direct URL (less common for create)
        public int InventoryQuantity { get; set; }
    }

}
