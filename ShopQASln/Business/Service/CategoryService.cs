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
           
            if (await categoryRepository.ExistsByNameAsync(category.Name))
            {
                
                throw new InvalidOperationException("Không được trùng tên");
            }

           
            Category ca = new Category();
            ca.Name = category.Name;
            await categoryRepository.AddAsync(ca);
        }

        public async Task UpdateCategoryAsync(int categoryId, CategoryDTO category)
        {
          
            if (await categoryRepository.ExistsByNameAsync(category.Name, categoryId))
            {
               
                throw new InvalidOperationException("Không được trùng tên");
            }

           
            var ca = await categoryRepository.GetByIdAsync(categoryId);
            if (ca == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy danh mục với ID {categoryId}.");
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
                return false; 

            await categoryRepository.DeleteAsync(category);
            return true;
        }
        public async Task<IEnumerable<Category>> SearchCategoriesByNameAsync(string keyword)
        {
            return await categoryRepository.SearchByNameAsync(keyword);
        }

        public async Task<IEnumerable<Category>> SortCategoriesByNameAsync(bool sortAsc)
        {
            return await categoryRepository.SortByNameAsync(sortAsc);
        }

        public async Task<IEnumerable<Category>> SearchSortPagedCategoriesAsync(string? keyword, bool? sortAsc, int page, int pageSize)
        {
            return await categoryRepository.SearchSortPagedAsync(keyword, sortAsc, page, pageSize);
        }

    }
}
