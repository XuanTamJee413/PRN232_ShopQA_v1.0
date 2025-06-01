using Domain.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShopQaMVC.Services
{
    public class CategoryApiService
    {
        private readonly HttpClient _httpClient;

        public CategoryApiService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ShopApi");
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Category>>("api/category");
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Category>($"api/category/{id}");
        }

        
        public async Task<bool> CreateAsync(Category category)
        {
            var response = await _httpClient.PostAsJsonAsync("api/category", category);
            return response.IsSuccessStatusCode;
        }

        
        public async Task<bool> UpdateAsync(Category category)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/category/{category.Id}", category);
            return response.IsSuccessStatusCode;
        }

       
        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/category/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
