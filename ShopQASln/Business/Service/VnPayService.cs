// File: Business/Services/VnPayService.cs
// PHIÊN BẢN HOÀN CHỈNH CUỐI CÙNG

using Business.DTO;
using Business.Iservices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web; // Thêm using này nếu HttpUtility báo lỗi

namespace Business.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPayConfig _vnPayConfig;

        public VnPayService(IOptions<VnPayConfig> vnPayConfig)
        {
            _vnPayConfig = vnPayConfig.Value;
        }

        // Phương thức này chỉ nhận dữ liệu đã được chuẩn bị và tạo URL
        public string CreatePaymentUrl(HttpContext context, decimal amount, string orderInfo, string paymentId)
        {
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _vnPayConfig.TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());

            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var vnp_CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).ToString("yyyyMMddHHmmss");
            vnpay.AddRequestData("vnp_CreateDate", vnp_CreateDate);

            vnpay.AddRequestData("vnp_CurrCode", "VND");

            var ipAddr = context.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddr) || ipAddr == "::1")
                ipAddr = "127.0.0.1";
            vnpay.AddRequestData("vnp_IpAddr", ipAddr);

            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _vnPayConfig.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", paymentId);

            string paymentUrl = vnpay.CreateRequestUrl(_vnPayConfig.BaseUrl, _vnPayConfig.HashSecret);
            return paymentUrl;
        }

        // Phương thức này xác thực chữ ký trả về
        public (bool, string, string) ProcessPaymentResponse(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_SecureHash = collections["vnp_SecureHash"].FirstOrDefault();

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _vnPayConfig.HashSecret);

            if (!checkSignature)
            {
                return (false, vnp_TxnRef, "Invalid signature");
            }

            if (vnp_ResponseCode != "00")
            {
                return (false, vnp_TxnRef, "Payment failed");
            }

            return (true, vnp_TxnRef, "Payment success");
        }
    }

    // Lớp helper này copy logic TẠO HASH và XÁC THỰC HASH từ bản demo thành công
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(StringComparer.Ordinal);
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(StringComparer.Ordinal);

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _requestData.Add(key, value);
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _responseData.Add(key, value);
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var data = new StringBuilder();
            foreach (var (key, value) in _requestData)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    data.Append(HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value) + "&");
                }
            }

            var queryString = data.ToString().TrimEnd('&');
            var vnp_SecureHash = HmacSHA512(hashSecret, queryString);

            Console.WriteLine($"[VNPay] RawData for HMAC (Encoded): {queryString}");
            return $"{baseUrl}?{queryString}&vnp_SecureHash={vnp_SecureHash}";
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var data = new StringBuilder();
            foreach (var (key, value) in _responseData.Where(kv => kv.Key != "vnp_SecureHash"))
            {
                if (!string.IsNullOrEmpty(value))
                {
                    data.Append(HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value) + "&");
                }
            }

            var signData = data.ToString().TrimEnd('&');
            var myChecksum = HmacSHA512(secretKey, signData);

            Console.WriteLine($"[VNPay] Validate signData (re-encoded): {signData}");
            Console.WriteLine($"[VNPay] computedHash={myChecksum}, receivedHash={inputHash}");
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var messageBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(messageBytes);
                foreach (var b in hashValue)
                {
                    hash.Append(b.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }
}