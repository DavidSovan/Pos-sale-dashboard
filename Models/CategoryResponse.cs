using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PosSale.Models;
public class CategoryResponse
{
    [JsonPropertyName("data")]
    public List<Category> Data { get; set; } = new List<Category>();
}