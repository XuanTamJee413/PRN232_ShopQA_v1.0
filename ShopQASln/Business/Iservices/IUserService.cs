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
    }
}
