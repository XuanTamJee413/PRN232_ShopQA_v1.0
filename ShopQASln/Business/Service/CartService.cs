using DataAccess.Context;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Service
{
    internal class CartService
    {
        private readonly ShopQADbContext _context;

        public CartService(ShopQADbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => c.UserId == userId);
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
