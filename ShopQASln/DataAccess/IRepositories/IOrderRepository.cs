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

    }
}
