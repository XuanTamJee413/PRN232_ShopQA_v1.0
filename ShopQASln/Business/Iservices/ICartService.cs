using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Iservices
{
    public interface ICartService
    {
        IQueryable<Cart> GetCarts();
        Task<Cart?> GetCartByIdAsync(int id);
        Task CreateCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(int id);
        Task<bool> AddItemToCartAsync(CartItem item);
        Task<bool> RemoveItemFromCartAsync(int cartId, int itemId);
    }

}
