using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Business.Iservices
{
    public interface IVnPayService 
    {
        string CreatePaymentUrl(HttpContext context, decimal amount, string orderInfo, string paymentId);
        (bool, string, string) ProcessPaymentResponse(IQueryCollection collections); 

    }
}
