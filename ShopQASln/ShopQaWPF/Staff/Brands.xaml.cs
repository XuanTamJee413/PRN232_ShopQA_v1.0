using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ShopQaWPF.Staff
{
    public class Brand : INotifyPropertyChanged
    {
        private int _id;
        [JsonPropertyName("id")]
        public int Id
        {
            get => _id;
            set { if (_id != value) { _id = value; OnPropertyChanged(nameof(Id)); } }
        }

        private string _name;
        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value; OnPropertyChanged(nameof(Name)); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

   
    public partial class ManageBrands : Window
    {
        private static readonly HttpClient client = new HttpClient(
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

        private const string ApiBaseUrl = "https://localhost:7101/odata/Brand";

        public ObservableCollection<Brand> BrandsCollection { get; set; }
        private ICollectionView BrandView;

        public ManageBrands()
        {
            InitializeComponent();
            BrandsCollection = new ObservableCollection<Brand>();
            BrandView = CollectionViewSource.GetDefaultView(BrandsCollection);
            BrandView.Filter = FilterBrands;
            dgBrands.ItemsSource = BrandView; 
            this.Loaded += ManageBrandsWindow_Loaded;
        }

        private async void ManageBrandsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadBrandsAsync();
        }

        private async Task LoadBrandsAsync()
        {
            try
            {
                var odataResponse = await client.GetFromJsonAsync<ODataResponse<Brand>>(ApiBaseUrl);
                BrandsCollection.Clear();
                if (odataResponse?.Value != null)
                {
                    foreach (var brand in odataResponse.Value)
                    {
                        BrandsCollection.Add(brand);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "\n\nChi tiết: " + ex.InnerException.Message;
                }
                MessageBox.Show($"Không thể tải dữ liệu từ API.\nLỗi: {errorMessage}", "Lỗi API", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region CRUD Button Handlers

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBrandName.Text))
            {
                MessageBox.Show("Tên thương hiệu không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var newBrand = new { Name = txtBrandName.Text };
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(ApiBaseUrl, newBrand);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm thương hiệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearInputs();
                    await LoadBrandsAsync();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Thêm thất bại. Lỗi từ server: {response.ReasonPhrase}\n{error}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi thêm thương hiệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgBrands.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một thương hiệu để sửa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtBrandName.Text))
            {
                MessageBox.Show("Tên thương hiệu không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var selectedBrand = (Brand)dgBrands.SelectedItem;
            var updatedBrand = new { Id = selectedBrand.Id, Name = txtBrandName.Text };
            string updateUrl = $"{ApiBaseUrl}({selectedBrand.Id})";
            try
            {
                HttpResponseMessage response = await client.PutAsJsonAsync(updateUrl, updatedBrand);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật thương hiệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearInputs();
                    await LoadBrandsAsync();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Cập nhật thất bại. Lỗi từ server: {response.ReasonPhrase}\n{error}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgBrands.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một thương hiệu để xóa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var selectedBrand = (Brand)dgBrands.SelectedItem;
            MessageBoxResult confirmation = MessageBox.Show($"Bạn có chắc chắn muốn xóa thương hiệu '{selectedBrand.Name}' không?",
                                                              "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmation == MessageBoxResult.Yes)
            {
                string deleteUrl = $"{ApiBaseUrl}({selectedBrand.Id})";
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync(deleteUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Xóa thương hiệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearInputs();
                        await LoadBrandsAsync();
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.Conflict)
                        {
                            MessageBox.Show(error, "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Xóa thất bại. Có sản phẩm thuộc thương hiệu này");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Xóa thất bại. Có sản phẩm thuộc thương hiệu này");
                }
            }
        }

        #endregion

        #region UI Event Handlers
        private void dgBrands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBrands.SelectedItem is Brand selectedBrand)
            {
                txtBrandId.Text = selectedBrand.Id.ToString();
                txtBrandName.Text = selectedBrand.Name;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            BrandView.Refresh();
        }

        private bool FilterBrands(object item)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                return true;
            }
            var brand = item as Brand;
            return brand.Name.IndexOf(txtSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void ClearInputs()
        {
            txtBrandId.Text = string.Empty;
            txtBrandName.Text = string.Empty;
            dgBrands.SelectedItem = null;
        }
        #endregion
    }
}
