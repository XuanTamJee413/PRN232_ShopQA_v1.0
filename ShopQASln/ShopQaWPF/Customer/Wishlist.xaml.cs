
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
using ShopQaWPF.DTO;
// using Business.DTO;

namespace ShopQaWPF.Customer
{
    /// <summary>
    /// Interaction logic for Wishlist.xaml
    /// </summary>
    public partial class Wishlist : Window
    {
        public Wishlist()
        {
            InitializeComponent();
            Loaded += Wishlist_Loaded;
        }

        private async void Wishlist_Loaded(object sender, RoutedEventArgs e)
        {
            if (ShopQaWPF.App.CurrentUser == null) return;
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new System.Uri("https://localhost:7101/");
                if (!string.IsNullOrEmpty(ShopQaWPF.App.JwtToken))
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ShopQaWPF.App.JwtToken);
                try
                {
                    var response = await client.GetAsync($"api/Wishlist/list?userId={ShopQaWPF.App.CurrentUser.Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var wishlist = System.Text.Json.JsonSerializer.Deserialize<ShopQaWPF.DTO.WishlistDTO>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        var items = wishlist?.Items?.Select(i => new WishlistItemDto
                        {
                            ProductId = i.ProductId,
                            Name = i.ProductName ?? "",
                            Description = i.ProductDescription ?? "",
                            ImageUrl = i.ProductImageUrl ?? "",
                            AddedAt = i.AddedAt.ToString("dd/MM/yyyy")
                        }).ToList() ?? new List<WishlistItemDto>();
                        lvWishlist.ItemsSource = items;
                    }
                }
                catch { }
            }
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn?.Tag as WishlistItemDto;
            if (item == null || ShopQaWPF.App.CurrentUser == null) return;
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new System.Uri("https://localhost:7101/");
                if (!string.IsNullOrEmpty(ShopQaWPF.App.JwtToken))
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ShopQaWPF.App.JwtToken);
                try
                {
                    var response = await client.DeleteAsync($"api/Wishlist/remove?userId={ShopQaWPF.App.CurrentUser.Id}&productId={item.ProductId}");
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Đã xóa khỏi danh sách yêu thích!");
                        Wishlist_Loaded(this, new RoutedEventArgs());
                    }
                    else
                        MessageBox.Show("Xóa thất bại!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void ContinueShopping_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
                private void ProductDetail_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var item = fe?.Tag as WishlistItemDto;
            if (item == null) return;
            var detailWindow = new ShopQaWPF.SingleProduct(item.ProductId);
            detailWindow.ShowDialog();
        }
    }
}
