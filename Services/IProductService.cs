using System.Collections.Generic;
using System.Threading.Tasks;
using PosSale.Models;

namespace PosSale.Services;
public interface IProductService
{
    Task<List<Category>> GetCategoriesAsync();
    Task<ProductResponse> GetProductsAsync(int? categoryId = null, string? search = null, int page = 1);
    Task<SaleItemResponse> AddItemToSaleAsync(int saleId, int productId, int quantity, string token);
}