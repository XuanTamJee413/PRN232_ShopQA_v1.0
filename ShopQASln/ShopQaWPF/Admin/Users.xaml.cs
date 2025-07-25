using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ShopQaWPF.Models;

namespace ShopQaWPF.Admin
{
    public partial class Users : Window
    {
        public class UserModel
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Password { get; set; }
            public string Role { get; set; } = string.Empty;
            public string Status { get; set; } = "Active";
        }
        private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new System.Uri("https://localhost:7101/") };
        private List<UserModel> _users = new();

        public Users()
        {
            InitializeComponent();
            LoadUsers();
        }
        private void AddNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddUserWindow();
            addUserWindow.Owner = this;
            if (addUserWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private async void LoadUsers(string keyword = "", string role = "")
        {
            try
            {
                string url = $"api/User/search-users?keyword={keyword}&role={role}";
                _users = await _httpClient.GetFromJsonAsync<List<UserModel>>(url);
                UserDataGrid.ItemsSource = _users;
            }
            catch
            {
                MessageBox.Show("Failed to load users.");
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchBox.Text.Trim();
            string role = (RoleFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            role = role == "All Roles" ? "" : role;
            LoadUsers(keyword, role);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as FrameworkElement)?.DataContext as UserModel;
            if (user != null)
            {
                var editWindow = new EditUserWindow(user);
                editWindow.Owner = this;
                if (editWindow.ShowDialog() == true)
                {
                    LoadUsers();
                }
            }
        }


        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as FrameworkElement)?.DataContext as UserModel;
            if (user != null)
            {
                if (MessageBox.Show($"Delete user '{user.Username}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    await _httpClient.DeleteAsync($"api/User/{user.Id}");
                    LoadUsers();
                }
            }
        }

        private async void BanButton_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as FrameworkElement)?.DataContext as UserModel;
            if (user != null)
            {
                await _httpClient.PutAsync($"api/User/status/{user.Id}?status=Deactive", null);
                LoadUsers();
            }
        }

        private async void UnbanButton_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as FrameworkElement)?.DataContext as UserModel;
            if (user != null)
            {
                await _httpClient.PutAsync($"api/User/status/{user.Id}?status=Active", null);
                LoadUsers();
            }
        }
    }
}
