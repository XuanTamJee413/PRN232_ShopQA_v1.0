// This file is already complete and represents a WPF window for displaying product details.
// Now we will adjust the WPF logic to align with the structure and behavior of the Razor Page code using API calls
// and JWT token for authorization.

using Newtonsoft.Json;
using ShopQaWPF.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShopQaWPF.Admin
{
    public partial class ProductDetailWindow : Window
    {
        private readonly HttpClient _httpClient;
        private int _productId;
        private string _jwtToken;
        private List<ProductVariantDto> _allVariants = new List<ProductVariantDto>();
        private int _currentPage = 1;
        private const int PageSize = 5;

        public ProductDetailWindow(int productId)
        {
            InitializeComponent();
            _productId = productId;

            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7101/") };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Loaded += ProductDetailWindow_Loaded;
        }

        private async void ProductDetailWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadProductDetailsAsync();
        }

        private async Task LoadProductDetailsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Product/{_productId}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<ProductDetailDto>(content);

                if (product != null)
                {
                    MainImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(product.ImageUrl ?? "/Images/default.png", UriKind.RelativeOrAbsolute));
                    ProductName.Text = product.Name;
                    ProductDescription.Text = product.Description;
                    ProductCategory.Text = product.CategoryName;
                    ProductBrand.Text = product.Brand?.Name ?? "N/A";
                    _allVariants = product.Variants ?? new List<ProductVariantDto>();
                    DisplayVariants();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Failed to load product details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayVariants()
        {
            string searchName = SearchVariantTextBox.Text.Trim().ToLower();
            decimal minPrice = decimal.TryParse(MinPriceTextBox.Text, out var parsedMin) ? parsedMin : 0;
            decimal maxPrice = decimal.TryParse(MaxPriceTextBox.Text, out var parsedMax) ? parsedMax : decimal.MaxValue;

            var filteredVariants = _allVariants.FindAll(v =>
              (string.IsNullOrEmpty(searchName) || v.Size.ToLower().Contains(searchName) || v.Color.ToLower().Contains(searchName)) &&
              v.Price >= minPrice && v.Price <= maxPrice);

            int startIndex = (_currentPage - 1) * PageSize;
            int endIndex = Math.Min(startIndex + PageSize, filteredVariants.Count);
            var variantsToDisplay = filteredVariants.GetRange(startIndex, endIndex - startIndex);

            VariantDataGrid.ItemsSource = variantsToDisplay;
            PageNumberText.Text = $"Page {_currentPage}";
            PrevPageBtn.IsEnabled = _currentPage > 1;
            NextPageBtn.IsEnabled = (_currentPage * PageSize) < filteredVariants.Count;
        }

        private void PrevPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                DisplayVariants();
            }
        }
        // Sửa biến thể
        private void EditVariant_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int variantId)
            {
                MessageBox.Show($"Edit Variant {variantId} clicked!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                // TODO: Mở form EditVariantWindow với variantId
            }
        }

        // Xóa biến thể
        private async void DeleteVariant_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int variantId)
            {
                var confirm = MessageBox.Show("Are you sure you want to delete this variant?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"api/ProductVariant/{variantId}");
                        response.EnsureSuccessStatusCode();

                        // Xóa khỏi danh sách hiện tại
                        _allVariants.RemoveAll(v => v.Id == variantId);
                        DisplayVariants();
                        MessageBox.Show("✅ Variant deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ Failed to delete variant: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void NextPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((_currentPage * PageSize) < _allVariants.Count)
            {
                _currentPage++;
                DisplayVariants();
            }
        }
        private void AddVariant_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddVariantWindow(_productId);
            addWindow.Owner = this;
            addWindow.ShowDialog();

            // Sau khi đóng, load lại danh sách nếu cần
            _ = LoadProductDetailsAsync();
        }

        private void FilterVariants_Click(object sender, RoutedEventArgs e)
        {
            _currentPage = 1;
            DisplayVariants();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }

    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public BrandDto Brand { get; set; }
        public List<ProductVariantDto> Variants { get; set; }
    }

    public class ProductVariantDto
    {
        public int Id { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public InventoryDto Inventory { get; set; }
    }

    public class InventoryDto
    {
        public int Quantity { get; set; }
    }

    public class BrandDto
    {
        public string Name { get; set; }
    }
}
