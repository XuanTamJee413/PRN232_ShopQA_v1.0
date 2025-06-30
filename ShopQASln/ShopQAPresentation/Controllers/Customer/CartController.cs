using Business.DTO;
using Business.Iservices;
using Domain.Models;
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
        public async Task<IActionResult> GetCarts(int userId)
        {
            var carts = await _cartService.GetCartsByUserIdAsync(userId);

            if (carts == null || carts.Count == 0)
                return NotFound();

            // Map từ Cart entity sang CartDTO
            var cartDtos = carts.Select(c => new CartDTO
            {
                Id = c.Id,
                UserId = c.UserId,
                CreatedAt = c.CreatedAt,
                Status = c.Status,
                Items = c.Items.Select(i => new CartItemDTO
                {
                    Id = i.Id,
                    CartId = i.CartId,
                    ProductVariantId = i.ProductVariantId,
                    Quantity = i.Quantity,
                    ProductVariant = i.ProductVariant == null ? null : new ProductVariantDTO
                    {
                        Id = i.ProductVariant.Id,
                        Price = i.ProductVariant.Price,
                        Color = i.ProductVariant.Color,
                        Size = i.ProductVariant.Size,
                        Stock = i.ProductVariant.Stock,
                        ImageUrl = i.ProductVariant.ImageUrl,
                        ProductId = i.ProductVariant.ProductId
                    }
                }).ToList()
            }).ToList();

            return Ok(cartDtos);
        }

        // POST: api/cart
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItem item)
        {
            var result = await _cartService.AddItemToCartAsync(item);
            if (!result)
                return BadRequest();

            return Ok();
        }

        // DELETE: api/cart/{cartId}/item/{itemId}
        [HttpDelete("{cartId}/item/{itemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId, int itemId)
        {
            var result = await _cartService.RemoveItemFromCartAsync(cartId, itemId);
            if (!result)
                return BadRequest();

            return Ok();
        }
    }
}
