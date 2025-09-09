
using System.Text.Json.Serialization;

namespace PosSale.Models;
public class LoginRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}