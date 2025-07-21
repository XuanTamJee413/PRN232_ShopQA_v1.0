using Business.Iservices; 
using Business.DTO;     
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ShopQAPresentation.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]             
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

       
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Tạo yêu cầu thanh toán VNPAY.
        /// </summary>
        /// <param name="model">Thông tin yêu cầu thanh toán (Số tiền, Thông tin đơn hàng, ID đơn hàng).</param>
        /// <returns>URL thanh toán VNPAY hoặc thông báo lỗi.</returns>
        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestModel model)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            try
            {
                
                var paymentUrl = await _paymentService.CreateVnPayPayment(
                    model.Amount,
                    model.OrderInfo,
                    model.OrderId,
                    HttpContext 
                );

                
                if (string.IsNullOrEmpty(paymentUrl))
                {
                    return StatusCode(500, new { Message = "Không thể tạo URL thanh toán VNPAY. Vui lòng thử lại." });
                }

               
                Console.WriteLine($"paymentUrl trả về: {paymentUrl}");
                return Ok(new { paymentUrl = paymentUrl });
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Lỗi khi tạo yêu cầu thanh toán VNPAY: {ex.Message}");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi nội bộ khi xử lý yêu cầu thanh toán. Vui lòng thử lại sau.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint xử lý callback từ VNPAY sau khi giao dịch hoàn tất.
        /// VNPAY sẽ gọi GET request đến URL này.
        /// </summary>
        /// <returns>Thông báo kết quả thanh toán.</returns>
        [HttpGet("callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
               
                var result = await _paymentService.ProcessVnPayCallback(Request.Query);

                if (result.Item1) // Item1 là cờ success (true/false)
                {
                    
                    return Content($"<h1>Thanh toán thành công!</h1><p>Mã giao dịch của bạn: {result.Item2}</p><p>Cảm ơn bạn đã sử dụng dịch vụ.</p>", "text/html", System.Text.Encoding.UTF8);
                }
                else 
                {
                    
                    return Content($"<h1>Thanh toán thất bại</h1><p>Mã giao dịch: {result.Item2}</p><p>Lý do: {result.Item3}</p>", "text/html", System.Text.Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Lỗi khi xử lý callback thanh toán VNPAY: {ex.Message}");
                return Content($"<h1>Lỗi xử lý thanh toán</h1><p>Đã xảy ra lỗi khi xử lý kết quả giao dịch: {ex.Message}</p>", "text/html", System.Text.Encoding.UTF8);
            }
        }


       
    }
}