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
using DataAccess.Repository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static Business.DTO.InventoryDTO;

namespace Business.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IBrandRepository brandRepository;
        private readonly IProductVariantRepository _variantRepo;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IBrandRepository brandRepository, IProductVariantRepository variantRepo)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.brandRepository = brandRepository;
            _variantRepo = variantRepo;
        }
        public IQueryable<Product> GetQueryableVisibleProducts()
        {
            return productRepository.GetAllQueryable();
        }
        // Tamnx lay product hien thi cho Shop.html
        public IEnumerable<Product> GetVisibleProducts(string? name, int? categoryId, int? brandId)
        {
            var products = productRepository.GetAll();

            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }
            if (brandId.HasValue)
                products = products.Where(p => p.BrandId == brandId.Value);

            return products;
        }




        public ProductResponseDTO GetProductById(int id)
        {
            var product = productRepository.GetProductById(id);
            if (product == null)
            {
                throw new ArgumentException("Product not found.");
            }

            return new ProductResponseDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.Category.Id,
                CategoryName = product.Category.Name,
                ImageUrl = product.ImageUrl,
                Brand = product.Brand,
                Variants = product.Variants.Select(v => new ProductVariantResponseDTO
                {
                    Id = v.Id,
                    Price = v.Price,
                    Size = v.Size,
                    Color = v.Color,
                    Stock = v.Stock,
                    ImageUrl = v.ImageUrl,
                    ProductId = v.ProductId,
                    Inventory = v.Inventory != null ? new InventoryResponseDTO
                    {
                        Id = v.Inventory.Id,
                        Quantity = v.Inventory.Quantity,
                        UpdatedAt = v.Inventory.UpdatedAt
                    } : null
                }).ToList()
            };
        }


        public Product AddProduct(ProductCreateReqDTO productDTO)
        {
            if (string.IsNullOrWhiteSpace(productDTO.Name))
                throw new ArgumentException("Name is required.");

            if (!Regex.IsMatch(productDTO.Name, @"^[a-zA-ZÀ-ỹà-ỹ\s]+$"))
                throw new ArgumentException("Name must contain only letters and spaces.");

            if (string.IsNullOrWhiteSpace(productDTO.Description))
                throw new ArgumentException("Description is required.");

            if (productDTO.CategoryId <= 0)
                throw new ArgumentException("Invalid category ID.");

            var category = categoryRepository.GetById(productDTO.CategoryId);
            if (category == null)
                throw new ArgumentException("Category not found.");

            var brand = brandRepository.GetById(productDTO.BrandId);
            if (brand == null)
                throw new ArgumentException("Brand not found.");

            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Category = category,
                Brand = brand,
                ImageUrl = null
            };

            productRepository.Add(product);
            return product;
        }


        public Product UpdateProduct(int productId, ProductCreateReqDTO productDTO)
        {
            var product = productRepository.GetById(productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found!");
            }

            // Kiểm tra tên
            if (string.IsNullOrWhiteSpace(productDTO.Name))
            {
                throw new ArgumentException("Name is required.");
            }

            if (!Regex.IsMatch(productDTO.Name, @"^[a-zA-ZÀ-Ỹà-ỹ\s]+$"))
            {
                throw new ArgumentException("Name must contain only letters and spaces.");
            }

            // Kiểm tra mô tả
            if (string.IsNullOrWhiteSpace(productDTO.Description))
            {
                throw new ArgumentException("Description is required.");
            }

            // Kiểm tra Category
            var category = categoryRepository.GetById(productDTO.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Invalid CategoryId.");
            }

            // Kiểm tra Brand
            var brand = brandRepository.GetById(productDTO.BrandId);
            if (brand == null)
            {
                throw new ArgumentException("Invalid BrandId.");
            }

            // Cập nhật thông tin
            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Category = category;
            product.Brand = brand;

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

        public IEnumerable<ProductResponseDTO> GetAllProduct(string? name, int? categoryId, decimal? startPrice, decimal? toPrice)
        {
            var products = productRepository.GetAll();

            // Lọc theo tên sản phẩm
            if (!string.IsNullOrWhiteSpace(name))
            {
                products = products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            // Lọc theo khoảng giá trong các biến thể
            if (startPrice.HasValue)
            {
                products = products.Where(p => p.Variants.Any(v => v.Price >= startPrice.Value));
            }

            if (toPrice.HasValue)
            {
                products = products.Where(p => p.Variants.Any(v => v.Price <= toPrice.Value));
            }

            // Ánh xạ sang DTO (chuyển từ entity sang response model)
            var result = products.Select(p => new ProductResponseDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                ImageUrl = p.ImageUrl,
                Brand = new Brand
                {
                    Id = p.Brand.Id,
                    Name = p.Brand.Name
                    // thêm thuộc tính khác nếu cần
                },
                Variants = p.Variants.Select(v => new ProductVariantResponseDTO
                {
                    Id = v.Id,
                    Price = v.Price,
                    Size = v.Size,
                    Color = v.Color,
                    Stock = v.Stock,
                    ImageUrl = v.ImageUrl,
                    ProductId = v.ProductId,
                    Inventory = v.Inventory != null ? new InventoryResponseDTO
                    {
                        Id = v.Inventory.Id,
                        Quantity = v.Inventory.Quantity,
                        UpdatedAt = v.Inventory.UpdatedAt
                    } : null
                }).ToList()

            });

            return result;
        }
        public ProductVariantWithInventoryResDTO UpdateVariantWithInventory(int variantId, ProductVariantWithInventoryUpdateDTO dto)
        {
            var variant = _variantRepo.GetVariantWithInventory(variantId)
                ?? throw new ArgumentException("Không tìm thấy biến thể sản phẩm.");

            variant.Price = dto.Price;
            variant.Size = dto.Size;
            variant.Color = dto.Color;
            variant.Stock = dto.Stock;
            variant.ImageUrl = dto.ImageUrl;

            if (variant.Inventory != null)
            {
                variant.Inventory.Quantity = dto.InventoryQuantity;
                variant.Inventory.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                variant.Inventory = new Inventory
                {
                    ProductVariantId = variant.Id,
                    Quantity = dto.InventoryQuantity,
                    UpdatedAt = DateTime.UtcNow
                };
            }

            _variantRepo.Update(variant);
            _variantRepo.Save();

            return new ProductVariantWithInventoryResDTO
            {
                Id = variant.Id,
                Price = variant.Price,
                Size = variant.Size,
                Color = variant.Color,
                Stock = variant.Stock,
                ImageUrl = variant.ImageUrl,
                Inventory = new InventoryResponseDTO
                {
                    Id = variant.Inventory.Id,
                    Quantity = variant.Inventory.Quantity,
                    UpdatedAt = variant.Inventory.UpdatedAt
                }
            };
        }

        public ProductVariantWithInventoryResDTO CreateVariant(ProductVariantCreateDTO dto)
        {
            var variant = new ProductVariant
            {
                ProductId = dto.ProductId,
                Price = dto.Price,
                Size = dto.Size,
                Color = dto.Color,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                Inventory = new Inventory
                {
                    Quantity = dto.InventoryQuantity,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _variantRepo.Add(variant);
            _variantRepo.Save();

            return new ProductVariantWithInventoryResDTO
            {
                Id = variant.Id,
                Price = variant.Price,
                Size = variant.Size,
                Color = variant.Color,
                Stock = variant.Stock,
                ImageUrl = variant.ImageUrl,
                Inventory = new InventoryResponseDTO
                {
                    Id = variant.Inventory.Id,
                    Quantity = variant.Inventory.Quantity,
                    UpdatedAt = variant.Inventory.UpdatedAt
                }
            };
        }

    }
}
