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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ShopQADbContext _context;

        public PaymentRepository(ShopQADbContext context)
        {
            _context = context;
        }

        public async Task AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetPaymentById(int id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task UpdatePayment(Payment payment)
        {
            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
