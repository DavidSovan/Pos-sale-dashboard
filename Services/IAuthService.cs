using System.Threading.Tasks;
using PosSale.Models;

namespace PosSale.Services;
public interface IAuthService
{
    Task<LoginResponse> LoginAsync(string email, string password);
    void SaveToken(string token);
    string GetToken();
    void ClearToken();
}