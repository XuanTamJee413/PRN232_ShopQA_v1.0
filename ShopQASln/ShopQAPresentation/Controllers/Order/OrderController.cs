using Business.Iservices;
using Business.Service;
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
        public IActionResult GetAllOrders()
        {
            var orders = _orderService.GetAllOrderDtos();
            return Ok(orders);
        }

    }
}
