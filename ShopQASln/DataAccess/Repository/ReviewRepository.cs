using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Context;
using DataAccess.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
    public class ReviewRepository :IReviewRepository
    {
        private readonly ShopQADbContext _context;

        public ReviewRepository(ShopQADbContext context)
        {
            _context = context;
        }

        public IEnumerable<Review> GetAllReviews()
        {
            return _context.Review
    .Include(r => r.User)
    .Include(r => r.Product)
    .ToList();

        }
        public void AddReview(Review review)
        {
            _context.Review.Add(review);
            _context.SaveChanges();
        }
        public Review? GetReviewById(int id)
        {
            return _context.Review.FirstOrDefault(r => r.Id == id);
        }

        public void DeleteReview(Review review)
        {
            _context.Review.Remove(review);
            _context.SaveChanges();
        }
        public Review? GetById(int id)
        {
            return _context.Review.FirstOrDefault(r => r.Id == id);
        }

        public void Update(Review review)
        {
            _context.Review.Update(review);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public IEnumerable<Review> GetAll()
        {
            return _context.Review
                .Include(r => r.User)
                .Include(r => r.Product)
                .ToList();
        }
        public void Delete(int reviewId)
        {
            var review = _context.Review.FirstOrDefault(r => r.Id == reviewId);
            if (review != null)
            {
                _context.Review.Remove(review);
                _context.SaveChanges();
            }
        }
    }
}
