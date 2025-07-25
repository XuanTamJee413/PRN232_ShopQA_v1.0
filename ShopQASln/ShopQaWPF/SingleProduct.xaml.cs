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

namespace ShopQaWPF
{
    class ProductVariantTempDto
    {
        public int Id { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
    }

    public partial class SingleProduct : Window
    {
        private readonly HttpClient _httpClient;
        private List<ProductVariantTempDto> variants;
        private ProductVariantTempDto currentVariant;

        private int productId;

        public SingleProduct(int productId)
        {
            InitializeComponent();
            this.productId = productId;

            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7101/") };
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.JwtToken);

            LoadProduct();
        }


        private async void LoadProduct()
        {
            try
            {
                var res = await _httpClient.GetAsync($"/odata/Product({productId})?$expand=Variants,Category,Brand");
                var json = await res.Content.ReadAsStringAsync();

                var product = JsonDocument.Parse(json).RootElement;

                ProductName.Text = product.GetProperty("Name").GetString();
                ProductDescription.Text = product.GetProperty("Description").GetString();
                ProductBrand.Text = "Thương hiệu: " + product.GetProperty("Brand").GetProperty("Name").GetString();
                ProductCategory.Text = "Danh mục: " + product.GetProperty("Category").GetProperty("Name").GetString();

                string img = product.GetProperty("ImageUrl").GetString();
                if (!string.IsNullOrEmpty(img))
                    ProductImage.Source = new BitmapImage(new Uri(img));

                // parse biến thể
                variants = JsonSerializer.Deserialize<List<ProductVariantTempDto>>(
                    product.GetProperty("Variants").ToString(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (variants == null || variants.Count == 0)
                {
                    MessageBox.Show("Không có biến thể sản phẩm");
                    return;
                }

                ColorComboBox.ItemsSource = variants.Select(v => v.Color).Distinct().ToList();
                SizeComboBox.ItemsSource = variants.Select(v => v.Size).Distinct().ToList();

                // chọn variant đầu tiên
                var first = variants[0];
                ColorComboBox.SelectedItem = first.Color;
                SizeComboBox.SelectedItem = first.Size;
                UpdateVariantUI(first);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải sản phẩm: " + ex.Message);
            }
        }

        private void OnVariantSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedColor = ColorComboBox.SelectedItem as string;
            string selectedSize = SizeComboBox.SelectedItem as string;

            if (selectedColor != null && selectedSize != null)
            {
                var matched = variants.FirstOrDefault(v => v.Color == selectedColor && v.Size == selectedSize);
                if (matched != null)
                    UpdateVariantUI(matched);
            }
        }

        private void UpdateVariantUI(ProductVariantTempDto variant)
        {
            currentVariant = variant;
            VariantPrice.Text = $"Giá: {variant.Price:N0}₫";
            VariantStock.Text = $"Kho: {variant.Stock}";

            if (!string.IsNullOrEmpty(variant.ImageUrl))
                ProductImage.Source = new BitmapImage(new Uri(variant.ImageUrl));
        }
    }
}
