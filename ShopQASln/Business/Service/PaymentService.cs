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
            // === BƯỚC 1: TẠO MÃ GIAO DỊCH GIỐNG HỆT BẢN DEMO ===
            // Tạo một mã tham chiếu dài, duy nhất, phức tạp, không dùng ID từ DB.
            var vnpTxnRef = $"{orderId}_{DateTime.Now.Ticks}"; // Ví dụ: "1_638868122695967158"

            // Tạo record Payment trong DB để theo dõi
            var newPayment = new Payment
            {
                // Nếu bạn muốn, có thể thêm một cột mới trong bảng Payment để lưu vnpTxnRef này
                // PaymentReferenceCode = vnpTxnRef, 
                OrderId = orderId,
                Method = "VNPAY",
                Amount = amount,
                PaidAt = DateTime.UtcNow,
                Status = "Pending"
            };
            await _paymentRepository.AddPayment(newPayment);
            // Lưu ý: newPayment.Id không còn được dùng để gửi cho VNPAY nữa.

            // === BƯỚC 2: TẠO ORDER INFO GIỐNG HỆT BẢN DEMO ===
            var finalOrderInfo = "Thanh toan don hang:" + orderInfo;

            // === BƯỚC 3: GỌI VNPayService VỚI DỮ LIỆU ĐÃ ĐƯỢC CHUẨN HÓA ===
            var paymentUrl = _vnPayService.CreatePaymentUrl(httpContext, amount, finalOrderInfo, vnpTxnRef);

            if (string.IsNullOrEmpty(paymentUrl))
            {
                Console.WriteLine("VnPayService trả về paymentUrl null, debug ngay!");
                Console.WriteLine($"amount: {amount}, orderInfo: {finalOrderInfo}, paymentId: {vnpTxnRef}");
                // Cập nhật trạng thái thanh toán thất bại nếu không tạo được URL
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