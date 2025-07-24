using Microsoft.Win32;
using Newtonsoft.Json;
using ShopQaWPF.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ShopQaWPF.Admin
{
    public partial class AddProductWindow : Window
    {
        private readonly HttpClient _httpClient;
        private string _selectedImagePath;

        public AddProductWindow()
        {
            InitializeComponent();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7101/")
            };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Loaded += AddProductWindow_Loaded;
        }

        private async void AddProductWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCategoriesAsync();
            await LoadBrandsAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("odata/Category");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ODataResult<CategoryDto>>(json);
            CategoryComboBox.ItemsSource = result.Value;
        }

        private async Task LoadBrandsAsync()
        {
            var response = await _httpClient.GetAsync("odata/Brand");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ODataResult<BrandDto>>(json);
            BrandComboBox.ItemsSource = result.Value;
        }

        private void ChooseImage_Click(object sender, RoutedEventArgs e)
        {
            var openFile = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFile.ShowDialog() == true)
            {
                _selectedImagePath = openFile.FileName;
                ImagePathTextBlock.Text = Path.GetFileName(_selectedImagePath);
            }
        }

        private async void CreateProduct_Click(object sender, RoutedEventArgs e)
        {
            var name = NameTextBox.Text.Trim();
            var desc = DescriptionTextBox.Text.Trim();

            var selectedCategory = CategoryComboBox.SelectedItem as CategoryDto;
            var selectedBrand = BrandComboBox.SelectedItem as BrandDto;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("❌ Please enter a product name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(desc))
            {
                MessageBox.Show("❌ Please enter a description.");
                return;
            }

            if (selectedCategory == null)
            {
                MessageBox.Show("❌ Please select a category.");
                return;
            }

            //if (selectedBrand == null)
            //{
            //    MessageBox.Show("❌ Please select a brand.");
            //    return;
            //}

            if (string.IsNullOrWhiteSpace(_selectedImagePath) || !File.Exists(_selectedImagePath))
            {
                MessageBox.Show("❌ Please select a valid image file.");
                return;
            }

            var form = new MultipartFormDataContent
    {
        { new StringContent(name), "name" },
        { new StringContent(desc), "description" },
        { new StringContent(selectedCategory.Id.ToString()), "categoryId" },
        { new StringContent("2"), "brandId" }
    };

            try
            {
                var fileBytes = await File.ReadAllBytesAsync(_selectedImagePath);
                var imageContent = new ByteArrayContent(fileBytes);

                var extension = Path.GetExtension(_selectedImagePath).ToLower();
                string contentType = extension switch
                {
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    _ => throw new InvalidOperationException("Unsupported image format")
                };

                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                form.Add(imageContent, "image", Path.GetFileName(_selectedImagePath));

                var response = await _httpClient.PostAsync("api/Product", form);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("✅ Product created successfully!");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Failed to create product:\n" + ex.Message);
            }
        }


        public class ODataResult<T>
        {
            [JsonProperty("value")]
            public List<T> Value { get; set; }
        }
    }
}
