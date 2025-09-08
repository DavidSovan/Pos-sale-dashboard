using PosSale.Models;

namespace PosSale.Services;

public interface INavigationService
{
    void NavigateToHome(User user);
    void NavigateToLogin();
}
