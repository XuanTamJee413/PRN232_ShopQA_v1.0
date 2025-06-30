using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace DataAccess.IRepositories
{
     public interface IWishlistRepository
    {
        Task AddToWishlistAsync(int userId, int ProductId);
        Task<List<int>> GetWishlist(int userId);
        Task RemoveFromWishlistAsync(int userId, int ProductId);
        Task<Wishlist?> GetWishlistWithDetailsAsync(int ProductId);
    }
}
