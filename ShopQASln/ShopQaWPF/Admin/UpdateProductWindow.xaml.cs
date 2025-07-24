using Microsoft.Win32;
using Newtonsoft.Json;
using ShopQaWPF.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ShopQaWPF.Admin
{
    public partial class UpdateProductWindow : Window
    {
        private readonly int productId;
        private string selectedImagePath;
        private readonly string baseUrl = "https://localhost:7101";
        private readonly string token = ""; // 🔑 Nếu cần truyền token, thêm vào đây

        public UpdateProductWindow(int productId)
        {
            InitializeComponent();
            this.productId = productId;
            _ = InitializeAsync();
        }

        private async System.Threading.Tasks.Task InitializeAsync()
        {
            await LoadCategoriesAsync();
            await LoadBrandsAsync();
            await LoadProductDetailsAsync();
        }

        private async System.Threading.Tasks.Task LoadCategoriesAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{baseUrl}/odata/Category");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ODataResponse<CategoryDto>>(json);
            CategoryComboBox.ItemsSource = result.Value;
            CategoryComboBox.DisplayMemberPath = "Name";
            CategoryComboBox.SelectedValuePath = "Id";
        }

        private async System.Threading.Tasks.Task LoadBrandsAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{baseUrl}/odata/Brand");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ODataResponse<BrandDto>>(json);
            BrandComboBox.ItemsSource = result.Value;
            BrandComboBox.DisplayMemberPath = "Name";
            BrandComboBox.SelectedValuePath = "Id";
        }

        private async System.Threading.Tasks.Task LoadProductDetailsAsync()
        {
            using var client = new HttpClient();
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{baseUrl}/api/Product/{productId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<ProductDto>(json);

            NameTextBox.Text = dto.Name;
            DescriptionTextBox.Text = dto.Description;
            CategoryComboBox.SelectedValue = dto.CategoryId;
            BrandComboBox.SelectedValue = dto.BrandId;

            if (!string.IsNullOrEmpty(dto.ImageUrl))
                ProductImage.Source = new BitmapImage(new Uri(dto.ImageUrl, UriKind.Absolute));
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Image files (*.jpg;*.png)|*.jpg;*.png" };
            if (dlg.ShowDialog() == true)
            {
                selectedImagePath = dlg.FileName;
                ProductImage.Source = new BitmapImage(new Uri(selectedImagePath));
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text)
                || CategoryComboBox.SelectedValue == null
                || BrandComboBox.SelectedValue == null)
            {
                MessageBox.Show("Please fill all required fields.");
                return;
            }

            using var client = new HttpClient();
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(NameTextBox.Text), "Name");
            form.Add(new StringContent(DescriptionTextBox.Text ?? ""), "Description");
            form.Add(new StringContent(CategoryComboBox.SelectedValue.ToString()), "CategoryId");
            form.Add(new StringContent(BrandComboBox.SelectedValue.ToString()), "BrandId");

            if (!string.IsNullOrEmpty(selectedImagePath))
            {
                var stream = File.OpenRead(selectedImagePath);
                var image = new StreamContent(stream);
                image.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                form.Add(image, "Image", Path.GetFileName(selectedImagePath));
            }

            var response = await client.PutAsync($"{baseUrl}/api/Product/{productId}", form);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("✅ Product updated successfully!");
                this.Close();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"❌ Failed to update product\n{response.StatusCode}\n{error}");
            }
        }
    }

    // Dùng cho OData
    public class ODataResponse<T>
    {
        [JsonProperty("value")]
        public List<T> Value { get; set; }
    }
}
