using Avalonia.Controls;
using Avalonia.ReactiveUI;
using PosSale.ViewModels;
using ReactiveUI;
using PosSale.Services;
using Microsoft.Extensions.DependencyInjection;

namespace PosSale.Views;
public partial class HomeView : ReactiveWindow<HomeViewModel>
{
    public HomeView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel != null)
            {
                d(ViewModel.NavigateToSale.RegisterHandler(interaction =>
                {
                    var saleId = interaction.Input;
                    var productService = Program.ServiceProvider?.GetService<IProductService>();
                    var authService = Program.ServiceProvider?.GetService<IAuthService>();
                    
                    if (productService != null && authService != null)
                    {
                        var saleView = new SaleView(saleId, productService, authService);
                        saleView.Show();
                        this.Close();
                    }
                }));
            }
        });
    }
}
