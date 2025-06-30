using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;
using Domain.Models;

namespace Business.Iservices
{
    public interface IProductService
    {
        IQueryable<Product> GetQueryableVisibleProducts();
        IEnumerable<Product> GetVisibleProducts(string? name, int? categoryId, int? brandId);

        IEnumerable<ProductResponseDTO> GetAllProduct(string? name, int? categoryId, decimal? startPrice ,decimal? toPrice);

        ProductResponseDTO GetProductById(int id);
        Product AddProduct(ProductCreateReqDTO productDTO);
        Product UpdateProduct(int id, ProductCreateReqDTO productDTO);

        String DeleteProduct(int id);
        ProductVariantWithInventoryResDTO UpdateVariantWithInventory(int variantId, ProductVariantWithInventoryUpdateDTO dto);

        ProductVariantWithInventoryResDTO CreateVariant(ProductVariantCreateDTO dto);

    }
}
