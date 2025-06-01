using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;
using Business.Iservices;
using DataAccess.IRepositories;
using Domain.Models;

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

        public async Task DeleteCategoryAsync(int id)
        {
            await categoryRepository.DeleteAsync(id);
        }
    }
}
