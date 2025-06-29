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
        public Task<List<Cart>> GetCartsByUserIdAsync(int userId);
        Task<bool> AddItemToCartAsync(CartItem item);
        Task<bool> RemoveItemFromCartAsync(int cartId, int itemId);
    }

}
