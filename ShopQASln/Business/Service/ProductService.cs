﻿using System;
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
using Microsoft.EntityFrameworkCore;
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

            if (productDTO.CategoryId == null)
            {
                throw new ArgumentException("Category is required.");
            }
            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Category = categoryRepository.GetById(productDTO.CategoryId),
                ImageUrl = null
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

            if (productDTO.CategoryId == null)
            {
                throw new ArgumentException("Category is required.");
            }
            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
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
