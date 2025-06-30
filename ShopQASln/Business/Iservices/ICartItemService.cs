using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Iservices
{
    public interface ICartItemService
    {
        IQueryable<CartItem> GetCartItems();
        Task<CartItem?> GetCartItemByIdAsync(int id);
        Task CreateCartItemAsync(CartItem item);
        Task<CartItem> UpdateCartItemAsync(CartItem item);

        Task<bool> DeleteCartItemAsync(int id);
    }

}
