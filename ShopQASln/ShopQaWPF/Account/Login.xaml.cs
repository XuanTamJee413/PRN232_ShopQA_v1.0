using ShopQaWPF.Admin;
using ShopQaWPF.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShopQaWPF.Account
{
    public partial class Login : Window
    {
        private readonly HttpClient _httpClient;

        public Login()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7101/")
            };
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            txtMessage.Visibility = Visibility.Collapsed;

            var username = txtUsername.Text.Trim();
            var password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowMessage("Vui lòng nhập đầy đủ thông tin.");
                username = "tranthib";
                password = "123";
                //return;
            }

            var loginDto = new
            {
                UsernameOrEmail = username,
                Password = password
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/Auth/login", content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var user = JsonSerializer.Deserialize<UserDto>(responseText, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Lưu token và user toàn cục
                    App.JwtToken = user.Token;
                    App.CurrentUser = user;

                    // 👇 Lưu vào LocalStorage
                    LocalStorage.Set("JwtToken", user.Token);
                    LocalStorage.Set("User", user);


                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);

                    ShowMessage($"Đăng nhập thành công! Người dùng: {user.Username} | Quyền: {user.Role}");

                    await Task.Delay(2000);

                    Window nextWindow = user.Role switch
                    {
                        "Admin" => new Dashboard(),          // Users.xaml
                      //  "Staff" => new Dashboard(),        // Dashboard.xaml
                        "Customer" => new Shop(),        // Shop.xaml
                        _ => null
                    };

                    nextWindow?.Show();
                    this.Close();
                }
                else
                {
                    ShowMessage("Đăng nhập thất bại: " + responseText);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Lỗi kết nối: " + ex.Message);
            }
        }

        private void ShowMessage(string message)
        {
            txtMessage.Text = message;
            txtMessage.Visibility = Visibility.Visible;
        }
    }

    
}
