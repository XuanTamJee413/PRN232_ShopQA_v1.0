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
using ShopQaWPF.ViewModels;
namespace ShopQaWPF.Customer
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        public Profile()
        {
            InitializeComponent();
            var vm = new ProfileViewModel();
            // Lấy thông tin user từ App
            vm.User = ShopQaWPF.App.CurrentUser;
            DataContext = vm;

            // Gọi API lấy thông tin chi tiết
            Loaded += async (s, e) =>
            {
                if (vm.User != null)
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        client.BaseAddress = new System.Uri("https://localhost:7101/");
                        if (!string.IsNullOrEmpty(ShopQaWPF.App.JwtToken))
                            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ShopQaWPF.App.JwtToken);
                        try
                        {
                            var response = await client.GetAsync($"api/Profile/{vm.User.Id}");
                            if (response.IsSuccessStatusCode)
                            {
                                var json = await response.Content.ReadAsStringAsync();
                                var userDetail = System.Text.Json.JsonSerializer.Deserialize<UserDetailDto>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                if (userDetail != null)
                                {
                                    vm.Address = userDetail.Address;
                                    vm.City = userDetail.City;
                                    vm.Country = userDetail.Country;
                                }
                            }
                        }
                        catch { }
                    }
                }
            };
            SaveChanges_ClickedVm = vm;
            ChangePassword_ClickedVm = vm;
        }

        private ProfileViewModel SaveChanges_ClickedVm;
        private ProfileViewModel ChangePassword_ClickedVm;

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            var vm = SaveChanges_ClickedVm;
            if (vm?.User == null) return;
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new System.Uri("https://localhost:7101/");
                if (!string.IsNullOrEmpty(ShopQaWPF.App.JwtToken))
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ShopQaWPF.App.JwtToken);
                var userDto = new UserDetailDto
                {
                    Id = vm.User.Id,
                    Username = vm.User.Username,
                    Email = vm.User.Email,
                    Address = vm.Address,
                    City = vm.City,
                    Country = vm.Country
                };
                var json = System.Text.Json.JsonSerializer.Serialize(userDto);
                var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                try
                {
                    var response = await client.PutAsync($"api/Profile/{vm.User.Id}", content);
                    if (response.IsSuccessStatusCode)
                        MessageBox.Show("Cập nhật thành công!");
                    else
                        MessageBox.Show("Cập nhật thất bại!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private async void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var vm = ChangePassword_ClickedVm;
            if (vm?.User == null) return;
            var oldPassword = OldPasswordBox.Password;
            var newPassword = NewPasswordBox.Password;
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu cũ và mới.");
                return;
            }
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new System.Uri("https://localhost:7101/");
                if (!string.IsNullOrEmpty(ShopQaWPF.App.JwtToken))
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ShopQaWPF.App.JwtToken);
                var req = new { OldPassword = oldPassword, NewPassword = newPassword };
                var json = System.Text.Json.JsonSerializer.Serialize(req);
                var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                try
                {
                    var response = await client.PutAsync($"api/Profile/{vm.User.Id}/change-password", content);
                    if (response.IsSuccessStatusCode)
                        MessageBox.Show("Đổi mật khẩu thành công!");
                    else
                    {
                        var respText = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Đổi mật khẩu thất bại: " + respText);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
    }
}
