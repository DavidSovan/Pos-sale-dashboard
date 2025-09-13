using System.Text.Json.Serialization;

namespace PosSale.Models;
public class CheckoutRequest
{
    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; } = "cash";
    
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}