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

        public async Task<BrandDTO> AddAsync(BrandDTO brandDto)
        {
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

            existing.Name = brandDto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return false;

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
