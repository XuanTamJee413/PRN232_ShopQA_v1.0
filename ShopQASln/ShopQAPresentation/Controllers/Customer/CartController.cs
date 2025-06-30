using Business.DTO;
using Business.Iservices;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace ShopQAPresentation.Controllers.Customer
{
    [Authorize(Roles = "Customer")]
    public class CartController : ODataController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [EnableQuery(MaxExpansionDepth = 5)]

        public IActionResult Get()
        {
            var carts = _cartService.GetCarts();
            return Ok(carts);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var cart = await _cartService.GetCartByIdAsync(key);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        public async Task<IActionResult> Post([FromBody] Cart cart)
        {
            await _cartService.CreateCartAsync(cart);
            return Created(cart);
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            var result = await _cartService.DeleteCartAsync(key);
            return result ? NoContent() : NotFound();
        }
    }
}
