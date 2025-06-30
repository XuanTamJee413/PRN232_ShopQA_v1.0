using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;

namespace Business.Iservices
{
    public interface IUserService
    {
        Task<UserDTO> CreateUserAsync(UserDTO userDto);
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task UpdateUserAsync(UserDTO userDto);
        Task DeleteUserAsync(int id);
        Task<UserDTO?> FindByEmailOrUsernameAsync(string keyword);
        Task<IEnumerable<UserDTO>> FilterUsersByRoleAsync(string role);
        Task ChangePasswordAsync(int userId, string newPassword);
        Task<UserDTO?> GetProfileAsync(int userId);
        Task UpdatePersonalInfoAsync(UserDTO userDto);
        Task<bool> ValidatePasswordAsync(int userId, string oldPassword);
      
        Task UpdateAccountStatusAsync(int userId, string status);
    }
}
