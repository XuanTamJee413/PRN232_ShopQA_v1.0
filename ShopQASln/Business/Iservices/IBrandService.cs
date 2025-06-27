using Business.DTO;
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
        Task<List<BrandDTO>> GetAllAsync();
        Task<BrandDTO?> GetByIdAsync(int id);
        Task<BrandDTO> AddAsync(BrandDTO brandDto);
        Task<bool> UpdateAsync(BrandDTO brandDto);
        Task<bool> DeleteAsync(int id);
        Task<List<BrandDTO>> SearchByNameAsync(string name);
        Task<List<BrandDTO>> SortByNameAsync(bool descending);
    }
}
