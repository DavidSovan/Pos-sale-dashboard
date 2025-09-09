
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using PosSale.ViewModels;

namespace PosSale.Views;
public partial class SaleView : ReactiveWindow<SaleViewModel>
{
    public SaleView(int saleId)
    {
        InitializeComponent();
        ViewModel = new SaleViewModel(saleId);
        DataContext = ViewModel;
    }
}