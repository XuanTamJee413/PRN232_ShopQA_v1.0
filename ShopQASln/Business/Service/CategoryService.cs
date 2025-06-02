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
            Category ca = new Category();
            ca.Id = category.Id;
            ca.Name = category.Name;
            await categoryRepository.AddAsync(ca);
        }

        public async Task UpdateCategoryAsync(int CategoryId, CategoryDTO category)
        {   
           
            Category ca = categoryRepository.GetById(CategoryId);
         
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
