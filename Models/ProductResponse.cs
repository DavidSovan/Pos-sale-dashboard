using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PosSale.Models;
public class ProductResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("data")]
    public ProductData Data { get; set; } = new ProductData();
}

public class ProductData
{
    [JsonPropertyName("products")]
    public List<Product> Products { get; set; } = new List<Product>();
    
    [JsonPropertyName("pagination")]
    public Pagination Pagination { get; set; } = new Pagination();
}

public class Pagination
{
    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }
    
    [JsonPropertyName("last_page")]
    public int LastPage { get; set; }
    
    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }
    
    [JsonPropertyName("total")]
    public int Total { get; set; }
}