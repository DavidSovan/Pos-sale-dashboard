
using System.Text.Json.Serialization;

namespace PosSale.Models;
public class UpdateItemRequest
{
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}