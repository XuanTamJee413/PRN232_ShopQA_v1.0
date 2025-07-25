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
        private async void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (currentVariant == null)
            {
                MessageBox.Show("Vui lòng chọn màu và size.");
                return;
            }

            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Số lượng không hợp lệ.");
                return;
            }

            var confirmMsg = $"Xác nhận thêm vào giỏ:\n\nSản phẩm: {ProductName.Text}\n" +
                             $"Màu: {currentVariant.Color}\nSize: {currentVariant.Size}\nSố lượng: {quantity}";
            var result = MessageBox.Show(confirmMsg, "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                int userId = App.CurrentUser.Id;

                // 1. Tìm cart status = "Open" của user
                var cartRes = await _httpClient.GetAsync($"/odata/Cart?$filter=UserId eq {userId} and Status eq 'Open'&$expand=Items");
                var cartJson = await cartRes.Content.ReadAsStringAsync();
                var root = JsonDocument.Parse(cartJson).RootElement;
                JsonElement? openCart = null;

                if (root.TryGetProperty("value", out var cartList) && cartList.GetArrayLength() > 0)
                {
                    openCart = cartList[0];
                }

                if (openCart != null)
                {
                    int cartId = openCart.Value.GetProperty("Id").GetInt32();
                    var items = openCart.Value.GetProperty("Items");

                    var existedItem = items.EnumerateArray()
                        .FirstOrDefault(i => i.GetProperty("ProductVariantId").GetInt32() == currentVariant.Id);

                    if (existedItem.ValueKind != JsonValueKind.Undefined)
                    {
                        // 2. Nếu item đã có → PATCH tăng số lượng
                        int itemId = existedItem.GetProperty("Id").GetInt32();
                        int oldQty = existedItem.GetProperty("Quantity").GetInt32();
                        int maxStock = currentVariant.Stock;
                        int newQty = oldQty + quantity;

                        if (newQty > maxStock)
                        {
                            MessageBox.Show($"Vượt quá số lượng tồn kho! Hiện còn {maxStock}, bạn đã có {oldQty} trong giỏ.");
                            return;
                        }

                        var patchData = new { Quantity = newQty };
                        var patchJson = JsonSerializer.Serialize(patchData);
                        var patchContent = new StringContent(patchJson, Encoding.UTF8, "application/json");

                        var patchRes = await _httpClient.PatchAsync($"/odata/CartItems({itemId})", patchContent);
                        if (patchRes.IsSuccessStatusCode)
                            MessageBox.Show("Cập nhật số lượng thành công!");
                        else
                            MessageBox.Show("Lỗi cập nhật item: " + await patchRes.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        // 3. Nếu item chưa có → POST thêm mới
                        var newItem = new
                        {
                            ProductVariantId = currentVariant.Id,
                            Quantity = quantity,
                            CartId = cartId
                        };
                        var postJson = JsonSerializer.Serialize(newItem);
                        var postRes = await _httpClient.PostAsync("/odata/CartItems", new StringContent(postJson, Encoding.UTF8, "application/json"));

                        if (postRes.IsSuccessStatusCode)
                            MessageBox.Show("Thêm vào giỏ hàng thành công!");
                        else
                            MessageBox.Show("Lỗi thêm mới item: " + await postRes.Content.ReadAsStringAsync());
                    }
                }
                else
                {
                    // 4. Chưa có cart → tạo mới cart với status = Open
                    var newCart = new
                    {
                        UserId = userId,
                        Status = "Open",
                        CreatedAt = DateTime.UtcNow,
                        Items = new[]
                        {
                    new {
                        ProductVariantId = currentVariant.Id,
                        Quantity = quantity
                    }
                }
                    };

                    var cartJsonBody = JsonSerializer.Serialize(newCart);
                    var cartRes2 = await _httpClient.PostAsync("/odata/Cart", new StringContent(cartJsonBody, Encoding.UTF8, "application/json"));

                    if (cartRes2.IsSuccessStatusCode)
                        MessageBox.Show("Tạo giỏ mới và thêm sản phẩm thành công!");
                    else
                        MessageBox.Show("Lỗi tạo mới cart: " + await cartRes2.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }


    }
}
