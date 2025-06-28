using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace DataAccess.IRepositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        Category? GetById(int id);
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task<bool> HasProductsAsync(int categoryId);
        Task DeleteAsync(Category category);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<IEnumerable<Category>> SearchByNameAsync(string keyword);
        Task<IEnumerable<Category>> SortByNameAsync(bool sortAsc);
        Task<IEnumerable<Category>> SearchSortPagedAsync(string? keyword, bool? sortAsc, int page, int pageSize);

    }
}
