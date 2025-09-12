using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using PosSale.Models;
using PosSale.Services;
using Avalonia.Threading;

namespace PosSale.ViewModels;
public class SaleViewModel : ViewModelBase
{
    private readonly IProductService _productService;
    private readonly IAuthService _authService;
    private readonly int _saleId;
    
    private string _searchText = string.Empty;
    private int? _selectedCategoryId;
    private int _currentPage = 1;
    private int _totalPages = 1;
    private bool _isLoading;
    private string _errorMessage = string.Empty;
    private Sale _sale = new Sale();
    
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }
    
    public int? SelectedCategoryId
    {
        get => _selectedCategoryId;
        set => this.RaiseAndSetIfChanged(ref _selectedCategoryId, value);
    }
    
    public int CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }
    
    public int TotalPages
    {
        get => _totalPages;
        set => this.RaiseAndSetIfChanged(ref _totalPages, value);
    }
    
    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }
    
    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }
    
    public Sale Sale
    {
        get => _sale;
        set => this.RaiseAndSetIfChanged(ref _sale, value);
    }
    
    public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();
    public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();
    
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    public ReactiveCommand<Unit, Unit> NextPageCommand { get; }
    public ReactiveCommand<Unit, Unit> PreviousPageCommand { get; }
    public ReactiveCommand<Product, Unit> AddProductCommand { get; }
    
    public SaleViewModel(int saleId, IProductService productService, IAuthService authService)
    {
        _saleId = saleId;
        _productService = productService;
        _authService = authService;
        
        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadData);
        SearchCommand = ReactiveCommand.CreateFromTask(SearchProducts);
        NextPageCommand = ReactiveCommand.CreateFromTask(GoToNextPage);
        PreviousPageCommand = ReactiveCommand.CreateFromTask(GoToPreviousPage);
        AddProductCommand = ReactiveCommand.CreateFromTask<Product>(AddProductToSale);
        
        // Load initial data
        LoadDataCommand.Execute().Subscribe();
    }
    
    private async Task LoadData()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            
            // Load categories
            var categories = await _productService.GetCategoriesAsync();
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            });
            
            // Load products
            await LoadProducts();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading data: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private async Task LoadProducts()
    {
        try
        {
            ErrorMessage = string.Empty;
            var response = await _productService.GetProductsAsync(SelectedCategoryId, SearchText, CurrentPage);
            
            var products = response?.Data?.Products;
            if (products is not null)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Products.Clear();
                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }

                    var lastPage = response?.Data?.Pagination?.LastPage;
                    TotalPages = lastPage > 0 ? lastPage.Value : 1;
                });
            }
            else
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Products.Clear();
                    ErrorMessage = "No products found.";
                    TotalPages = 1;
                });
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading products: {ex.Message}";
        }
    }
    
    private async Task SearchProducts()
    {
        CurrentPage = 1;
        await LoadProducts();
    }
    
    private async Task GoToNextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadProducts();
        }
    }
    
    private async Task GoToPreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadProducts();
        }
    }
    
    private async Task AddProductToSale(Product product)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            
            var token = _authService.GetToken();
            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "Not authenticated. Please log in again.";
                return;
            }
            
            var response = await _productService.AddItemToSaleAsync(_saleId, product.Id, 1, token);
            
            if (response.Status == "success")
            {
                // Update the sale with the latest data
                Sale = response.Data.Sale;
            }
            else
            {
                ErrorMessage = "Failed to add product to sale";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error adding product to sale: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}