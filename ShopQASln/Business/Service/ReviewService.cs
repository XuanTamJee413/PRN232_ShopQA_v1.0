using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;
using Business.Iservices;
using DataAccess.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public IEnumerable<ReviewDto> GetAllReviews()
        {
            var reviews = _reviewRepository.GetAllReviews();

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                ProductId = r.ProductId,
                Username = r.User?.Username ?? "Unknown",
                ProductName = r.Product?.Name ?? "Unknown"
            }).ToList();
        }
        public void AddReview(ReviewCreateDto reviewDto)
        {
            if (reviewDto == null || reviewDto.UserId == 0 || reviewDto.ProductId == 0 || reviewDto.Rating < 1 || reviewDto.Rating > 5)
                throw new ArgumentException("Invalid review data");

            var review = new Review
            {
                UserId = reviewDto.UserId,
                ProductId = reviewDto.ProductId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _reviewRepository.AddReview(review);
        }
        public void DeleteReview(int id)
        {
            var review = _reviewRepository.GetReviewById(id);
            if (review == null)
                throw new ArgumentException("Review not found");

            _reviewRepository.DeleteReview(review);
        }
        public void UpdateReview(ReviewUpdateDto reviewDto, int userId)
        {
            var existing = _reviewRepository.GetById(reviewDto.Id);
            if (existing == null)
                throw new ArgumentException("Không tìm thấy đánh giá.");

            if (existing.UserId != userId)
                throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa đánh giá này.");

            existing.Rating = reviewDto.Rating;
            existing.Comment = reviewDto.Comment;
            existing.CreatedAt = DateTime.UtcNow;

            _reviewRepository.Update(existing);
            _reviewRepository.Save();
        }
        public IEnumerable<ReviewSingleDto> GetReviewsByProductId(int productId)
        {
            var reviews = _reviewRepository.GetAll() // hoặc GetQueryable()
                .Where(r => r.ProductId == productId)
                .Select(r => new ReviewSingleDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    ProductId = r.ProductId,
                    Username = r.User != null ? r.User.Username : "N/A",
                    UserNameId = r.UserId != 0 ? r.UserId : 0,
                    ProductName = r.Product != null ? r.Product.Name : "N/A"
                    
                })
                .ToList();

            return reviews;
        }
        public bool DeleteReviewWithUser(int reviewId, int userId)
        {
            var review = _reviewRepository.GetById(reviewId);

            if (review == null)
                throw new ArgumentException("Không tìm thấy đánh giá.");

            if (review.UserId != userId)
                return false; // Không phải người dùng sở hữu review

            _reviewRepository.Delete(reviewId);
            return true;
        }



    }
}
