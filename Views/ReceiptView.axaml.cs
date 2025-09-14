using Avalonia.Controls;
using Avalonia.ReactiveUI;
using PosSale.ViewModels;
using ReactiveUI;
using PosSale.Services;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;

namespace PosSale.Views;
public partial class ReceiptView : ReactiveWindow<ReceiptViewModel>
{
    private readonly IProductService _productService;
    private readonly IAuthService _authService;
    private readonly ISaleService _saleService;
    
    public ReceiptView(int saleId, IProductService productService, IAuthService authService)
    {
        InitializeComponent();
        _productService = productService;
        _authService = authService;
        _saleService = Program.ServiceProvider?.GetService<ISaleService>()!;
        
        ViewModel = new ReceiptViewModel(saleId, _productService, _authService, _saleService);
        DataContext = ViewModel;
        
        this.WhenActivated(d => 
        {
            if (ViewModel != null)
            {
                d(ViewModel.NavigateToSale.RegisterHandler(async interaction =>
                {
                    var saleView = new SaleView(interaction.Input, _productService, _authService);
                    if (Application.Current?.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.MainWindow = saleView;
                    }
                    saleView.Show();
                    this.Close();
                    interaction.SetOutput(System.Reactive.Unit.Default);
                }));
            }
        });
    }
}