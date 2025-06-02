using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;
using Business.Iservices;
using DataAccess.IRepositories;
using Domain.Models;

namespace Business.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDTO> CreateUserAsync(UserDTO userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Password))
            {
                throw new ArgumentException("Password is required when creating a new user.");
            }

            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                Role = userDto.Role,
                PasswordHash = HashPassword(userDto.Password)
            };

            var createdUser = await _userRepository.AddAsync(user);

            return new UserDTO
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                Role = createdUser.Role
                // Không trả password về
            };
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role
            });
        }

        public async Task UpdateUserAsync(UserDTO userDto)
        {
            var user = await _userRepository.GetByIdAsync(userDto.Id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.Role = userDto.Role;

            if (!string.IsNullOrWhiteSpace(userDto.Password))
            {
                user.PasswordHash = HashPassword(userDto.Password);
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
