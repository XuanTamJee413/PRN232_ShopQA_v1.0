using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Iservices
{
    public interface IBrandService
    {
        Task<List<Brand>> GetAllAsync();
        Task<Brand?> GetByIdAsync(int id);
        Task<Brand> AddAsync(Brand brand);
        Task<bool> UpdateAsync(Brand brand);
        Task<bool> DeleteAsync(int id);
    }
}
