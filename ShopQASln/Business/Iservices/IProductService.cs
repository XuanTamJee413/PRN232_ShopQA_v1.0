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
        IEnumerable<Product> GetVisibleProducts(string? name, int? categoryId, int? brandId);

        IEnumerable<Product> GetAllProduct(string? name, int? categoryId, decimal? startPrice ,decimal? toPrice);

        Product getProductById(int id);
        Product AddProduct(ProductDTO productDTO);
        Product UpdateProduct(int productId, ProductDTO productDTO);

        String DeleteProduct(int id);

    }
}
