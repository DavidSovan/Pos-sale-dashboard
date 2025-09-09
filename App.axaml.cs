
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using PosSale.Services;
using PosSale.ViewModels;
using PosSale.Views;

namespace PosSale;
public partial class App : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Check if we have a stored token to decide which window to show
            var authService = Program.ServiceProvider?.GetService<IAuthService>();
            var token = authService?.GetToken();
            
            if (!string.IsNullOrEmpty(token))
            {
                // User is already logged in, show home screen
                var homeView = new HomeView
                {
                    DataContext = Program.ServiceProvider?.GetService<HomeViewModel>()
                };
                
                desktop.MainWindow = homeView;
            }
            else
            {
                // Show login screen
                var loginView = new LoginView
                {
                    DataContext = Program.ServiceProvider?.GetService<LoginViewModel>()
                };
                
                desktop.MainWindow = loginView;
            }
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}