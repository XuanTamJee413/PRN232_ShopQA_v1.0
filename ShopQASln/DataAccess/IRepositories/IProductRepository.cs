using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        IQueryable<Product> GetAllQueryable();
        Product? GetById(int id);
        Product? GetProductById(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);

        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
