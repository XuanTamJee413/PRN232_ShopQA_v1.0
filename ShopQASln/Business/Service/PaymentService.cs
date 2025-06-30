using Business.DTO; 
using Business.Iservices; 
using DataAccess.IRepositories;
using Domain.Models; 
using Microsoft.AspNetCore.Http; 
using System; 
using System.Threading.Tasks;

namespace Business.Services
{
    public class PaymentService : IPaymentService 
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IVnPayService _vnPayService;

        public PaymentService(IPaymentRepository paymentRepository, IVnPayService vnPayService)
        {
            _paymentRepository = paymentRepository;
            _vnPayService = vnPayService;
        }

        public async Task<string?> CreateVnPayPayment(decimal amount, string orderInfo, int orderId, HttpContext httpContext)
        {
            
            var vnpTxnRef = $"{orderId}_{DateTime.Now.Ticks}"; 

           
            var newPayment = new Payment
            {
               
                OrderId = orderId,
                Method = "VNPAY",
                Amount = amount,
                PaidAt = DateTime.UtcNow,
                Status = "Pending"
            };
            await _paymentRepository.AddPayment(newPayment);
           
            var finalOrderInfo = "Thanh toan don hang:" + orderInfo;

           
            var paymentUrl = _vnPayService.CreatePaymentUrl(httpContext, amount, finalOrderInfo, vnpTxnRef);

            if (string.IsNullOrEmpty(paymentUrl))
            {
                Console.WriteLine("VnPayService trả về paymentUrl null, debug ngay!");
                Console.WriteLine($"amount: {amount}, orderInfo: {finalOrderInfo}, paymentId: {vnpTxnRef}");
                
                var paymentToUpdate = await _paymentRepository.GetPaymentById(newPayment.Id);
                if (paymentToUpdate != null)
                {
                    paymentToUpdate.Status = "Failed";
                    await _paymentRepository.UpdatePayment(paymentToUpdate);
                }
                return null;
            }

            return paymentUrl;
        }

        public async Task<(bool, string, string)> ProcessVnPayCallback(IQueryCollection collections) 
        {
            var response = _vnPayService.ProcessPaymentResponse(collections);

            var vnpTxnRef = response.Item2;
            int paymentId;

            if (!int.TryParse(vnpTxnRef, out paymentId))
            {
                return (false, vnpTxnRef, "Mã giao dịch từ VNPAY không hợp lệ.");
            }

            var payment = await _paymentRepository.GetPaymentById(paymentId);

            if (payment == null)
            {
                return (false, vnpTxnRef, "Không tìm thấy giao dịch trong hệ thống.");
            }

            if (response.Item1) 
            {
                payment.Status = "Completed";
                payment.PaidAt = DateTime.UtcNow;
            }
            else 
            {
                payment.Status = "Failed";
            }

            await _paymentRepository.UpdatePayment(payment);

            return response; 
        }
    }
}