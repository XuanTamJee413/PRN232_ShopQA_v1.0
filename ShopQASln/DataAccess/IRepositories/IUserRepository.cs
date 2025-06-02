using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace DataAccess.IRepositories
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user);
        
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
       
        Task UpdateAsync(User user);
        
        Task DeleteAsync(int id);
    }
}
