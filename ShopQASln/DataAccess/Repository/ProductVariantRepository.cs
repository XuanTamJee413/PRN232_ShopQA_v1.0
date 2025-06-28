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
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly ShopQADbContext _context;

        public ProductVariantRepository(ShopQADbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductVariant>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductVariants
                .Where(v => v.ProductId == productId)
                .ToListAsync();
        }
        public ProductVariant? GetVariantWithInventory(int variantId)
        {
            return _context.ProductVariants
                .Include(v => v.Inventory)
                .FirstOrDefault(v => v.Id == variantId);
        }

        public void Update(ProductVariant variant)
        {
            _context.ProductVariants.Update(variant);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        public void Add(ProductVariant variant)
        {
            _context.ProductVariants.Add(variant);
        }
    }
}
