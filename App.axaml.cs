
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using PosSale.ViewModels;
using PosSale.Views;

namespace PosSale;
public partial class App : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var loginView = new LoginView
            {
                DataContext = Program.ServiceProvider?.GetService<LoginViewModel>()
            };
            
            desktop.MainWindow = loginView;
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}