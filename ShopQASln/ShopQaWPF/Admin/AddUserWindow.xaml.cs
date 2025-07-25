using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace ShopQaWPF.Admin
{
    public partial class AddUserWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new System.Uri("https://localhost:7101/") };

        public AddUserWindow()
        {
            InitializeComponent();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string role = (RoleBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // ✅ Validate dữ liệu
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                MessageBox.Show("Valid email is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.");
                return;
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Role is required.");
                return;
            }

            // Gửi dữ liệu hợp lệ
            var newUser = new Users.UserModel
            {
                Username = username,
                Email = email,
                Password = password,
                Role = role,
                Status = "Active"
            };

            var response = await new HttpClient { BaseAddress = new Uri("https://localhost:7101/") }
                .PostAsJsonAsync("api/User", newUser);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("User created successfully.");
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to create user.");
            }
        }

    }
}
