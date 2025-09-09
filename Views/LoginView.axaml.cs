using System.Reactive;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using PosSale.ViewModels;
using ReactiveUI;

namespace PosSale.Views;
public partial class LoginView : ReactiveWindow<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
        this.WhenActivated(d => 
        {
            if (ViewModel != null)
            {
                d(ViewModel.LoginSuccessful.RegisterHandler(interaction =>
                {
                    var homeViewModel = Program.ServiceProvider?.GetService<HomeViewModel>();
                    var homeView = new HomeView
                    {
                        DataContext = homeViewModel
                    };
                    homeViewModel?.Initialize(interaction.Input.Data.User);
                    
                    this.Hide();
                    homeView.Show();
                    this.Close();
                    
                    interaction.SetOutput(Unit.Default);
                }));
            }
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}