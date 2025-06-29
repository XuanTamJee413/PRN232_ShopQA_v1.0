﻿using DataAccess.Context;
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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ShopQADbContext _context;

        public CategoryRepository(ShopQADbContext context)
        {
            _context = context;
        }
        public IEnumerable<Category> GetAll()
        {
            return _context.Categories.ToList();
        }

        public Category? GetById(int id)
        {
            return _context.Categories.FirstOrDefault(x => x.Id == id);
        }
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasProductsAsync(int categoryId)
        {
            return await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

       
        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
           
            var normalizedName = name.ToLower();
            var query = _context.Categories.Where(c => c.Name.ToLower() == normalizedName);

           
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
        public async Task<IEnumerable<Category>> SearchByNameAsync(string keyword)
        {
            var normalizedKeyword = keyword.ToLower();
            return await _context.Categories
                .Where(c => c.Name.ToLower().Contains(normalizedKeyword))
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> SortByNameAsync(bool sortAsc)
        {
            var query = _context.Categories.AsQueryable();

            query = sortAsc
                ? query.OrderBy(c => c.Name)
                : query.OrderByDescending(c => c.Name);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Category>> SearchSortPagedAsync(string? keyword, bool? sortAsc, int page, int pageSize)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(c => c.Name.ToLower().Contains(keyword.ToLower()));
            }

            if (sortAsc.HasValue)
            {
                query = sortAsc.Value
                    ? query.OrderBy(c => c.Name)
                    : query.OrderByDescending(c => c.Name);
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

    }
}
