
using Avalonia.Controls;
using PosSale.ViewModels;

namespace PosSale.Views;
public partial class HomeView : Window
{
    public HomeViewModel ViewModel { get; } = new HomeViewModel();
    
    public HomeView()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }
}