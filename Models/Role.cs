
using System.Text.Json.Serialization;

namespace PosSale.Models;
public class Role
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}