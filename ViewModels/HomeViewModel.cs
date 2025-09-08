using CommunityToolkit.Mvvm.ComponentModel;
using PosSale.Models;

namespace PosSale.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _welcomeMessage;
    
    [ObservableProperty]
    private string _userRole;
    
    public HomeViewModel(User? user)
    {
        WelcomeMessage = $"Hello, {user?.Name ?? "User"}!";
        UserRole = $"You are logged in as: {user?.Role?.Name ?? "Unknown Role"}";
    }
}
