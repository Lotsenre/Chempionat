using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VendingMachineDesktop.Models;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class AddEditContractDialogViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly bool _isEditMode;
    private readonly ContractDto _contract;

    public event EventHandler<bool>? CloseRequested;

    [ObservableProperty]
    private string? _contractNumber;

    [ObservableProperty]
    private DateTime _startDate = DateTime.Now;

    [ObservableProperty]
    private DateTime _endDate = DateTime.Now.AddYears(1);

    [ObservableProperty]
    private decimal _monthlyRent;

    [ObservableProperty]
    private decimal _yearlyRent;

    [ObservableProperty]
    private bool _insuranceRequired;

    [ObservableProperty]
    private string? _managementType;

    [ObservableProperty]
    private ObservableCollection<Company> _companies = new();

    [ObservableProperty]
    private Company? _selectedCompany;

    [ObservableProperty]
    private ObservableCollection<VendingMachineDto> _vendingMachines = new();

    [ObservableProperty]
    private VendingMachineDto? _selectedVendingMachine;

    [ObservableProperty]
    private bool _isBusy;

    public ObservableCollection<string> ManagementTypes { get; } = new() { "Full", "Partial", "Self" };

    public string DialogTitle => _isEditMode ? "Редактирование контракта" : "Добавление контракта";
    public string ButtonText => _isEditMode ? "Сохранить" : "Добавить";

    public AddEditContractDialogViewModel(IApiService apiService, ContractDto? contract = null)
    {
        _apiService = apiService;
        _isEditMode = contract != null;
        _contract = contract ?? new ContractDto { Id = Guid.NewGuid() };

        if (_isEditMode)
        {
            ContractNumber = _contract.ContractNumber;
            StartDate = _contract.StartDate;
            EndDate = _contract.EndDate;
            MonthlyRent = _contract.MonthlyRent;
            YearlyRent = _contract.YearlyRent;
            InsuranceRequired = _contract.InsuranceRequired;
            ManagementType = _contract.ManagementType;
        }

        LoadDataAsync();
    }

    private async void LoadDataAsync()
    {
        try
        {
            var companies = await _apiService.GetAsync<List<Company>>("companies");
            if (companies != null)
            {
                Companies = new ObservableCollection<Company>(companies);
                if (_isEditMode && _contract.CompanyId.HasValue)
                    SelectedCompany = Companies.FirstOrDefault(c => c.Id == _contract.CompanyId);
            }

            var machines = await _apiService.GetAsync<List<VendingMachineDto>>("vendingmachines");
            if (machines != null)
            {
                VendingMachines = new ObservableCollection<VendingMachineDto>(machines);
                if (_isEditMode)
                    SelectedVendingMachine = VendingMachines.FirstOrDefault(v => v.Id == _contract.VendingMachineId);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load data error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (SelectedVendingMachine == null)
        {
            System.Windows.MessageBox.Show("Выберите торговый автомат", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        IsBusy = true;
        try
        {
            if (_isEditMode)
            {
                var request = new UpdateContractRequest
                {
                    StartDate = StartDate,
                    EndDate = EndDate,
                    MonthlyRent = MonthlyRent,
                    YearlyRent = YearlyRent,
                    InsuranceRequired = InsuranceRequired,
                    ManagementType = ManagementType
                };
                await _apiService.PutAsync<UpdateContractRequest, object>($"contracts/{_contract.Id}", request);
            }
            else
            {
                var request = new CreateContractRequest
                {
                    ContractNumber = ContractNumber,
                    CompanyId = SelectedCompany?.Id,
                    VendingMachineId = SelectedVendingMachine.Id,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    MonthlyRent = MonthlyRent,
                    YearlyRent = YearlyRent,
                    InsuranceRequired = InsuranceRequired,
                    ManagementType = ManagementType
                };
                await _apiService.PostAsync<CreateContractRequest, ContractDto>("contracts", request);
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
