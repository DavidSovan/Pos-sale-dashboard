using System;
using System.Collections.ObjectModel;
using System.Linq;
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
    private Category? _selectedCategory;
    private int _currentPage = 1;
    private int _totalPages = 1;
    private bool _isLoading;
    private string _errorMessage = string.Empty;
    private Sale _sale = new Sale();
    private decimal _discount;
    private string _paymentMethod = "cash";
    private decimal _paymentAmount;
    
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
    
    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedCategory, value);
            SelectedCategoryId = value?.Id;
            CurrentPage = 1;
            _ = LoadProducts();
        }
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
        set
        {
            this.RaiseAndSetIfChanged(ref _sale, value);
            // Keep CartItems synchronized with Sale.SaleItems
            SyncCartItems();
            // Update computed totals
            this.RaisePropertyChanged(nameof(Subtotal));
            this.RaisePropertyChanged(nameof(Total));
        }
    }
    
    public decimal Discount
    {
        get => _discount;
        set => this.RaiseAndSetIfChanged(ref _discount, value);
    }
    
    public string PaymentMethod
    {
        get => _paymentMethod;
        set => this.RaiseAndSetIfChanged(ref _paymentMethod, value);
    }
    
    public decimal PaymentAmount
    {
        get => _paymentAmount;
        set => this.RaiseAndSetIfChanged(ref _paymentAmount, value);
    }
    
    public decimal Subtotal => Sale.SaleItems.Sum(item => item.Subtotal);
    public decimal Total => Subtotal - Discount;
    
    public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();
    public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();
    public ObservableCollection<SaleItem> CartItems { get; } = new ObservableCollection<SaleItem>();
    public ObservableCollection<string> PaymentMethods { get; } = new ObservableCollection<string>
    {
        "cash",
        "card",
        "digital"
    };
    
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    public ReactiveCommand<Unit, Unit> NextPageCommand { get; }
    public ReactiveCommand<Unit, Unit> PreviousPageCommand { get; }
    public ReactiveCommand<Product, Unit> AddProductCommand { get; }
    public ReactiveCommand<SaleItem, Unit> UpdateItemCommand { get; }
    public ReactiveCommand<SaleItem, Unit> RemoveItemCommand { get; }
    public ReactiveCommand<Unit, Unit> CheckoutCommand { get; }
    
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
        UpdateItemCommand = ReactiveCommand.CreateFromTask<SaleItem>(UpdateSaleItem);
        RemoveItemCommand = ReactiveCommand.CreateFromTask<SaleItem>(RemoveSaleItem);
        CheckoutCommand = ReactiveCommand.CreateFromTask(CheckoutSale);
        
        // Load initial data
        LoadDataCommand.Execute().Subscribe();
        
        // Update payment amount when total changes
        this.WhenAnyValue(x => x.Total).Subscribe(total =>
        {
            PaymentAmount = total;
        });
    }

    private void SyncCartItems()
    {
        // Ensure this runs on UI thread
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Post(SyncCartItems);
            return;
        }

        CartItems.Clear();
        if (Sale?.SaleItems != null)
        {
            foreach (var si in Sale.SaleItems)
            {
                CartItems.Add(si);
            }
        }
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
                // Update the sale with the latest data on the UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Sale = response.Data.Sale;
                    this.RaisePropertyChanged(nameof(Subtotal));
                    this.RaisePropertyChanged(nameof(Total));
                });
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
    
    private async Task UpdateSaleItem(SaleItem item)
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
            
            var response = await _productService.UpdateSaleItemAsync(_saleId, item.Id, item.Quantity, token);
            
            if (response.Status == "success")
            {
                // Update the sale with the latest data on the UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Sale = response.Data.Sale;
                    this.RaisePropertyChanged(nameof(Subtotal));
                    this.RaisePropertyChanged(nameof(Total));
                });
            }
            else
            {
                ErrorMessage = "Failed to update item quantity";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error updating item: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private async Task RemoveSaleItem(SaleItem item)
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
            
            var response = await _productService.RemoveSaleItemAsync(_saleId, item.Id, token);
            
            if (response.Status == "success")
            {
                // Update the sale with the latest data on the UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Sale = response.Data.Sale;
                    this.RaisePropertyChanged(nameof(Subtotal));
                    this.RaisePropertyChanged(nameof(Total));
                });
            }
            else
            {
                ErrorMessage = "Failed to remove item from sale";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error removing item: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private async Task CheckoutSale()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            
            if (Sale.SaleItems.Count == 0)
            {
                ErrorMessage = "Cannot checkout an empty sale";
                return;
            }
            
            if (PaymentAmount < Total)
            {
                ErrorMessage = "Payment amount is less than the total";
                return;
            }
            
            var token = _authService.GetToken();
            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "Not authenticated. Please log in again.";
                return;
            }
            
            var response = await _productService.CheckoutSaleAsync(_saleId, PaymentMethod, PaymentAmount, token);
            
            if (response.Status == "success")
            {
                // Show success message and potentially navigate to a receipt screen
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ErrorMessage = "Sale completed successfully!";
                    // Clear the sale after successful checkout
                    Sale = new Sale();
                    this.RaisePropertyChanged(nameof(Subtotal));
                    this.RaisePropertyChanged(nameof(Total));
                });
            }
            else
            {
                ErrorMessage = "Failed to checkout sale";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error during checkout: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}