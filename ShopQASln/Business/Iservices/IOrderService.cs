using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;
using Domain.Models;

namespace Business.Iservices
{
    public interface IOrderService
    {
        IEnumerable<OrderDto> GetAllOrderDtos();
        Task<OrderDto> CreateOrderAsync(OrderDto orderDto);
        Task<Order?> CreateOrderFromCartIdAsync(int cartId);
        List<OrderDto> GetOrdersByUserId(int userId);
        OrderDto? GetOrderWithDetails(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);

    }
}
