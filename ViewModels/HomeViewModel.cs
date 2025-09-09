
using ReactiveUI;
using PosSale.Models;

namespace PosSale.ViewModels;
public class HomeViewModel : ViewModelBase
{
    private string _userName = string.Empty;
    private string _userRole = string.Empty;
    
    public string UserName
    {
        get => _userName;
        set => this.RaiseAndSetIfChanged(ref _userName, value);
    }
    
    public string UserRole
    {
        get => _userRole;
        set => this.RaiseAndSetIfChanged(ref _userRole, value);
    }
    
    public void Initialize(User user)
    {
        UserName = user.Name;
        UserRole = user.Role.Name;
    }
}