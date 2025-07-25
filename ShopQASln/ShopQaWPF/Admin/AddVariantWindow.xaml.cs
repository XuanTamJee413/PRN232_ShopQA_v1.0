using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;

namespace ShopQaWPF.Admin
{
    public partial class AddVariantWindow : Window
    {
        private readonly int _productId;
        private string _selectedImagePath;

        public AddVariantWindow(int productId)
        {
            InitializeComponent();
            _productId = productId;
        }

        private void ChooseImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (dialog.ShowDialog() == true)
            {
                _selectedImagePath = dialog.FileName;
                ImagePathTextBlock.Text = Path.GetFileName(_selectedImagePath);
            }
        }

        private async void AddVariant_Click(object sender, RoutedEventArgs e)
        {
            string size = SizeTextBox.Text.Trim();
            string color = ColorTextBox.Text.Trim();
            string priceText = PriceTextBox.Text.Trim();
            string quantityText = InventoryQuantityTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(size) ||
                string.IsNullOrWhiteSpace(color) ||
                string.IsNullOrWhiteSpace(priceText) ||
                string.IsNullOrWhiteSpace(quantityText) ||
                string.IsNullOrWhiteSpace(_selectedImagePath))
            {
                MessageBox.Show("❌ Please fill all fields and choose an image.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("❌ Price must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(quantityText, out int quantity))
            {
                MessageBox.Show("❌ Quantity must be a valid integer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var form = new MultipartFormDataContent
                {
                    { new StringContent(size), "size" },
                    { new StringContent(color), "color" },
                    { new StringContent(price.ToString()), "price" },
                    { new StringContent(quantity.ToString()), "inventoryQuantity" },
                    { new StringContent(_productId.ToString()), "productId" }
                };

                byte[] fileBytes = await File.ReadAllBytesAsync(_selectedImagePath);
                var imageContent = new ByteArrayContent(fileBytes);

                var ext = Path.GetExtension(_selectedImagePath).ToLower();
                string contentType = ext switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => throw new InvalidOperationException("Unsupported image type")
                };

                imageContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                form.Add(imageContent, "ImageFile", Path.GetFileName(_selectedImagePath));

                using var client = new HttpClient();
                var response = await client.PostAsync("https://localhost:7101/api/Product/variant", form);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("✅ Variant added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"❌ Failed to add variant: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
