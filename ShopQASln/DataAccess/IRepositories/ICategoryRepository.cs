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

        /// <summary>
        /// Kiểm tra xem tên danh mục đã tồn tại hay chưa.
        /// </summary>
        /// <param name="name">Tên danh mục cần kiểm tra.</param>
        /// <param name="excludeId">ID của danh mục cần loại trừ (dùng cho trường hợp cập nhật).</param>
        /// <returns>True nếu tên đã tồn tại, ngược lại là False.</returns>
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

      
        Task<IEnumerable<Category>> SearchByNameAsync(string keyword);
        Task<IEnumerable<Category>> SortByNameAsync(bool sortAsc);
    }
}
