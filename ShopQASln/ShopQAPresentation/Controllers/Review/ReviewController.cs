using Business.DTO;
using Business.Iservices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShopQAPresentation.Controllers.Review
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Get()
        {
            var reviews = _reviewService.GetAllReviews();
            return Ok(reviews);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] ReviewCreateDto reviewDto)
        {
            try
            {
                _reviewService.AddReview(reviewDto);
                return Ok(new { message = "Review added successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        //[Authorize]
        public IActionResult Delete(int id)
        {
            try
            {
                _reviewService.DeleteReview(id);
                return Ok(new { message = "Review deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}/user/{userId}")]
        
        public IActionResult Delete(int id, int userId)
        {
            try
            {
                bool isDeleted = _reviewService.DeleteReviewWithUser(id, userId);
                if (!isDeleted)
                    return Forbid("Bạn không có quyền xoá đánh giá này."); // hoặc return Unauthorized()

                return Ok(new { message = "Review deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Customer")]

        public IActionResult Put(int id, [FromQuery] int userId, [FromBody] ReviewUpdateDto reviewDto)
        {
            if (id != reviewDto.Id)
                return BadRequest("ID không khớp.");

            try
            {
                _reviewService.UpdateReview(reviewDto, userId);
                return Ok(new { message = "Cập nhật đánh giá thành công." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("product/{productId}")]
        //[Authorize]
        public IActionResult GetReviewsByProductId(int productId)
        {
            var reviews = _reviewService.GetReviewsByProductId(productId);
            return Ok(reviews);
        }


    }
}



