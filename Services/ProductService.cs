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
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch products: {response.StatusCode}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, NumberHandling = JsonNumberHandling.AllowReadingFromString };
            var productResponse = JsonSerializer.Deserialize<ProductResponse>(content, options);
            
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
    
    public async Task<SaleItemResponse> UpdateSaleItemAsync(int saleId, int itemId, int quantity, string token)
    {
        try
        {
            var request = new UpdateItemRequest
            {
                Quantity = quantity
            };
            
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // Set authorization header
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PatchAsync($"/api/sales/{saleId}/items/{itemId}", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to update sale item: {response.StatusCode}. Response: {errorContent}");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, NumberHandling = JsonNumberHandling.AllowReadingFromString };
            var saleItemResponse = JsonSerializer.Deserialize<SaleItemResponse>(responseContent, options);
            
            return saleItemResponse ?? new SaleItemResponse();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating sale item: {ex.Message}", ex);
        }
    }
    
    public async Task<SaleItemResponse> RemoveSaleItemAsync(int saleId, int itemId, string token)
    {
        try
        {
            // Set authorization header
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.DeleteAsync($"/api/sales/{saleId}/items/{itemId}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to remove sale item: {response.StatusCode}. Response: {errorContent}");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, NumberHandling = JsonNumberHandling.AllowReadingFromString };
            var saleItemResponse = JsonSerializer.Deserialize<SaleItemResponse>(responseContent, options);
            
            return saleItemResponse ?? new SaleItemResponse();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error removing sale item: {ex.Message}", ex);
        }
    }
    
    public async Task<CheckoutResponse> CheckoutSaleAsync(int saleId, string paymentMethod, decimal amount, string token)
    {
        try
        {
            var request = new CheckoutRequest
            {
                PaymentMethod = paymentMethod,
                Amount = amount
            };
            
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // Set authorization header
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PatchAsync($"/api/sales/{saleId}/checkout", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to checkout sale: {response.StatusCode}. Response: {errorContent}");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, NumberHandling = JsonNumberHandling.AllowReadingFromString };
            var checkoutResponse = JsonSerializer.Deserialize<CheckoutResponse>(responseContent, options);
            
            return checkoutResponse ?? new CheckoutResponse();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error checking out sale: {ex.Message}", ex);
        }
    }
}