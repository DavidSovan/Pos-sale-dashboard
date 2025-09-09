
using ReactiveUI;

namespace PosSale.ViewModels;
public class SaleViewModel : ViewModelBase
{
    private int _saleId;
    
    public string SaleIdText => $"This is Sale Screen (saleId={_saleId})";
    
    public SaleViewModel(int saleId)
    {
        _saleId = saleId;
    }
}