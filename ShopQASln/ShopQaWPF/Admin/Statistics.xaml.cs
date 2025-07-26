using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ShopQaWPF.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using ShopQaWPF.DTO;

namespace ShopQaWPF.Admin
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        public Statistics()
        {
            InitializeComponent();
        }

        public async void LoadStatistics()
        {
            string token = "YOUR_ADMIN_JWT_TOKEN_HERE"; // Thay bằng token thật
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7101/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // Tổng số đơn hàng
                var orderCountResponse = await client.GetAsync("api/Order/total-count");
                var orderCount = await orderCountResponse.Content.ReadAsStringAsync();
                txtTotalOrders.Text = orderCount;
                // Tổng doanh thu
                var revenueResponse = await client.GetAsync("api/Order/total-revenue");
                var revenue = await revenueResponse.Content.ReadAsStringAsync();
                txtTotalRevenue.Text = String.Format("{0:N0}₫", decimal.Parse(revenue));
                // Sản phẩm bán chạy
                var salesResponse = await client.GetAsync("api/Order/product-variant-sales");
                var salesJson = await salesResponse.Content.ReadAsStringAsync();
                var salesList = JsonConvert.DeserializeObject<List<ProductVariantSalesDto>>(salesJson) ?? new List<ProductVariantSalesDto>();
                lvBestSelling.ItemsSource = salesList.Take(3);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
        }
    }
}
