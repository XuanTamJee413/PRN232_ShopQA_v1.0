using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Context;
using DataAccess.IRepositories;
using Domain.Models;

namespace DataAccess.Repository
{
    public class BrandRepository :IBrandRepository
    {
        private readonly ShopQADbContext _context;

        public BrandRepository(ShopQADbContext context)
        {
            _context = context;
        }

       
        public Brand? GetById(int id)
        {
            return _context.Brands.FirstOrDefault(b => b.Id == id);
        }
    }
}
