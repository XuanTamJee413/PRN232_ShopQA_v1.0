using Business.DTO; // Để sử dụng VnPayConfig (nếu cần), PaymentRequestModel
using Business.Iservices; // Để sử dụng IVnPayService và IPaymentService
using DataAccess.IRepositories; // Để sử dụng IPaymentRepository
using Domain.Models; // Để sử dụng Payment model
using Microsoft.AspNetCore.Http; // Để sử dụng HttpContext, IQueryCollection
using System; // Cho DateTime, Tuple
using System.Threading.Tasks;

namespace Business.Services
{
    public class PaymentService : IPaymentService // Đảm bảo là public
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
            var newPayment = new Payment
            {
                OrderId = orderId,
                Method = "VNPAY",
                Amount = amount,
                PaidAt = DateTime.UtcNow,
                Status = "Pending"
            };

            await _paymentRepository.AddPayment(newPayment);

            var vnpTxnRef = newPayment.Id.ToString();

            var paymentUrl = _vnPayService.CreatePaymentUrl(httpContext, amount, orderInfo, vnpTxnRef);

            if (string.IsNullOrEmpty(paymentUrl))
            {
                newPayment.Status = "Failed";
                await _paymentRepository.UpdatePayment(newPayment);
                return null;
            }

            return paymentUrl;
        }

        public async Task<(bool, string, string)> ProcessVnPayCallback(IQueryCollection collections) // Đảm bảo là C# Value Tuple
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

            if (response.Item1) // Giao dịch thành công (true)
            {
                payment.Status = "Completed";
                payment.PaidAt = DateTime.UtcNow;
            }
            else // Giao dịch thất bại (false)
            {
                payment.Status = "Failed";
            }

            await _paymentRepository.UpdatePayment(payment);

            return response; // Trả về C# Value Tuple
        }
    }
}