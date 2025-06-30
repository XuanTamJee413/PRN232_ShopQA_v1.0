using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;
using Business.Iservices;
using DataAccess.IRepositories;

namespace Business.Service
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task AddToWishlistAsync(int userId, int productId)
        {
            await _wishlistRepository.AddToWishlistAsync(userId, productId);
        }

        public async Task<WishlistDTO> GetWishlistAsync(int userId)
        {
            var wishlist = await _wishlistRepository.GetWishlistWithDetailsAsync(userId);
            if (wishlist == null)
                return new WishlistDTO { UserId = userId, Items = new List<WishlistItemDTO>() };
            var dto = new WishlistDTO
            {
                Id = wishlist.Id,
                UserId = wishlist.UserId,
                CreatedAt = wishlist.CreatedAt,
                Items = wishlist.Items.Select(i => new WishlistItemDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    AddedAt = i.AddedAt,
                    ProductName = i.Product?.Name ?? string.Empty,
                    ProductImageUrl = i.Product?.ImageUrl ?? string.Empty,
                    ProductDescription = i.Product?.Description ?? string.Empty
                }).ToList()
            };
            return dto;
        }

        public async Task RemoveFromWishlistAsync(int userId, int productId)
        {
            await _wishlistRepository.RemoveFromWishlistAsync(userId, productId);
        }
    }
  
}
