
using System.Text.Json.Serialization;

namespace PosSale.Models;
public class SaleItemRequest
{
    [JsonPropertyName("product_id")]
    public int ProductId { get; set; }
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 1;
}