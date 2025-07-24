using Newtonsoft.Json;
using ShopQaWPF.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ShopQaWPF.Admin
{
    public partial class Product : Window
    {
        private readonly HttpClient _httpClient;

        public Product()
        {
            InitializeComponent();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7101/")
            };

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // BỎ token nếu không cần xác thực
            // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_JWT_TOKEN_HERE");

            Loaded += Product_Loaded;
        }

        private async void Product_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCategoriesAsync();
            await LoadProductsAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("odata/Category");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ODataResult<CategoryDto>>(content);

                CategoryComboBox.ItemsSource = result.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Failed to load categories: " + ex.Message);
            }
        }

        private async Task LoadProductsAsync(string searchName = "", int? categoryId = null)
        {
            try
            {
                string url = "api/Product";

                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(searchName))
                    queryParams.Add($"name={Uri.EscapeDataString(searchName)}");
                if (categoryId.HasValue)
                    queryParams.Add($"categoryId={categoryId}");

                if (queryParams.Count > 0)
                    url += "?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ProductDto>>(content);

                ProductDataGrid.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Failed to load products: " + ex.Message);
            }
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            string name = SearchTextBox.Text.Trim();
            int? categoryId = CategoryComboBox.SelectedValue as int?;
            await LoadProductsAsync(name, categoryId);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int productId))
            {
                var updateWindow = new UpdateProductWindow(productId);
                updateWindow.ShowDialog();

                // Sau khi cập nhật xong, reload lại danh sách
                _ = LoadProductsAsync();
            }
        }
        private void ProductName_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productId)
            {
                var detailWindow = new ProductDetailWindow(productId);
                detailWindow.ShowDialog();

                _ = LoadProductsAsync();
            }
        }
        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddProductWindow();
            if (addWindow.ShowDialog() == true)
            {
                await LoadProductsAsync(); // refresh lại danh sách
            }
        }


        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var confirm = MessageBox.Show(
                    "Are you sure you want to delete this product?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"api/Product/{id}");
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("✅ Product deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadProductsAsync(); // refresh grid
                        }
                        else
                        {
                            var errorMsg = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"❌ Failed to delete product:\n{errorMsg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ Exception while deleting product:\n{ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }




        // Class hỗ trợ cho OData response
        public class ODataResult<T>
        {
            [JsonProperty("value")]
            public List<T> Value { get; set; }
        }
    }
}
