using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PosSale.Models;

namespace PosSale.Services;
public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    
    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<Category>> GetCategoriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/categories");
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch categories: {response.StatusCode}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, NumberHandling = JsonNumberHandling.AllowReadingFromString };
            var categoryResponse = JsonSerializer.Deserialize<CategoryResponse>(content, options);
            
            return categoryResponse?.Data ?? new List<Category>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching categories: {ex.Message}", ex);
        }
    }
    
    public async Task<ProductResponse> GetProductsAsync(int? categoryId = null, string? search = null, int page = 1)
    {
        try
        {
            var queryParams = new List<string>();
            
            if (categoryId.HasValue)
                queryParams.Add($"category_id={categoryId.Value}");
                
            if (!string.IsNullOrEmpty(search))
                queryParams.Add($"search={Uri.EscapeDataString(search)}");
                
            queryParams.Add($"page={page}");
            
            var queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : "";
            var url = $"/api/products{queryString}";
            Console.WriteLine($"[ProductService] GET {url}");
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch products: {response.StatusCode}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[ProductService] Products payload: {content}");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, NumberHandling = JsonNumberHandling.AllowReadingFromString };
            var productResponse = JsonSerializer.Deserialize<ProductResponse>(content, options);
            
            var count = productResponse?.Data?.Products?.Count ?? 0;
            Console.WriteLine($"[ProductService] Parsed products count: {count}");
            
            return productResponse ?? new ProductResponse();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching products: {ex.Message}", ex);
        }
    }
    
    public async Task<SaleItemResponse> AddItemToSaleAsync(int saleId, int productId, int quantity, string token)
    {
        try
        {
            var request = new SaleItemRequest
            {
                ProductId = productId,
                Quantity = quantity
            };
            
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // Set authorization header
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PostAsync($"/api/sales/{saleId}/items", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to add item to sale: {response.StatusCode}. Response: {errorContent}");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, NumberHandling = JsonNumberHandling.AllowReadingFromString };
            var saleItemResponse = JsonSerializer.Deserialize<SaleItemResponse>(responseContent, options);
            
            return saleItemResponse ?? new SaleItemResponse();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding item to sale: {ex.Message}", ex);
        }
    }
}