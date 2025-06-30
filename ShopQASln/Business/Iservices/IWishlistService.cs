using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;

namespace Business.Iservices
{
    public interface IWishlistService
    {
        Task AddToWishlistAsync(int userId, int productId);
        Task<WishlistDTO> GetWishlistAsync(int userId);
        Task RemoveFromWishlistAsync(int userId, int productId);
    }
}
