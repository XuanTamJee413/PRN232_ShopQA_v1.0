using Domain.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShopQAPresentation.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/cart/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound();
            }
            return Ok(cart);
        }

        // POST: api/cart
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItem item)
        {
            var result = await _cartService.AddItemToCartAsync(item);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

        // DELETE: api/cart/{cartId}/item/{itemId}
        [HttpDelete("{cartId}/item/{itemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId, int itemId)
        {
            var result = await _cartService.RemoveItemFromCartAsync(cartId, itemId);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
