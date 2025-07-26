using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace DataAccess.IRepositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAllOrders();
        Task AddOrderAsync(Order order);
        Order GetOrderById(int id);
        Task<Order?> CreateOrderFromCartIdAsync(int cartId);
        List<Order> GetOrdersByUserId(int userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);

    }
}
