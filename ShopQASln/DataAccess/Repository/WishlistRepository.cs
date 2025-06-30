using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Context;
using DataAccess.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ShopQADbContext _context;
        public WishlistRepository(ShopQADbContext context)
        {
            _context = context;
        }

        public async Task AddToWishlistAsync(int userId, int productId)
        {
            var wishlist = await _context.Wishlists.Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    Items = new List<WishlistItem>()
                };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }
            // Không cho thêm trùng
            if (wishlist.Items.Any(i => i.ProductId == productId))
                return;
            var item = new WishlistItem
            {
                WishlistId = wishlist.Id,
                ProductId = productId,
                AddedAt = DateTime.Now
            };
            _context.WishlistItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> GetWishlist(int userId)
        {
            var wishlist = await _context.Wishlists.Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist == null) return new List<int>();
            return wishlist.Items.Select(i => i.ProductId).ToList();
        }

        public async Task RemoveFromWishlistAsync(int userId, int productId)
        {
            var wishlist = await _context.Wishlists.Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist == null) return;
            var item = wishlist.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Wishlist?> GetWishlistWithDetailsAsync(int userId)
        {
            return await _context.Wishlists
                .Include(w => w.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }
    }
}
