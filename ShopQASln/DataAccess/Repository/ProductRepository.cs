using DataAccess.Context;
using DataAccess.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopQADbContext _context;

        public ProductRepository(ShopQADbContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAll() => _context.Products.Include(p => p.Category).Include(p => p.Brand).ToList();

        //tamnx get all queryable
        public IQueryable<Product> GetAllQueryable()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Variants);
        }
        public Product? GetById(int id) => _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.Id == id);

        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public Product? GetProductById(int id)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Inventory) 
                .FirstOrDefault(p => p.Id == id);
        }

    }
}
