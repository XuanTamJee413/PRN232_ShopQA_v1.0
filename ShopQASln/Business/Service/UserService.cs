
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
                Role = user.Role,
                Status = user.Status
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
                Role = u.Role,
                Status = u.Status
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
        public async Task<IEnumerable<UserDTO>> FindByEmailOrUsernameAsync(string keyword)
        {
            var users = await _userRepository.FindByEmailOrUsernameAsync(keyword);
            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                Status = u.Status
            });
        }

        public async Task<IEnumerable<UserDTO>> FilterUsersByRoleAsync(string role)
        {
            var users = await _userRepository.FilterByRoleAsync(role);

            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role
            });
        }

        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");
            // Không hash, lưu trực tiếp mật khẩu mới
            await _userRepository.ChangePasswordAsync(userId, newPassword);
        }

        public async Task<UserDTO?> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetProfileAsync(userId);
            if (user == null) return null;
            // Lấy address đầu tiên (hoặc có thể lấy nhiều address nếu muốn)
            var address = user.Addresses.FirstOrDefault();
            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                // Không trả về Role khi view profile
                // Thông tin địa chỉ
                Address = address?.Street,
                City = address?.City,
                Country = address?.Country
            };
        }

        public async Task UpdatePersonalInfoAsync(UserDTO userDto)
        {
            var user = await _userRepository.GetByIdAsync(userDto.Id);
            if (user == null)
                throw new KeyNotFoundException("User not found.");
            user.Username = userDto.Username;
            user.Email = userDto.Email;
            // Không cập nhật Role
            await _userRepository.UpdatePersonalInfoAsync(user);
            // Cập nhật địa chỉ
            await _userRepository.UpdateAddressAsync(user.Id, userDto.Address, userDto.City, userDto.Country);
        }

        public async Task<bool> ValidatePasswordAsync(int userId, string oldPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;
            // So sánh trực tiếp với mật khẩu thuần
            return user.PasswordHash == oldPassword;
        }



        public async Task UpdateAccountStatusAsync(int userId, string status)
        {
            await _userRepository.UpdateAccountStatusAsync(userId, status);
        }
                public async Task<IEnumerable<UserDTO>> SearchUsersAsync(string keyword, string? role)
        {
            var users = await _userRepository.SearchUsersAsync(keyword, role);
            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                Status = u.Status
            });
        }
    }
}
