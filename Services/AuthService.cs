
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using PosSale.Models;

namespace PosSale.Services;
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private string _accessToken = string.Empty;
    
    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<LoginResponse> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };
            
            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/auth/login", content);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Login failed with status code: {response.StatusCode}");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, options);
            
            if (loginResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize login response");
            }
            
            if (loginResponse.Status == "success" && loginResponse.Data != null)
            {
                SaveToken(loginResponse.Data.AccessToken);
            }
            
            return loginResponse;
        }
        catch (Exception ex)
        {
            throw new Exception($"Login error: {ex.Message}", ex);
        }
    }
    
    public void SaveToken(string token)
    {
        _accessToken = token;
        // For persistence across app restarts, you could add local storage here
        // e.g., using ISecureStorage in Avalonia
    }
    
    public string GetToken()
    {
        return _accessToken;
    }
    
    public void ClearToken()
    {
        _accessToken = string.Empty;
    }
}