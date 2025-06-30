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

            var paymentUrl = vnpay.CreateRequestUrl(_vnPayConfig.BaseUrl, _vnPayConfig.HashSecret); 

            Console.WriteLine($"[VNPay] Payment URL: {paymentUrl}");
            return paymentUrl;
        }

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

            var isValid = vnpay.ValidateSignature(vnp_SecureHash, _vnPayConfig.HashSecret); 

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
        // VNPayLibrary - class nội bộ hỗ trợ - PHIÊN BẢN CHUẨN
        // ==================================================================
        public class VnPayLibrary
        {
            private readonly SortedList<string, string> _requestData = new(new VnPayCompare());
            private readonly SortedList<string, string> _responseData = new(new VnPayCompare());

            public void AddRequestData(string key, string value)
            {
                if (!string.IsNullOrEmpty(value))
                    _requestData[key] = value;
            }

            public void AddResponseData(string key, string value)
            {
                if (!string.IsNullOrEmpty(value))
                    _responseData[key] = value;
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

                string queryString = data.ToString();
                if (queryString.Length > 0)
                {
                    queryString = queryString.Remove(queryString.Length - 1, 1);
                }

                var vnp_SecureHash = HmacSHA512(hashSecret, queryString);
                var finalUrl = $"{baseUrl}?{queryString}&vnp_SecureHash={vnp_SecureHash}";

                Console.WriteLine($"[VNPay] RawData for HMAC (Encoded): {queryString}");
                return finalUrl;
            }

            public bool ValidateSignature(string receivedHash, string hashSecret)
            {
                var data = new StringBuilder();
                foreach (var (key, value) in _responseData)
                {
                    if (!string.IsNullOrEmpty(value) && key != "vnp_SecureHash")
                    {
                        data.Append(HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value) + "&");
                    }
                }

                string signData = data.ToString();
                if (signData.Length > 0)
                {
                    signData = signData.Remove(signData.Length - 1, 1);
                }

                var myHash = HmacSHA512(hashSecret, signData);

                Console.WriteLine($"[VNPay] Validate signData (re-encoded): {signData}");
                Console.WriteLine($"[VNPay] computedHash={myHash}, receivedHash={receivedHash}");

                return myHash.Equals(receivedHash, StringComparison.InvariantCultureIgnoreCase);
            }

            private string HmacSHA512(string key, string inputData)
            {
                var hash = new StringBuilder();
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var inputBytes = Encoding.UTF8.GetBytes(inputData);
                using (var hmac = new HMACSHA512(keyBytes))
                {
                    var hashBytes = hmac.ComputeHash(inputBytes);
                    foreach (var b in hashBytes)
                    {
                        hash.Append(b.ToString("x2"));
                    }
                }
                return hash.ToString();
            }

            private class VnPayCompare : IComparer<string>
            {
                public int Compare(string? x, string? y) => string.CompareOrdinal(x, y);
            }
        }
    }
}