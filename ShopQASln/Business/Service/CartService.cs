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
    public class CartService : ICartService
    {
        private readonly ShopQADbContext _context;

        public CartService(ShopQADbContext context)
        {
            _context = context;
        }

        public IQueryable<Cart> GetCarts()
        {
            return _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                .OrderByDescending(c => c.CreatedAt);
        }



        public async Task<Cart?> GetCartByIdAsync(int id)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateCartAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteCartAsync(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null) return false;

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddItemToCartAsync(CartItem item)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == item.CartId);

            if (cart == null) return false;

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == item.ProductVariantId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveItemFromCartAsync(int cartId, int itemId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null) return false;

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return false;

            cart.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
