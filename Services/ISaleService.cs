
using PosSale.Models;
using System.Threading.Tasks;

namespace PosSale.Services;
public interface ISaleService
{
    Task<StartSaleResponse> StartSaleAsync(string token);
}