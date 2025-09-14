
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using PosSale.Models;
using PosSale.Services;

namespace PosSale.ViewModels;
public class ReceiptViewModel : ViewModelBase
{
    private readonly IProductService _productService;
    private readonly IAuthService _authService;
    private readonly ISaleService _saleService;
    private readonly int _saleId;
    
    private bool _isLoading;
    private string _errorMessage = string.Empty;
    private ReceiptData _receiptData = new ReceiptData();
    
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
    
    public ReceiptData ReceiptData
    {
        get => _receiptData;
        set => this.RaiseAndSetIfChanged(ref _receiptData, value);
    }
    
    public ObservableCollection<ReceiptItem> Items { get; } = new ObservableCollection<ReceiptItem>();
    
    public ReactiveCommand<Unit, Unit> LoadReceiptCommand { get; }
    public ReactiveCommand<Unit, Unit> NewSaleCommand { get; }
    
    // Interaction to navigate directly to a new Sale screen with the created saleId
    public Interaction<int, Unit> NavigateToSale { get; } = new Interaction<int, Unit>();
    
    public ReceiptViewModel(int saleId, IProductService productService, IAuthService authService, ISaleService saleService)
    {
        _saleId = saleId;
        _productService = productService;
        _authService = authService;
        _saleService = saleService;
        
        LoadReceiptCommand = ReactiveCommand.CreateFromTask(LoadReceipt);
        // Start a brand-new sale and navigate directly to SaleView
        NewSaleCommand = ReactiveCommand.CreateFromTask(StartNewSaleAsync);

        // Prevent unhandled exceptions from crashing the app
        LoadReceiptCommand.ThrownExceptions.Subscribe(ex =>
        {
            IsLoading = false;
            ErrorMessage = $"Error loading receipt: {ex.Message}";
        });
        NewSaleCommand.ThrownExceptions.Subscribe(ex =>
        {
            IsLoading = false;
            ErrorMessage = $"New sale error: {ex.Message}";
        });
        
        // Load receipt data automatically
        LoadReceiptCommand.Execute().Subscribe();
    }
    
    private async Task LoadReceipt()
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
            
            var response = await _productService.GetReceiptAsync(_saleId, token);
            
            if (response.Status == "success")
            {
                ReceiptData = response.Data;
                
                // Populate items collection
                Items.Clear();
                foreach (var item in response.Data.Items)
                {
                    Items.Add(item);
                }
            }
            else
            {
                ErrorMessage = "Failed to load receipt details";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading receipt: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private async Task StartNewSaleAsync()
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
            
            // Add a timeout to avoid hanging indefinitely
            var startTask = _saleService.StartSaleAsync(token);
            var completed = await Task.WhenAny(startTask, Task.Delay(TimeSpan.FromSeconds(15)));
            if (completed != startTask)
            {
                ErrorMessage = "Starting a new sale timed out. Please try again.";
                return;
            }
            var response = await startTask;
            if (response.Status == "success")
            {
                await NavigateToSale.Handle(response.Data.SaleId);
            }
            else
            {
                ErrorMessage = "Failed to start a new sale.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error starting new sale: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}