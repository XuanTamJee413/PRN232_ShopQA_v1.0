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
        Task<User?> FindByEmailOrUsernameAsync(string emailOrUsername);
        Task<IEnumerable<User>> FilterByRoleAsync(string role);
        Task ChangePasswordAsync(int userId, string newPasswordHash);
        Task<User?> GetProfileAsync(int userId);
        Task UpdatePersonalInfoAsync(User user);
        Task UpdateAddressAsync(int userId, string? address, string? city, string? country);
        Task UpdateAccountStatusAsync(int userId, string status);
    }
}
