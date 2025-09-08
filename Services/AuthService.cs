using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PosSale.Models;

namespace PosSale.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    public string ErrorMessage { get; private set; }
    public bool HasError { get; private set; }
    
    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        
        if (_httpClient.BaseAddress == null)
        {
            Console.WriteLine("ERROR: HttpClient BaseAddress is null");
            Console.WriteLine($"HttpClient configuration: {_httpClient}");
        }
        else
        {
            Console.WriteLine($"HttpClient configured with BaseAddress: {_httpClient.BaseAddress}");
        }
    }
    
    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        try
        {
            if (_httpClient.BaseAddress == null)
            {
                ErrorMessage = "API server configuration error - BaseAddress not set";
                HasError = true;
                return null;
            }
            
            // Ensure leading slash so it combines correctly with BaseAddress
            var requestUri = new Uri("/api/auth/login", UriKind.Relative);
            Console.WriteLine($"Request URI: {_httpClient.BaseAddress}{requestUri}");
            
            var response = await _httpClient.PostAsJsonAsync(requestUri, 
                new { email, password });
                
            Console.WriteLine($"Response status: {response.StatusCode}");
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response content: {responseContent}");
            
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = $"Login failed: {responseContent}";
                HasError = true;
                return null;
            }
            
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Request Error: {ex}");
            ErrorMessage = $"Network error: {ex.Message}";
            HasError = true;
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error: {ex}");
            ErrorMessage = $"Unexpected error: {ex.Message}";
            HasError = true;
            return null;
        }
    }
    
    public void StoreToken(string token)
    {
        // TODO: Implement secure token storage (e.g., SecureStorage)
    }
    
    public void ClearToken()
    {
        // TODO: Implement token clearing
    }
}

