using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Business.DTO;
using Business.Iservices;
using DataAccess;
using DataAccess.IRepositories;
using Domain.Models;
using Microsoft.IdentityModel.Tokens;

namespace Business.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
        }
        public IEnumerable<Product> GetAllProduct(string? name, int? categoryId, decimal? startPrice, decimal? toPrice)
        {
            var products = productRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(name))
            {
                products = products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            if (startPrice.HasValue && toPrice.HasValue)
            {
                products = products.Where(p => p.Price >= startPrice && p.Price <= toPrice);
            }
            else
            {
                if (startPrice.HasValue)
                {
                    products = products.Where(p => p.Price >= startPrice);
                }

                if (toPrice.HasValue)
                {
                    products = products.Where(p => p.Price <= toPrice);
                }
            }

            //if (!products.Any())
            //{
            //    return 0;
            //}

            return products;
        }


        public Product getProductById(int id)
        {
            Product product = productRepository.GetById(id);
            if (product == null)
            {
                return null;
            }
            return product;
        }
        public Product AddProduct(ProductDTO productDTO)
        {
            if (string.IsNullOrWhiteSpace(productDTO.Name)) { 
    
                throw new ArgumentException("Name is required.");
            }
            if (!Regex.IsMatch(productDTO.Name, @"^[a-zA-ZÀ-Ỹà-ỹ\s]+$"))
            {
                throw new ArgumentException("Name must contain only letters and spaces.");
            }

            if (string.IsNullOrWhiteSpace(productDTO.Description))
            {
                throw new ArgumentException("Description is required.");
            }

            if (productDTO.Price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.");
            }

            if (productDTO.CategoryId == null)
            {
                throw new ArgumentException("Category is required.");
            }
            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Category = categoryRepository.GetById(productDTO.CategoryId)
            };

                productRepository.Add(product);

                return product;
            }

        public Product UpdateProduct(int productId,ProductDTO productDTO)
        {

            Product product = productRepository.GetById(productId);
            if (product == null) {
                throw new ArgumentException("Not Found!");
            }
            if (string.IsNullOrWhiteSpace(productDTO.Name))
            {

                throw new ArgumentException("Name is required.");
            }
            if (!Regex.IsMatch(productDTO.Name, @"^[a-zA-ZÀ-Ỹà-ỹ\s]+$"))
            {
                throw new ArgumentException("Name must contain only letters and spaces.");
            }

            if (string.IsNullOrWhiteSpace(productDTO.Description))
            {
                throw new ArgumentException("Description is required.");
            }

            if (productDTO.Price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.");
            }

            if (productDTO.CategoryId == null)
            {
                throw new ArgumentException("Category is required.");
            }
            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Price = productDTO.Price;
            product.Category = categoryRepository.GetById(productDTO.CategoryId);
            productRepository.Update(product);
            return product;
        }

        public string DeleteProduct(int id)
        {

            Product product = productRepository.GetById(id);
            if (product == null)
            {
                throw new ArgumentException("Not Found!");
            }
            productRepository.Delete(id);
            return "Delete Succesfull!";
        }
    }
}
