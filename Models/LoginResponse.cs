
using System.Text.Json.Serialization;

namespace PosSale.Models;
public class LoginResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("data")]
    public LoginData Data { get; set; } = new LoginData();
}

public class LoginData
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "bearer";
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("user")]
    public User User { get; set; } = new User();
}