using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Iservices
{
    public interface IPaymentService 
    {
        Task<string?> CreateVnPayPayment(decimal amount, string orderInfo, int orderId, HttpContext httpContext);
        Task<(bool, string, string)> ProcessVnPayCallback(IQueryCollection collections); 
    }
}
