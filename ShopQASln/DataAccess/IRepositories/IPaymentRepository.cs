using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepositories
{
    public interface IPaymentRepository 
    {
        Task AddPayment(Payment payment);
        Task<Payment?> GetPaymentById(int id);
        Task UpdatePayment(Payment payment);
    }
}