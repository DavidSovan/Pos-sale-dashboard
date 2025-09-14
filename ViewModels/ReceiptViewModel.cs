
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
    
    // Interaction to navigate back to home screen
    public Interaction<Unit, Unit> NavigateToHome { get; } = new Interaction<Unit, Unit>();
    
    public ReceiptViewModel(int saleId, IProductService productService, IAuthService authService)
    {
        _saleId = saleId;
        _productService = productService;
        _authService = authService;
        
        LoadReceiptCommand = ReactiveCommand.CreateFromTask(LoadReceipt);
        NewSaleCommand = ReactiveCommand.CreateFromTask(StartNewSale);
        
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
    
    private async Task StartNewSale()
    {
        await NavigateToHome.Handle(Unit.Default);
    }
}