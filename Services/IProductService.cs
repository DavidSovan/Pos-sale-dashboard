using System.Collections.Generic;
using System.Threading.Tasks;
using PosSale.Models;

namespace PosSale.Services;
public interface IProductService
{
        Task<List<Category>> GetCategoriesAsync();
    Task<ProductResponse> GetProductsAsync(int? categoryId = null, string? search = null, int page = 1);
    Task<SaleItemResponse> AddItemToSaleAsync(int saleId, int productId, int quantity, string token);
    Task<SaleItemResponse> UpdateSaleItemAsync(int saleId, int itemId, int quantity, string token);
    Task<SaleItemResponse> RemoveSaleItemAsync(int saleId, int itemId, string token);
    Task<CheckoutResponse> CheckoutSaleAsync(int saleId, string paymentMethod, decimal amount, string token);
}