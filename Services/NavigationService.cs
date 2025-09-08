using Avalonia.Controls;
using System.Net.Http;
using PosSale.Models;
using PosSale.ViewModels;
using PosSale.Views;

namespace PosSale.Services;

public class NavigationService : INavigationService
{
    private readonly MainWindow _mainWindow;
    private readonly HttpClient _httpClient;
    
    public NavigationService(MainWindow mainWindow, HttpClient httpClient)
    {
        _mainWindow = mainWindow;
        _httpClient = httpClient;
    }
    
    public void NavigateToHome(User user)
    {
        var homeViewModel = new HomeViewModel(user);
        _mainWindow.Content = new HomeView { DataContext = homeViewModel };
    }
    
    public void NavigateToLogin()
    {
        var loginViewModel = new LoginViewModel(
            _httpClient,
            this);
        _mainWindow.Content = new LoginView { DataContext = loginViewModel };
    }
}
