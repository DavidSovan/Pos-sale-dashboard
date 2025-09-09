using Avalonia.Controls;
using Avalonia.ReactiveUI;
using PosSale.ViewModels;
using ReactiveUI;

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
                d(ViewModel.NavigateToSale.RegisterHandler(async interaction =>
                {
                    var saleId = interaction.Input;
                    var saleView = new SaleView(saleId);
                    saleView.Show();
                    this.Close();
                }));
            }
        });
    }
}
