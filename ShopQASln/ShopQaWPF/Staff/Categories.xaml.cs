using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ShopQaWPF.Staff
{
    public class ODataResponse<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; }
    }

    public class Category : INotifyPropertyChanged
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

    public partial class Categories : Window
    {
       
        private static readonly HttpClient client = new HttpClient(
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

        private const string ApiBaseUrl = "https://localhost:7101/odata/Category";

        public ObservableCollection<Category> CategoriesCollection { get; set; }
        private ICollectionView CategoryView;

        public Categories()
        {
            InitializeComponent();
            CategoriesCollection = new ObservableCollection<Category>();
            CategoryView = CollectionViewSource.GetDefaultView(CategoriesCollection);
            CategoryView.Filter = FilterCategories;
            dgCategories.ItemsSource = CategoryView;
            this.Loaded += CategoriesWindow_Loaded;
        }

        private async void CategoriesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var odataResponse = await client.GetFromJsonAsync<ODataResponse<Category>>(ApiBaseUrl);
                CategoriesCollection.Clear();
                if (odataResponse?.Value != null)
                {
                    foreach (var category in odataResponse.Value)
                    {
                        CategoriesCollection.Add(category);
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
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Tên danh mục không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var newCategory = new { Name = txtCategoryName.Text };
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(ApiBaseUrl, newCategory);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm danh mục thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearInputs();
                    await LoadCategoriesAsync();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Thêm thất bại. Lỗi từ server: {response.ReasonPhrase}\n{error}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi thêm danh mục: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgCategories.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một danh mục để sửa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Tên danh mục không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var selectedCategory = (Category)dgCategories.SelectedItem;
            var updatedCategory = new { Id = selectedCategory.Id, Name = txtCategoryName.Text };
            string updateUrl = $"{ApiBaseUrl}({selectedCategory.Id})";
            try
            {
                HttpResponseMessage response = await client.PutAsJsonAsync(updateUrl, updatedCategory);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật danh mục thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearInputs();
                    await LoadCategoriesAsync();
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
            if (dgCategories.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một danh mục để xóa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var selectedCategory = (Category)dgCategories.SelectedItem;
            MessageBoxResult confirmation = MessageBox.Show($"Bạn có chắc chắn muốn xóa danh mục '{selectedCategory.Name}' không?",
                                                              "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmation == MessageBoxResult.Yes)
            {
                string deleteUrl = $"{ApiBaseUrl}({selectedCategory.Id})";
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync(deleteUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Xóa danh mục thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearInputs();
                        await LoadCategoriesAsync();
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Xóa thất bại. Tồn tại sản phẩm thuộc danh mục này");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Xóa thất bại. Tồn tại sản phẩm thuộc danh mục này");
                }
            }
        }

        #endregion

        #region UI Event Handlers
        private void dgCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCategories.SelectedItem is Category selectedCategory)
            {
                txtCategoryId.Text = selectedCategory.Id.ToString();
                txtCategoryName.Text = selectedCategory.Name;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CategoryView.Refresh();
        }

        private bool FilterCategories(object item)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                return true;
            }
            var category = item as Category;
            return category.Name.IndexOf(txtSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void ClearInputs()
        {
            txtCategoryId.Text = string.Empty;
            txtCategoryName.Text = string.Empty;
            dgCategories.SelectedItem = null;
        }
        #endregion
    }
}
