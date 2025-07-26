using Business.DTO;
using Business.Iservices;
using Business.Service;
using OrderModel = Domain.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShopQAPresentation.Controllers.Order
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllOrders()
        {
            var orders = _orderService.GetAllOrderDtos();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null || orderDto.Items == null || !orderDto.Items.Any())
                return BadRequest("Order must contain at least one item.");

            var createdOrder = await _orderService.CreateOrderAsync(orderDto);
            return CreatedAtAction(nameof(GetAllOrders), new { id = createdOrder.Id }, createdOrder);
        }
        //tamnx
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(int cartId)
        {
            var order = await _orderService.CreateOrderFromCartIdAsync(cartId);
            if (order == null) return BadRequest("Giỏ hàng trống hoặc không tồn tại.");

            return Ok(new
            {
                order.Id,
                order.OrderDate,
                order.TotalAmount
            });
        }

        // Thống kê tổng số đơn hàng
        [HttpGet("total-count")]
        
        public IActionResult GetTotalOrderCount()
        {
            var count = _orderService.GetTotalOrderCount();
            return Ok(count);
        }

        // Thống kê tổng doanh thu
        [HttpGet("total-revenue")]
        
        public IActionResult GetTotalRevenue()
        {
            var revenue = _orderService.GetTotalRevenue();
            return Ok(revenue);
        }

        // Lấy danh sách productvariant và số lượng bán ra
        [HttpGet("product-variant-sales")]
       
        public IActionResult GetProductVariantSales()
        {
            var sales = _orderService.GetProductVariantSales();
            return Ok(sales);
        }


    }
}
