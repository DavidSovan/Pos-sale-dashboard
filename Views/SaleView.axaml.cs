using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using PosSale.ViewModels;
using PosSale.Models;
using PosSale.Services;
using System;
using ReactiveUI;
using System.Reactive;

namespace PosSale.Views;
public partial class SaleView : ReactiveWindow<SaleViewModel>
{
    private readonly IProductService _productService;
    private readonly IAuthService _authService;
    
    public SaleView(int saleId, IProductService productService, IAuthService authService)
    {
        InitializeComponent();
        _productService = productService;
        _authService = authService;
        ViewModel = new SaleViewModel(saleId, productService, authService);
        DataContext = ViewModel;
        
        this.WhenActivated(d =>
        {
            if (ViewModel != null)
            {
                d(ViewModel.NavigateToReceipt.RegisterHandler(async interaction =>
                {
                    var receiptView = new ReceiptView(interaction.Input, _productService, _authService);
                    receiptView.Show();
                    this.Close();
                    interaction.SetOutput(Unit.Default);
                }));
            }
        });
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