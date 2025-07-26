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

        public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto)
        {
            var order = new Order
            {
                OrderDate = orderDto.OrderDate,
                TotalAmount = orderDto.TotalAmount,
                UserId = orderDto.User.Id,
                Items = orderDto.Items.Select(i => new OrderItem
                {
                    Quantity = i.Quantity,
                    Price = i.Price,
                    ProductVariantId = i.Id // Giả sử i.Id là ID của ProductVariant
                }).ToList()
            };

            await _orderRepository.AddOrderAsync(order);

            // Load lại để lấy đầy đủ thông tin (User, ProductName...)
            var savedOrder = _orderRepository.GetOrderById(order.Id);

            return new OrderDto
            {
                Id = savedOrder.Id,
                OrderDate = savedOrder.OrderDate,
                TotalAmount = savedOrder.TotalAmount,
                User = new UserOrderDto
                {
                    Id = savedOrder.User.Id,
                    FullName = savedOrder.User.Username,
                    Email = savedOrder.User.Email
                },
                Items = savedOrder.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    ProductName = i.ProductVariant.Product.Name
                }).ToList()
            };
        }
        public async Task<Order?> CreateOrderFromCartIdAsync(int cartId)
        {
            return await _orderRepository.CreateOrderFromCartIdAsync(cartId);
        }
        public List<OrderDto> GetOrdersByUserId(int userId)
        {
            var orders = _orderRepository.GetOrdersByUserId(userId); 

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
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
                    ProductName = i.ProductVariant.Product.Name
                }).ToList()
            }).ToList();
        }
        public OrderDto? GetOrderWithDetails(int orderId)
        {
            var order = _orderRepository.GetOrderById(orderId);

            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                User = new UserOrderDto
                {
                    Id = order.User.Id,
                    FullName = order.User.Username,
                    Email = order.User.Email
                },
                Items = order.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    ProductName = i.ProductVariant.Product.Name
                }).ToList()
            };
        }
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            return await _orderRepository.UpdateOrderStatusAsync(orderId, status);
        }
        public int GetTotalOrderCount()
        {
            return _orderRepository.GetTotalOrderCount();
        }

        public decimal GetTotalRevenue()
        {
            return _orderRepository.GetTotalRevenue();
        }

        public IEnumerable<ProductVariantSalesDto> GetProductVariantSales()
        {
            var data = _orderRepository.GetProductVariantSales();
            return data.Select(x => new ProductVariantSalesDto
            {
                ProductVariantId = x.ProductVariantId,
                ProductName = x.ProductName,
                Size = x.Size,
                Color = x.Color,
                QuantitySold = x.QuantitySold,
                Price = x.Price
            });
        }
    }
}
