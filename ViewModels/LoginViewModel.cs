
using System;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using PosSale.Services;
using PosSale.Models;

namespace PosSale.ViewModels;
public class LoginViewModel : ViewModelBase
{
    private string _email = "david@gmail.com";
    private string _password = "David@123";
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private readonly IAuthService _authService;
    
    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }
    
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
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
    
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public Interaction<LoginResponse, Unit> LoginSuccessful { get; } = new Interaction<LoginResponse, Unit>();
    
    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        
        var canLogin = this.WhenAnyValue(
            x => x.Email,
            x => x.Password,
            x => x.IsLoading,
            (email, password, isLoading) => 
                !isLoading && 
                !string.IsNullOrWhiteSpace(email) && 
                !string.IsNullOrWhiteSpace(password));
        
        LoginCommand = ReactiveCommand.CreateFromTask(
            ExecuteLogin,
            canLogin);
    }
    
    private async Task ExecuteLogin()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            
            var response = await _authService.LoginAsync(Email, Password);
            
            if (response.Status == "success")
            {
                await LoginSuccessful.Handle(response);
            }
            else
            {
                ErrorMessage = "Login failed. Please check your credentials.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}