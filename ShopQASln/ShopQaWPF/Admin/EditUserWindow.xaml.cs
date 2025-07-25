using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace ShopQaWPF.Admin
{
    public partial class EditUserWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new System.Uri("https://localhost:7101/") };
        private readonly Users.UserModel _user;

        public EditUserWindow(Users.UserModel user)
        {
            InitializeComponent();
            _user = user;
            LoadUserData();
        }

        private void LoadUserData()
        {
            UsernameBox.Text = _user.Username;
            EmailBox.Text = _user.Email;

            // Select role in ComboBox
            foreach (ComboBoxItem item in RoleBox.Items)
            {
                if (item.Content.ToString() == _user.Role)
                {
                    RoleBox.SelectedItem = item;
                    break;
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var updatedUser = new Users.UserModel
            {
                Id = _user.Id,
                Username = UsernameBox.Text.Trim(),
                Email = EmailBox.Text.Trim(),
                Role = (RoleBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Status = _user.Status
            };

            var response = await _httpClient.PutAsJsonAsync($"api/User/{_user.Id}", updatedUser);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("User updated successfully.");
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to update user.");
            }
        }

        private bool ValidateInput(out string message)
        {
            if (string.IsNullOrWhiteSpace(UsernameBox.Text))
            {
                message = "Username is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailBox.Text) || !EmailBox.Text.Contains("@"))
            {
                message = "A valid email is required.";
                return false;
            }

            if (RoleBox.SelectedItem == null)
            {
                message = "Role must be selected.";
                return false;
            }

            message = string.Empty;
            return true;
        }
    }
}
