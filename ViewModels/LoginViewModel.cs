using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PosSale.Models;
using PosSale.Services;

namespace PosSale.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty] 
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    private bool _hasError;
    
    private readonly HttpClient _httpClient;
    private readonly INavigationService _navigationService;
    private readonly AuthService _authService;
    
    public LoginViewModel(HttpClient httpClient, INavigationService navigationService)
    {
        _httpClient = httpClient;
        _navigationService = navigationService;
        _authService = new AuthService(httpClient);
    }
    
    [RelayCommand]
    private async Task Login()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            
            var result = await _authService.LoginAsync(Email, Password);
            
            if (result?.Data?.AccessToken != null && result.Data.User != null)
            {
                _authService.StoreToken(result.Data.AccessToken);
                _navigationService.NavigateToHome(result.Data.User);
            }
            else
            {
                ErrorMessage = "Invalid email or password";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "An error occurred: " + ex.Message;
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
