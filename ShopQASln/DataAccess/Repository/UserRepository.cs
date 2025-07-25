
using DataAccess.Context;
using DataAccess.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly ShopQADbContext _context;
        public UserRepository(ShopQADbContext context)
        {
            _context = context;
        }
        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<User>> FindByEmailOrUsernameAsync(string emailOrUsername)
        {
            return await _context.Users
                .Include(u => u.Addresses)
                .Where(u =>
                    u.Email.Contains(emailOrUsername) ||
                    u.Username.Contains(emailOrUsername)
                )
                .ToListAsync();
        }
        public async Task<IEnumerable<User>> FilterByRoleAsync(string role)
        {
            return await _context.Users
                .Include(u => u.Addresses)
                .Where(u => u.Role == role)
                .ToListAsync();
        }
        public async Task ChangePasswordAsync(int userId, string newPasswordHash)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.PasswordHash = newPasswordHash;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<User?> GetProfileAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task UpdatePersonalInfoAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser != null)
            {
                existingUser.Username = user.Username;
                existingUser.Email = user.Email;
                // Không cập nhật Role
                existingUser.Status = user.Status;
                // Không cập nhật PasswordHash ở đây
                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<User>> SearchUsersAsync(string keyword, string? role)
        {
            var query = _context.Users.Include(u => u.Addresses).AsQueryable();

            bool hasKeyword = !string.IsNullOrWhiteSpace(keyword);
            bool hasRole = !string.IsNullOrWhiteSpace(role) && role != "All";

            if (hasKeyword && hasRole)
            {
                query = query.Where(u => (u.Username.Contains(keyword) || u.Email.Contains(keyword)) && u.Role == role);
            }
            else if (hasKeyword)
            {
                query = query.Where(u => u.Username.Contains(keyword) || u.Email.Contains(keyword));
            }
            else if (hasRole)
            {
                query = query.Where(u => u.Role == role);
            }
            // Nếu không có keyword và role thì trả về tất cả

            return await query.ToListAsync();
        }
        public async Task UpdateAddressAsync(int userId, string? address, string? city, string? country)
        {
            var user = await _context.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Id == userId);
            var addr = user?.Addresses.FirstOrDefault();
            if (addr != null)
            {
                addr.Street = address ?? addr.Street;
                addr.City = city ?? addr.City;
                addr.Country = country ?? addr.Country;
                _context.Addresses.Update(addr);
            }
            else if (user != null)
            {
                // Nếu chưa có address thì tạo mới
                var newAddr = new Domain.Models.Address
                {
                    Street = address ?? string.Empty,
                    City = city ?? string.Empty,
                    Country = country ?? string.Empty,
                    UserId = user.Id
                };
                _context.Addresses.Add(newAddr);
            }
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAccountStatusAsync(int userId, string status)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null && user.Status != status)
            {
                user.Status = status;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
