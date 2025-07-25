using DataAccess.Context;
using DataAccess.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ShopQADbContext _context;

        public OrderRepository(ShopQADbContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(pv => pv.Product) // nếu cần tên sản phẩm
                .Include(o => o.User) // nếu cần thông tin người dùng
                .ToList();
        }
        public async Task AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public Order GetOrderById(int id)
        {
            return _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Include(o => o.User)
                .First(o => o.Id == id);
        }
        public async Task<Order?> CreateOrderFromCartIdAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null || !cart.Items.Any()) return null;

            var order = new Order
            {
                UserId = cart.UserId,
                TotalAmount = cart.Items.Sum(i => i.ProductVariant!.Price * i.Quantity),
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductVariantId = i.ProductVariantId,
                    Quantity = i.Quantity,
                    Price = i.ProductVariant!.Price
                }).ToList()
            };

            _context.Orders.Add(order);


            //giam quatity product
            foreach (var cartItem in cart.Items)
            {
                if (cartItem.ProductVariant != null)
                {
                    if (cartItem.ProductVariant.Stock < cartItem.Quantity)
                    {
                        throw new InvalidOperationException($"Sản phẩm {cartItem.ProductVariant.Id} không đủ hàng tồn kho.");
                    }
                    cartItem.ProductVariant.Stock -= cartItem.Quantity;
                    _context.ProductVariants.Update(cartItem.ProductVariant);
                }
            }

            cart.Status = "Close";
            _context.Carts.Update(cart);

            await _context.SaveChangesAsync();
            return order;
        }

    }
}
