// File: Business/Services/VnPayService.cs
using Business.DTO;
using Business.Iservices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options; // Để inject IOptions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Business.Services
{
    public class VnPayService : IVnPayService // Đảm bảo là public
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
            vnpay.AddRequestData("vnp_Amount", (amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", context.Connection.RemoteIpAddress?.ToString());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _vnPayConfig.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", paymentId);

            string paymentUrl = vnpay.CreateRequestUrl(_vnPayConfig.BaseUrl, _vnPayConfig.HashSecret);
            return paymentUrl;
        }

        public (bool, string, string) ProcessPaymentResponse(IQueryCollection collections) // Đảm bảo là C# Value Tuple
        {
            var vnpay = new VnPayLibrary();

            foreach (string key in collections.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, collections[key]);
                }
            }

            string vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionNo = vnpay.GetResponseData("vnp_TransactionNo");
            string vnp_SecureHash = collections["vnp_SecureHash"];

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _vnPayConfig.HashSecret);

            if (checkSignature)
            {
                if (vnp_ResponseCode == "00" && vnp_TransactionNo != "00")
                {
                    return (true, vnp_TxnRef, "Giao dịch thành công."); // Trả về C# Value Tuple
                }
                else
                {
                    string message = $"Giao dịch thất bại. Mã lỗi VNPAY: {vnp_ResponseCode}";
                    return (false, vnp_TxnRef, message); // Trả về C# Value Tuple
                }
            }
            else
            {
                return (false, vnp_TxnRef, "Sai chữ ký HMAC."); // Trả về C# Value Tuple
            }
        }

        // --- VnPayLibrary và VnPayCompare Classes ---
        // Các lớp này cần được định nghĩa hoặc có thể được đặt trong một tệp riêng biệt
        // nếu bạn muốn chúng có quyền truy cập rộng hơn hoặc cấu trúc tốt hơn.
        // Tôi giữ chúng ở đây như bạn đã cung cấp.

        public class VnPayLibrary
        {
            private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
            private SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

            public void AddRequestData(string key, string value)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _requestData.Add(key, value);
                }
            }

            public void AddResponseData(string key, string value)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _responseData.Add(key, value);
                }
            }

            public string GetResponseData(string key)
            {
                return _responseData.ContainsKey(key) ? _responseData[key] : string.Empty;
            }

            public string CreateRequestUrl(string baseUrl, string hashSecret)
            {
                StringBuilder data = new StringBuilder();
                foreach (KeyValuePair<string, string> item in _requestData)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        data.Append(item.Key + "=" + HttpUtility.UrlEncode(item.Value, Encoding.UTF8) + "&");
                    }
                }
                string queryString = data.ToString();
                if (queryString.EndsWith("&"))
                {
                    queryString = queryString.Substring(0, queryString.Length - 1);
                }

                string signData = BuildQueryString(_requestData);
                string vnp_SecureHash = HmacSHA512(hashSecret, signData);

                return baseUrl + "?" + queryString + "&vnp_SecureHash=" + vnp_SecureHash;
            }

            public bool ValidateSignature(string receivedHash, string hashSecret)
            {
                string signData = BuildQueryString(_responseData);
                string myHash = HmacSHA512(hashSecret, signData);
                return myHash.Equals(receivedHash, StringComparison.InvariantCultureIgnoreCase);
            }

            private string BuildQueryString(SortedList<string, string> data)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> item in data)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        sb.Append(item.Key).Append("=").Append(item.Value).Append("&");
                    }
                }
                string result = sb.ToString();
                return result.EndsWith("&") ? result.Substring(0, result.Length - 1) : result;
            }

            private string HmacSHA512(string key, string inputData)
            {
                var hash = new StringBuilder();
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
                using (var hmac = new HMACSHA512(keyBytes)) // Sử dụng System.Security.Cryptography.HMACSHA512
                {
                    byte[] hashBytes = hmac.ComputeHash(inputBytes);
                    foreach (byte b in hashBytes)
                    {
                        hash.Append(b.ToString("x2"));
                    }
                }
                return hash.ToString();
            }

            private class VnPayCompare : IComparer<string>
            {
                public int Compare(string x, string y)
                {
                    if (x == y) return 0;
                    if (x == null) return -1;
                    if (y == null) return 1;
                    var vnp_order = new string[] { "vnp_Amount", "vnp_BankCode", "vnp_BankTranNo", "vnp_CardType", "vnp_OrderInfo", "vnp_PayDate", "vnp_ResponseCode", "vnp_TmnCode", "vnp_TransactionNo", "vnp_TxnRef", "vnp_Version", "vnp_Command", "vnp_CurrCode", "vnp_IpAddr", "vnp_Locale", "vnp_OrderType", "vnp_ReturnUrl", "vnp_ExpireDate", "vnp_CreateDate" };
                    int ix = Array.IndexOf(vnp_order, x);
                    int iy = Array.IndexOf(vnp_order, y);
                    if (ix != -1 && iy != -1)
                    {
                        return ix.CompareTo(iy);
                    }
                    else if (ix != -1)
                    {
                        return -1;
                    }
                    else if (iy != -1)
                    {
                        return 1;
                    }
                    return string.Compare(x, y, StringComparison.Ordinal);
                }
            }
        }
    }
}