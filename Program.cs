// Program.cs
using System;
using System.Net.Http;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using PosSale.Services;
using PosSale.ViewModels;
using PosSale.Views;

namespace PosSale;
internal class Program
{
    public static IServiceProvider? ServiceProvider { get; private set; }
    
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
            
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            // Log the exception here
            Console.WriteLine($"Application failed to start: {ex}");
        }
    }
    
    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<HttpClient>(_ => new HttpClient 
        { 
            BaseAddress = new Uri("http://127.0.0.1:8000") 
        });
        services.AddSingleton<IAuthService, AuthService>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<HomeViewModel>();
    }
    
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}