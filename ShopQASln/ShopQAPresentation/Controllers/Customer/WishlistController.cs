using Business.Iservices;
using Microsoft.AspNetCore.Mvc;

namespace ShopQAPresentation.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist(int userId, int productId)
        {
            await _wishlistService.AddToWishlistAsync(userId, productId);
            return Ok(new { message = "Added to wishlist" });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetWishlist(int userId)
        {
            var wishlist = await _wishlistService.GetWishlistAsync(userId);
            return Ok(wishlist);
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromWishlist(int userId, int productId)
        {
            await _wishlistService.RemoveFromWishlistAsync(userId, productId);
            return Ok(new { message = "Removed from wishlist" });
        }
    }
}
