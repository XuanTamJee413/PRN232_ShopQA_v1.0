using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http.Json;
using ShopQaWPF.DTO;

namespace ShopQaWPF
{
    public partial class Shop : Window
    {
        private readonly HttpClient _httpClient;

        public Shop()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7101/") };
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", App.Current.Properties["Token"]?.ToString());

            LoadFilters();
            LoadProducts();
        }

        private async void LoadFilters()
        {
            var categoryResponse = await _httpClient.GetFromJsonAsync<ODataResponse<CategoryDto>>("odata/Category");
            var categories = categoryResponse?.Value ?? new List<CategoryDto>();
            categories.Insert(0, new CategoryDto { Id = 0, Name = "Tất cả" });

            cbCategory.ItemsSource = categories;
            cbCategory.DisplayMemberPath = "Name";
            cbCategory.SelectedValuePath = "Id";
            cbCategory.SelectedIndex = 0;

            var brandResponse = await _httpClient.GetFromJsonAsync<ODataResponse<BrandDto>>("odata/Brand");
            var brands = brandResponse?.Value ?? new List<BrandDto>();
            brands.Insert(0, new BrandDto { Id = 0, Name = "Tất cả" });

            cbBrand.ItemsSource = brands;
            cbBrand.DisplayMemberPath = "Name";
            cbBrand.SelectedValuePath = "Id";
            cbBrand.SelectedIndex = 0;
        }


        private async void LoadProducts(string? name = null, int? categoryId = null, int? brandId = null)
        {
            var filters = new List<string> { "$expand=Brand,Category" };

            if (!string.IsNullOrWhiteSpace(name))
                filters.Add($"$filter=contains(tolower(Name), '{name.ToLower()}')");

            if (categoryId.HasValue)
                filters.Add($"CategoryId eq {categoryId}");

            if (brandId.HasValue)
                filters.Add($"BrandId eq {brandId}");

            string filterString = "";
            var filterConditions = filters.Where(f => f.StartsWith("CategoryId") || f.StartsWith("BrandId") || f.StartsWith("$filter=")).ToList();

            if (filterConditions.Count > 0)
            {
                var where = filterConditions.Select(f =>
                {
                    if (f.StartsWith("$filter=")) return f.Replace("$filter=", "");
                    return f;
                });

                filters = filters.Where(f => !filterConditions.Contains(f)).ToList();
                filters.Add($"$filter={string.Join(" and ", where)}");
            }

            var url = "odata/Product?" + string.Join("&", filters);

            var response = await _httpClient.GetFromJsonAsync<ODataResponse<ProductDto>>(url);
            var products = response?.Value ?? new List<ProductDto>();
            lvProducts.ItemsSource = products;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var name = txtSearch.Text.Trim();
            var catId = (cbCategory.SelectedValue is int id && id != 0) ? id : (int?)null;
            var brandId = (cbBrand.SelectedValue is int id2 && id2 != 0) ? id2 : (int?)null;

            LoadProducts(name, catId, brandId);
        }
        private void Detail_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var product = btn?.Tag as ProductDto;
            if (product == null) return;

            var detailWindow = new SingleProduct(product.Id);
            detailWindow.ShowDialog();
        }
    }
}
