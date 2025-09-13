using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using PosSale.ViewModels;
using PosSale.Models;
using PosSale.Services;
using System;

namespace PosSale.Views;
public partial class SaleView : ReactiveWindow<SaleViewModel>
{
    public SaleView(int saleId, IProductService productService, IAuthService authService)
    {
        InitializeComponent();
        ViewModel = new SaleViewModel(saleId, productService, authService);
        DataContext = ViewModel;
    }
    
    private void OnSearchKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ViewModel?.SearchCommand.Execute().Subscribe(
                onNext: result => { },
                onError: ex => { },
                onCompleted: () => { });
        }
    }
    
    private void OnQuantityLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox && textBox.Tag is SaleItem item)
        {
            ViewModel?.UpdateItemCommand.Execute(item).Subscribe(
                onNext: result => { },
                onError: ex => { },
                onCompleted: () => { });
        }
    }
}