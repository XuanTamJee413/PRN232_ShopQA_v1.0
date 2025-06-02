using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;
using Domain.Models;

namespace Business.Iservices
{
    public interface ICategoryService
    {
        IEnumerable<Category> getAll();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(CategoryDTO category);
        Task UpdateCategoryAsync(int CategoryId, CategoryDTO category);
        Task<bool> DeleteCategoryAsync(int id);

    }
}
