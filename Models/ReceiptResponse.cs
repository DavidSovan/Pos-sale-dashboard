
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PosSale.Models;
public class ReceiptResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("data")]
    public ReceiptData Data { get; set; } = new ReceiptData();
}

public class ReceiptData
{
    [JsonPropertyName("sale")]
    public ReceiptSale Sale { get; set; } = new ReceiptSale();
    
    [JsonPropertyName("items")]
    public List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
    
    [JsonPropertyName("payment")]
    public ReceiptPayment Payment { get; set; } = new ReceiptPayment();
    
    [JsonPropertyName("cashier")]
    public ReceiptCashier Cashier { get; set; } = new ReceiptCashier();
}

public class ReceiptSale
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; } = string.Empty;
    
    [JsonPropertyName("total_amount")]
    public decimal TotalAmount { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}

public class ReceiptItem
{
    [JsonPropertyName("product_id")]
    public int ProductId { get; set; }
    
    [JsonPropertyName("product_name")]
    public string ProductName { get; set; } = string.Empty;
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }
    
    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }
}

public class ReceiptPayment
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;
    
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    
    [JsonPropertyName("change_given")]
    public decimal ChangeGiven { get; set; }
}

public class ReceiptCashier
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}