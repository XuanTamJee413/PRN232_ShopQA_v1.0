using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace DataAccess.IRepositories
{
    public interface IReviewRepository
    {
        IEnumerable<Review> GetAllReviews();
        void AddReview(Review review);
        Review? GetReviewById(int id);
        void DeleteReview(Review review);
        Review? GetById(int id);
        void Update(Review review);
        void Save();
        void Delete(int reviewId);

        IEnumerable<Review> GetAll();
    }
}
