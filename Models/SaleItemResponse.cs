using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PosSale.Models;
public class SaleItemResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("data")]
    public SaleItemData Data { get; set; } = new SaleItemData();
}

public class SaleItemData
{
    [JsonPropertyName("sale")]
    public Sale Sale { get; set; } = new Sale();
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class Sale
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("cashier_id")]
    public int CashierId { get; set; }
    
    [JsonPropertyName("total_amount")]
    public decimal TotalAmount { get; set; }
    
    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    [JsonPropertyName("sale_items")]
    public List<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}

public class SaleItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("sale_id")]
    public int SaleId { get; set; }
    
    [JsonPropertyName("product_id")]
    public int ProductId { get; set; }
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }
    
    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    [JsonPropertyName("product")]
    public Product Product { get; set; } = new Product();
}