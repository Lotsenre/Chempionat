using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class AddEditProductDialogViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly bool _isEditMode;
    private readonly ProductDto _product;

    public event EventHandler<bool>? CloseRequested;

    [ObservableProperty]
    private string _productName = string.Empty;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private decimal _price;

    [ObservableProperty]
    private int _minStock = 10;

    [ObservableProperty]
    private string? _category;

    [ObservableProperty]
    private int _quantityAvailable;

    [ObservableProperty]
    private ObservableCollection<VendingMachineDto> _vendingMachines = new();

    [ObservableProperty]
    private VendingMachineDto? _selectedVendingMachine;

    [ObservableProperty]
    private bool _isBusy;

    public string DialogTitle => _isEditMode ? "Редактирование товара" : "Добавление товара";
    public string ButtonText => _isEditMode ? "Сохранить" : "Добавить";

    public AddEditProductDialogViewModel(IApiService apiService, ProductDto? product = null)
    {
        _apiService = apiService;
        _isEditMode = product != null;
        _product = product ?? new ProductDto { Id = Guid.NewGuid() };

        if (_isEditMode)
        {
            ProductName = _product.Name;
            Description = _product.Description;
            Price = _product.Price;
            MinStock = _product.MinStock;
            Category = _product.Category;
            QuantityAvailable = _product.QuantityAvailable;
        }

        LoadVendingMachinesAsync();
    }

    private async void LoadVendingMachinesAsync()
    {
        try
        {
            var machines = await _apiService.GetAsync<List<VendingMachineDto>>("vendingmachines");
            if (machines != null)
            {
                VendingMachines = new ObservableCollection<VendingMachineDto>(machines);
                if (_isEditMode && _product.VendingMachineId.HasValue)
                {
                    SelectedVendingMachine = VendingMachines.FirstOrDefault(v => v.Id == _product.VendingMachineId);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load machines error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(ProductName))
        {
            System.Windows.MessageBox.Show("Укажите название товара", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (Price <= 0)
        {
            System.Windows.MessageBox.Show("Цена должна быть больше 0", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        IsBusy = true;
        try
        {
            if (_isEditMode)
            {
                var request = new UpdateProductRequest
                {
                    Name = ProductName,
                    Description = Description,
                    Price = Price,
                    MinStock = MinStock,
                    Category = Category,
                    VendingMachineId = SelectedVendingMachine?.Id,
                    QuantityAvailable = QuantityAvailable
                };
                await _apiService.PutAsync<UpdateProductRequest, object>($"products/{_product.Id}", request);
            }
            else
            {
                var request = new CreateProductRequest
                {
                    Name = ProductName,
                    Description = Description,
                    Price = Price,
                    MinStock = MinStock,
                    Category = Category,
                    VendingMachineId = SelectedVendingMachine?.Id,
                    QuantityAvailable = QuantityAvailable
                };
                await _apiService.PostAsync<CreateProductRequest, ProductDto>("products", request);
            }

            CloseRequested?.Invoke(this, true);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseRequested?.Invoke(this, false);
    }
}
