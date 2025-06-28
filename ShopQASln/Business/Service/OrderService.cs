using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;
using Business.Iservices;
using DataAccess.Context;
using DataAccess.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IEnumerable<OrderDto> GetAllOrderDtos()
        {
            var orders = _orderRepository.GetAllOrders();

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                User = new UserOrderDto
                {
                    Id = o.User.Id,
                    FullName = o.User.Username,
                    Email = o.User.Email
                },
                Items = o.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    ProductName = i.ProductVariant.Product.Name // chắc chắn không null
                }).ToList()
            });
        }

    }
}
