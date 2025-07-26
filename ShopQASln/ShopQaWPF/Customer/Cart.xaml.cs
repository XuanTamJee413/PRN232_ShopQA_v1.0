using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
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
using ShopQaWPF.DTO;
using System.Diagnostics;

namespace ShopQaWPF.Customer
{
    public partial class Cart : Window
    {
        private List<CartVM> carts = new();
        private CartVM currentCart;
        private readonly HttpClient _httpClient;

        public Cart()
        {
            InitializeComponent();

            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7101/") };
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", App.JwtToken);

            LoadCarts();
        }

        public string CurrentCartTitle => currentCart != null ? $"Cart ID: {currentCart.Id} - {currentCart.CreatedAt:dd/MM/yyyy}" : "No Cart";

        private async void LoadCarts()
        {
            try
            {
                int userId = App.CurrentUser?.Id ?? -1;
                var url = $"odata/Cart?$expand=Items($expand=ProductVariant($expand=Product))&$filter=UserId eq {userId}";

                var res = await _httpClient.GetAsync(url);
                var json = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                    throw new Exception("Lỗi tải cart");

                var raw = JsonConvert.DeserializeObject<dynamic>(json);
                var value = raw?.value?.ToString();

                carts = JsonConvert.DeserializeObject<List<CartVM>>(value);

                CartSelector.ItemsSource = carts.Select((c, i) => $"Cart {c.Id} - {c.CreatedAt:dd/MM/yyyy} ({c.Status})").ToList();

                if (carts.Count > 0)
                {
                    CartSelector.SelectedIndex = 0;
                    ShowCart(0);
                }
                else
                {
                    Debug.WriteLine("[DEBUG] Không có cart nào.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG] Exception: {ex.Message}");
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void ShowCart(int index)
        {
            currentCart = carts[index];
            CartItemsListView.ItemsSource = currentCart.Items.Select(i => new CartItemDisplayVM
            {
                Id = i.Id,
                ProductName = i.ProductVariant?.Product?.Name,
                SizeColor = $"Size: {i.ProductVariant?.Size} - Color: {i.ProductVariant?.Color}",
                Price = $"{i.ProductVariant?.Price:N0} ₫",
                Quantity = i.Quantity,
                Total = $"{i.ProductVariant?.Price * i.Quantity:N0} ₫"
            }).ToList();


            double total = currentCart.Items.Sum(i => i.ProductVariant.Price * i.Quantity);
            TotalText.Text = $"{total:N0} ₫";
        }

        private void CartSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CartSelector.SelectedIndex >= 0)
                ShowCart(CartSelector.SelectedIndex);
        }

        private async void UpdateCart_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < currentCart.Items.Count; i++)
            {
                var item = currentCart.Items[i];

                var container = CartItemsListView.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                if (container != null)
                {
                    var txt = FindChild<TextBox>(container);
                    if (int.TryParse(txt?.Text, out int qty) && qty > 0)
                    {
                        var body = new { Quantity = qty };
                        var json = JsonConvert.SerializeObject(body);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        await _httpClient.PatchAsync($"odata/CartItems({item.Id})", content);
                    }
                }
            }

            MessageBox.Show("Đã cập nhật giỏ hàng");
            LoadCarts();
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            var item = (dynamic)btn.DataContext;
            int id = item.Id;

            if (MessageBox.Show("Xoá sản phẩm?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            var res = await _httpClient.DeleteAsync($"odata/CartItems({id})");
            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Đã xoá sản phẩm");
                LoadCarts();
            }
            else
            {
                MessageBox.Show("Xoá thất bại");
            }
        }

        private async void Checkout_Click(object sender, RoutedEventArgs e)
        {
            string phone = PhoneTextBox.Text;
            string address = AddressTextBox.Text;

            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin giao hàng");
                return;
            }

            var body = new { phone, address };
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await _httpClient.PostAsync($"api/Order/checkout?cartId={currentCart.Id}", content);

            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(result);
                MessageBox.Show($"Đặt hàng thành công! Mã đơn: {data.id}");
                LoadCarts();
            }
            else
            {
                var err = await res.Content.ReadAsStringAsync();
                MessageBox.Show("Lỗi: " + err);
            }
        }

        private T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;
                var result = FindChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }

    public class CartVM
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public List<CartItemVM> Items { get; set; }
    }

    public class CartItemVM
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public ProductVariantVM ProductVariant { get; set; }
    }
    public class CartItemDisplayVM
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string SizeColor { get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }
        public string Total { get; set; }
    }

    public class ProductVariantVM
    {
        public int Id { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public ProductVM Product { get; set; }
    }

    public class ProductVM
    {
        public string Name { get; set; }
    }
}
