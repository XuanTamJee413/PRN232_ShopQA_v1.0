using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;
using Business.Iservices;
using DataAccess.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        
        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public IEnumerable<Category> getAll()
        {
            var category = categoryRepository.GetAll();
            return category;
        }
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await categoryRepository.GetByIdAsync(id);
        }

        public async Task AddCategoryAsync(CategoryDTO category)
        {
            
            var existingCategories = categoryRepository.GetAll();

           
            if (existingCategories.Any(c => c.Name.Trim().ToLower() == category.Name.Trim().ToLower()))
            {
                throw new Exception("Tên danh mục đã tồn tại.");
            }

           
            Category ca = new Category
            {
                Id = category.Id,
                Name = category.Name
            };
            await categoryRepository.AddAsync(ca);
        }

        public async Task UpdateCategoryAsync(int categoryId, CategoryDTO category)
        {
            var ca = categoryRepository.GetById(categoryId);
            if (ca == null)
                throw new Exception("Không tìm thấy danh mục.");

            // Kiểm tra tên trùng với danh mục khác (không phải chính nó)
            var existingCategories = categoryRepository.GetAll();
            if (existingCategories.Any(c => c.Id != categoryId &&
                                            c.Name.Trim().ToLower() == category.Name.Trim().ToLower()))
            {
                throw new Exception("Tên danh mục đã tồn tại.");
            }

            ca.Name = category.Name;
            await categoryRepository.UpdateAsync(ca);
        }


        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Không tìm thấy danh mục.");

            bool hasProducts = await categoryRepository.HasProductsAsync(id);
            if (hasProducts)
                return false; // Không xóa nếu có sản phẩm

            await categoryRepository.DeleteAsync(category);
            return true;
        }


    }
}
