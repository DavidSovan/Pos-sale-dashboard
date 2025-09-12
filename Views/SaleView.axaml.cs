using System;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using PosSale.Services;
using PosSale.ViewModels;
using Avalonia.Input;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using PosSale.Models;

namespace PosSale.Views;
public partial class SaleView : ReactiveWindow<SaleViewModel>
{
    public SaleView()
    {
        // This constructor is needed for the XAML previewer.
    }

    public SaleView(int saleId)
    {
        InitializeComponent();
        var productService = Program.ServiceProvider?.GetService<IProductService>();
        var authService = Program.ServiceProvider?.GetService<IAuthService>();
        ViewModel = new SaleViewModel(saleId, productService!, authService!);
        DataContext = ViewModel;
    }

    public SaleView(int saleId, IProductService productService, IAuthService authService)
    {
        InitializeComponent();
        ViewModel = new SaleViewModel(saleId, productService, authService);
        DataContext = ViewModel;
    }

    private void OnSearchKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && ViewModel is not null)
        {
            var observer = Observer.Create<Unit>(_ => { });
            ViewModel.SearchCommand.Execute().Subscribe(observer);
            e.Handled = true;
        }
    }

    private void OnCategorySelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ViewModel is null) return;
        if (sender is ComboBox combo)
        {
            var selected = combo.SelectedItem as Category;
            ViewModel.SelectedCategoryId = selected?.Id;
            // Refresh products for the selected category
            var observer = Observer.Create<Unit>(_ => { });
            ViewModel.SearchCommand.Execute().Subscribe(observer);
        }
    }
}