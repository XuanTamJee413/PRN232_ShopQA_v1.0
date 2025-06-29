using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;
using Domain.Models;

namespace Business.Iservices
{
    public interface IReviewService
    {
        IEnumerable<ReviewDto> GetAllReviews();
        void AddReview(ReviewCreateDto reviewDto);
        void DeleteReview(int id);
        void UpdateReview(ReviewUpdateDto reviewDto, int userId);

        IEnumerable<ReviewSingleDto> GetReviewsByProductId(int productId);
        bool DeleteReviewWithUser(int reviewId, int userId);

    }
}
