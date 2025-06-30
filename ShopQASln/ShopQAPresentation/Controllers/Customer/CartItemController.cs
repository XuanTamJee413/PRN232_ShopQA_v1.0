using Business.Iservices;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace ShopQAPresentation.Controllers.Customer
{
    public class CartItemsController : ODataController
    {
        private readonly ICartItemService _cartItemService;

        public CartItemsController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var items = _cartItemService.GetCartItems();
            return Ok(items);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var item = await _cartItemService.GetCartItemByIdAsync(key);
            if (item == null) return NotFound();
            return Ok(item);
        }

        public async Task<IActionResult> Post([FromBody] CartItem cartItem)
        {
            await _cartItemService.CreateCartItemAsync(cartItem);
            return Created(cartItem);
        }

        [HttpPatch]
        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<CartItem> delta)
        {
            var item = await _cartItemService.GetCartItemByIdAsync(key);
            if (item == null) return NotFound();

            delta.Patch(item);
            var updated = await _cartItemService.UpdateCartItemAsync(item);
            return Updated(updated);
        }




        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            var result = await _cartItemService.DeleteCartItemAsync(key);
            return result ? NoContent() : NotFound();
        }
    }
}