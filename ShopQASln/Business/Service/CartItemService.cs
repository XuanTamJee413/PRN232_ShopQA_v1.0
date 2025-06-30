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
    public class CartItemService : ICartItemService
    {
        private readonly ShopQADbContext _context;

        public CartItemService(ShopQADbContext context)
        {
            _context = context;
        }

        public IQueryable<CartItem> GetCartItems()
        {
            // Load luôn ProductVariant nếu cần
            return _context.CartItem.Include(i => i.ProductVariant);
        }

        public async Task<CartItem?> GetCartItemByIdAsync(int id)
        {
            return await _context.CartItem
                .Include(i => i.ProductVariant)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task CreateCartItemAsync(CartItem item)
        {
            _context.CartItem.Add(item);
            await _context.SaveChangesAsync();
        }
        public async Task<CartItem> UpdateCartItemAsync(CartItem item)
        {
            _context.CartItem.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }


        public async Task<bool> DeleteCartItemAsync(int id)
        {
            var item = await _context.CartItem.FindAsync(id);
            if (item == null) return false;

            _context.CartItem.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
