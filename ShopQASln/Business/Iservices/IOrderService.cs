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
        // Thống kê tổng số đơn hàng
        int GetTotalOrderCount();
        // Thống kê tổng doanh thu
        decimal GetTotalRevenue();
        // Lấy danh sách productvariant và số lượng bán ra
        IEnumerable<ProductVariantSalesDto> GetProductVariantSales();
    }
}
