// File: Business/Services/PaymentService.cs
using DataAccess.IRepositories;
using Business.Iservices;
using Domain.Models; // Đảm bảo bạn đã using model Payment
using Microsoft.AspNetCore.Http; // Đảm bảo bạn đã using
using System;
using System.Threading.Tasks;

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
        // 1. Tạo mã giao dịch dài và phức tạp, giống bản demo
        var vnpTxnRef = $"{orderId}_{DateTime.Now.Ticks}";

        // 2. Thêm tiền tố vào thông tin đơn hàng, giống bản demo
        var finalOrderInfo = "Thanh toan don hang:" + orderInfo;

        // 3. Lưu vào DB để theo dõi
        var newPayment = new Payment
        {
            OrderId = orderId,
            Method = "VNPAY",
            Amount = amount,
            PaidAt = DateTime.UtcNow,
            Status = "Pending"
            // Nếu có cột lưu mã tham chiếu, bạn có thể gán: PaymentReferenceCode = vnpTxnRef
        };
        await _paymentRepository.AddPayment(newPayment);

        // 4. Gọi VnPayService với dữ liệu đã được chuẩn hóa
        var paymentUrl = _vnPayService.CreatePaymentUrl(httpContext, amount, finalOrderInfo, vnpTxnRef);

        if (string.IsNullOrEmpty(paymentUrl))
        {
            newPayment.Status = "Failed";
            await _paymentRepository.UpdatePayment(newPayment);
            return null;
        }

        return paymentUrl;
    }

    // Phương thức ProcessVnPayCallback giữ nguyên như cũ của bạn
    public async Task<(bool, string, string)> ProcessVnPayCallback(IQueryCollection collections)
    {
        var response = _vnPayService.ProcessPaymentResponse(collections);
        //... code xử lý cập nhật DB của bạn ở đây
        //...
        return response;
    }
}