
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PosSale.Models;
public class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("role")]
    public Role Role { get; set; } = new Role();
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("permissions")]
    public List<string> Permissions { get; set; } = new List<string>();
}