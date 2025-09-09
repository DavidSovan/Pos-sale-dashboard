
using PosSale.Models;
using PosSale.Services;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PosSale.ViewModels;
public class HomeViewModel : ViewModelBase
{
    private string _userName = string.Empty;
    private string _userRole = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private readonly IAuthService _authService;
    private readonly ISaleService _saleService;

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

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public ReactiveCommand<Unit, Unit> StartSaleCommand { get; }

    // Interaction to navigate to the sale screen with the saleId
    public Interaction<int, Unit> NavigateToSale { get; } = new Interaction<int, Unit>();

    public HomeViewModel(IAuthService authService, ISaleService saleService)
    {
        _authService = authService;
        _saleService = saleService;
        
        StartSaleCommand = ReactiveCommand.CreateFromTask(ExecuteStartSale);
    }
    
    public void Initialize(User user)
    {
        UserName = user.Name;
        UserRole = user.Role.Name;
    }
    
    private async Task ExecuteStartSale()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            
            // Get the token from AuthService
            var token = _authService.GetToken();
            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "Not authenticated. Please log in again.";
                return;
            }
            
            var response = await _saleService.StartSaleAsync(token);
            
            if (response.Status == "success")
            {
                // Navigate to SaleScreen with saleId
                await NavigateToSale.Handle(response.Data.SaleId);
            }
            else
            {
                ErrorMessage = "Failed to start sale.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error starting sale: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}