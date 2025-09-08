using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PosSale.Services;
using PosSale.ViewModels;
using PosSale.Views;
using System.Net.Http;
using System;

namespace PosSale;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();
            
            services.AddSingleton<HttpClient>(sp => 
            {
                var baseAddress = new Uri("http://127.0.0.1:8000");
                var client = new HttpClient()
                {
                    BaseAddress = baseAddress
                };
                Console.WriteLine($"HttpClient configured with BaseAddress: {client.BaseAddress}");
                return client;
            });
            services.AddSingleton<MainWindow>();
            services.AddSingleton<INavigationService, NavigationService>();
            
            var serviceProvider = services.BuildServiceProvider();
            
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            var navigationService = serviceProvider.GetRequiredService<INavigationService>();
            
            navigationService.NavigateToLogin();
            
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}