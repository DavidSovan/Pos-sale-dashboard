using Avalonia.Controls;
using Avalonia.ReactiveUI;
using PosSale.ViewModels;
using ReactiveUI;
using PosSale.Services;

namespace PosSale.Views;
public partial class ReceiptView : ReactiveWindow<ReceiptViewModel>
{
    public ReceiptView(int saleId, IProductService productService, IAuthService authService)
    {
        InitializeComponent();
        ViewModel = new ReceiptViewModel(saleId, productService, authService);
        DataContext = ViewModel;
        
        this.WhenActivated(d => 
        {
            if (ViewModel != null)
            {
                d(ViewModel.NavigateToHome.RegisterHandler(async interaction =>
                {
                    var homeView = new HomeView();
                    homeView.Show();
                    this.Close();
                }));
            }
        });
    }
}