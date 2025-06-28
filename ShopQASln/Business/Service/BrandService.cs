using Business.DTO;
using Business.Iservices;
using DataAccess.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Service
{
    public class BrandService : IBrandService
    {
        private readonly ShopQADbContext _context;

        public BrandService(ShopQADbContext context)
        {
            _context = context;
        }

        public async Task<List<BrandDTO>> GetAllAsync()
        {
            return await _context.Brands
                .Select(b => new BrandDTO
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToListAsync();
        }

        public async Task<BrandDTO?> GetByIdAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return null;

            return new BrandDTO
            {
                Id = brand.Id,
                Name = brand.Name
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _context.Brands
                .Include(b => b.Products)  
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null)
                return false;

            if (brand.Products != null && brand.Products.Any())
                throw new InvalidOperationException("Không Thể Xóa Vì Đang Tồn Tại Sản Phẩm Thuộc Brand Này.");

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BrandDTO> AddAsync(BrandDTO brandDto)
        {
            var exists = await _context.Brands
                .AnyAsync(b => b.Name.ToLower() == brandDto.Name.ToLower());
            if (exists)
                throw new InvalidOperationException("Tên Brand Đã Tồn Tại");

            var brand = new Brand
            {
                Name = brandDto.Name
            };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            brandDto.Id = brand.Id;
            return brandDto;
        }

        public async Task<bool> UpdateAsync(BrandDTO brandDto)
        {
            var existing = await _context.Brands.FindAsync(brandDto.Id);
            if (existing == null) return false;

            var nameExists = await _context.Brands
                .AnyAsync(b => b.Id != brandDto.Id && b.Name.ToLower() == brandDto.Name.ToLower());
            if (nameExists)
                throw new InvalidOperationException("Tên Brand Đã Tồn Tại");

            existing.Name = brandDto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BrandDTO>> SearchByNameAsync(string name)
        {
            var query = _context.Brands
                .Where(b => b.Name.Contains(name));

            return await query
                .Select(b => new BrandDTO
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToListAsync();
        }

        public async Task<List<BrandDTO>> SortByNameAsync(bool descending)
        {
            var query = _context.Brands.AsQueryable();

            query = descending
                ? query.OrderByDescending(b => b.Name)
                : query.OrderBy(b => b.Name);

            return await query
                .Select(b => new BrandDTO
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToListAsync();
        }

        public async Task<List<BrandDTO>> GetPagedAsync(string? search, string? sort, int page)
        {
            const int pageSize = 5;
            var query = _context.Brands.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                if (sort.ToLower() == "asc")
                    query = query.OrderBy(x => x.Name);
                else if (sort.ToLower() == "desc")
                    query = query.OrderByDescending(x => x.Name);
            }

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new BrandDTO
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

            return result;
        }

    }
}
