// Services/SaleService.cs
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PosSale.Models;

namespace PosSale.Services;
public class SaleService : ISaleService
{
    private readonly HttpClient _httpClient;
    
    public SaleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<StartSaleResponse> StartSaleAsync(string token)
    {
        try
        {
            // Set the authorization header
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/sales", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Start sale failed with status code: {response.StatusCode}. Response: {errorContent}");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var startSaleResponse = JsonSerializer.Deserialize<StartSaleResponse>(responseContent, options);
            
            if (startSaleResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize start sale response");
            }
            
            return startSaleResponse;
        }
        catch (Exception ex)
        {
            throw new Exception($"Start sale error: {ex.Message}", ex);
        }
    }
}

// Models for the sale response
public class StartSaleResponse
{
    public string Status { get; set; } = string.Empty;
    public SaleData Data { get; set; } = new SaleData();
}

public class SaleData
{
    [JsonPropertyName("sale_id")]
    public int SaleId { get; set; }
    public string Message { get; set; } = string.Empty;
}