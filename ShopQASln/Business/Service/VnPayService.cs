// File: Business/Services/VnPayService.cs

using Business.DTO;
using Business.Iservices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Business.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPayConfig _vnPayConfig;

        public VnPayService(IOptions<VnPayConfig> vnPayConfig)
        {
            _vnPayConfig = vnPayConfig.Value;
        }

        /// <summary>
        /// Tạo URL thanh toán VNPay
        /// </summary>
        public string CreatePaymentUrl(HttpContext context, decimal amount, string orderInfo, string paymentId)
        {


            // === BẮT ĐẦU PHẦN CODE TEST ===
            // Thay thế bằng TmnCode và HashSecret MỚI NHẤT của bạn
            var hardcoded_TmnCode = "6Y29YWLW";
            var hardcoded_HashSecret = "VME3OX05VHGNNBWQ3W78D7WZELJO4EK2";

            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");

            vnpay.AddRequestData("vnp_TmnCode", hardcoded_TmnCode);


            vnpay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());


            //vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var vnp_CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).ToString("yyyyMMddHHmmss");
            vnpay.AddRequestData("vnp_CreateDate", vnp_CreateDate);

            vnpay.AddRequestData("vnp_CurrCode", "VND");

            // IP
            var ipAddr = context.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddr) || ipAddr == "::1")
                ipAddr = "127.0.0.1";
            vnpay.AddRequestData("vnp_IpAddr", ipAddr);

            
            vnpay.AddRequestData("vnp_Locale", "vn");


            vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _vnPayConfig.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", paymentId);

            var paymentUrl = vnpay.CreateRequestUrl(_vnPayConfig.BaseUrl, hardcoded_HashSecret);

            Console.WriteLine($"[VNPay] Payment URL: {paymentUrl}");

            return paymentUrl;
        }

        /// <summary>
        /// Xử lý kết quả thanh toán VNPay callback
        /// </summary>
        public (bool, string, string) ProcessPaymentResponse(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();

            foreach (var key in collections.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, collections[key]);
                }
            }

            var vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_SecureHash = collections["vnp_SecureHash"];

            var hardcoded_HashSecret = "VME3OX05VHGNNBWQ3W78D7WZELJO4EK2";
            var isValid = vnpay.ValidateSignature(vnp_SecureHash, hardcoded_HashSecret);

            if (isValid)
            {
                if (vnp_ResponseCode == "00")
                {
                    return (true, vnp_TxnRef, "Giao dịch thành công.");
                }
                else
                {
                    return (false, vnp_TxnRef, $"Giao dịch thất bại. Mã lỗi VNPAY: {vnp_ResponseCode}");
                }
            }
            else
            {
                return (false, vnp_TxnRef, "Sai chữ ký (HMAC) - dữ liệu có thể đã bị thay đổi.");
            }
        }

        // ==================================================================
        // VNPayLibrary - class nội bộ hỗ trợ
        // ==================================================================
        public class VnPayLibrary
        {
            private readonly SortedList<string, string> _requestData = new(new VnPayCompare());
            private readonly SortedList<string, string> _responseData = new(new VnPayCompare());

            public void AddRequestData(string key, string value)
            {
                if (!string.IsNullOrEmpty(value))
                    _requestData[key] = value;  // raw
            }

            public void AddResponseData(string key, string value)
            {
                if (!string.IsNullOrEmpty(value))
                    _responseData[key] = value;
            }

            public string GetResponseData(string key)
            {
                return _responseData.ContainsKey(key) ? _responseData[key] : string.Empty;
            }

            /// <summary>
            /// Tạo URL redirect kèm SecureHash
            /// </summary>
            public string CreateRequestUrl(string baseUrl, string hashSecret)
            {
                // Sửa lại hoàn toàn theo logic của bản demo đã chạy thành công
                var data = new StringBuilder();

                // Duyệt qua SortedList đã được sắp xếp theo key
                foreach (var (key, value) in _requestData)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        // Mã hóa cả key và value theo chuẩn của VNPAY
                        data.Append(HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value) + "&");
                    }
                }

                // Cắt ký tự '&' cuối cùng
                string queryString = data.ToString();
                if (queryString.Length > 0)
                {
                    queryString = queryString.Remove(queryString.Length - 1, 1);
                }

                // Dữ liệu để tạo chữ ký chính là chuỗi đã mã hóa này
                var vnp_SecureHash = HmacSHA512(hashSecret, queryString);

                // Tạo URL cuối cùng
                var finalUrl = $"{baseUrl}?{queryString}&vnp_SecureHash={vnp_SecureHash}";

                // Log lại để kiểm tra (nếu cần)
                Console.WriteLine($"[VNPay] RawData for HMAC (Encoded): {queryString}");
                Console.WriteLine($"[VNPay] Final payment URL: {finalUrl}");

                return finalUrl;
            }

            /// <summary>
            /// Validate chữ ký trả về
            /// </summary>
            /// <summary>
            /// Validate chữ ký trả về
            /// </summary>
            public bool ValidateSignature(string receivedHash, string hashSecret)
            {
                var data = new StringBuilder();

                // Phải duyệt qua _responseData đã được sắp xếp để tạo chuỗi dữ liệu
                foreach (var (key, value) in _responseData)
                {
                    // Bỏ qua tham số vnp_SecureHash và các tham số trống
                    if (!string.IsNullOrEmpty(value) && key != "vnp_SecureHash")
                    {
                        // Mã hóa lại key và value để tạo chuỗi giống hệt VNPAY đã làm
                        data.Append(HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value) + "&");
                    }
                }

                // Cắt ký tự '&' cuối cùng
                string signData = data.ToString();
                if (signData.Length > 0)
                {
                    signData = signData.Remove(signData.Length - 1, 1);
                }

                // Tạo hash từ chuỗi đã mã hóa lại
                var myHash = HmacSHA512(hashSecret, signData);

                // Log lại để debug
                Console.WriteLine($"[VNPay] Validate signData (re-encoded): {signData}");
                Console.WriteLine($"[VNPay] computedHash={myHash}, receivedHash={receivedHash}");

                return myHash.Equals(receivedHash, StringComparison.InvariantCultureIgnoreCase);
            }

            /// <summary>
            /// build query string
            /// </summary>
            private string BuildQueryString(SortedList<string, string> data, bool encode)
            {
                var sb = new StringBuilder();
                foreach (var item in data)
                {
                    var value = encode ? HttpUtility.UrlEncode(item.Value, Encoding.UTF8) : item.Value;
                    sb.Append(item.Key).Append('=').Append(value).Append('&');
                }
                return sb.ToString().TrimEnd('&');
            }

            /// <summary>
            /// Tính Hmac SHA512
            /// </summary>
            private string HmacSHA512(string key, string inputData)
            {
                var hash = new StringBuilder();
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var inputBytes = Encoding.UTF8.GetBytes(inputData);

                using var hmac = new HMACSHA512(keyBytes);
                var hashBytes = hmac.ComputeHash(inputBytes);

                foreach (var b in hashBytes)
                    hash.Append(b.ToString("x2"));

                return hash.ToString();
            }

            /// <summary>
            /// Custom comparator: key alphabet
            /// </summary>
            private class VnPayCompare : IComparer<string>
            {
                public int Compare(string? x, string? y)
                {
                    return string.CompareOrdinal(x, y);
                }
            }
        }
    }
}
