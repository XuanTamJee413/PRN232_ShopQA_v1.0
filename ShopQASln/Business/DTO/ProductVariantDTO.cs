using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string? ImageUrl { get; set; }

        // Thêm phần cập nhật inventory
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
        public string Size { get; set; } = default!;
        public string Color { get; set; } = default!;
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }

        // Inventory
        public int InventoryQuantity { get; set; }
    }

}
