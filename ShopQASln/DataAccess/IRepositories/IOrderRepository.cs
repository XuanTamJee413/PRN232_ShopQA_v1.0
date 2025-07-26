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

        // Thống kê tổng số đơn hàng
        int GetTotalOrderCount();
        // Thống kê tổng doanh thu
        decimal GetTotalRevenue();
        // Lấy danh sách productvariant và số lượng bán ra
        IEnumerable<ProductVariantSalesModel> GetProductVariantSales();
    }
    public class ProductVariantSalesModel
{
    public int ProductVariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Price { get; set; }
}

   
}
