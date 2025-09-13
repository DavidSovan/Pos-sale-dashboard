using System;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace PosSale.Models;
public class CheckoutResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("data")]
    public CheckoutData Data { get; set; } = new CheckoutData();
}

public class CheckoutData
{
    [JsonPropertyName("sale")]
    public Sale Sale { get; set; } = new Sale();
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class Payment
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("sale_id")]
    public int SaleId { get; set; }
    
    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;
    
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    
    [JsonPropertyName("change_given")]
    public decimal ChangeGiven { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class Cashier
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("role_id")]
    public int RoleId { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("email_verified_at")]
    public DateTime? EmailVerifiedAt { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}