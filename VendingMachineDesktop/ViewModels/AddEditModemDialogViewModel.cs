using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class AddEditModemDialogViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly bool _isEditMode;
    private readonly ModemDto _modem;

    public event EventHandler<bool>? CloseRequested;

    [ObservableProperty]
    private string _modemNumber = string.Empty;

    [ObservableProperty]
    private string? _model;

    [ObservableProperty]
    private string _selectedStatus = "Active";

    [ObservableProperty]
    private ObservableCollection<VendingMachineDto> _vendingMachines = new();

    [ObservableProperty]
    private VendingMachineDto? _selectedVendingMachine;

    [ObservableProperty]
    private bool _isBusy;

    public ObservableCollection<string> Statuses { get; } = new() { "Active", "Inactive", "Maintenance" };

    public string DialogTitle => _isEditMode ? "Редактирование модема" : "Добавление модема";
    public string ButtonText => _isEditMode ? "Сохранить" : "Добавить";

    public AddEditModemDialogViewModel(IApiService apiService, ModemDto? modem = null)
    {
        _apiService = apiService;
        _isEditMode = modem != null;
        _modem = modem ?? new ModemDto { Id = Guid.NewGuid() };

        if (_isEditMode)
        {
            ModemNumber = _modem.ModemNumber;
            Model = _modem.Model;
            SelectedStatus = _modem.Status;
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
                if (_isEditMode && _modem.VendingMachineId.HasValue)
                {
                    SelectedVendingMachine = VendingMachines.FirstOrDefault(v => v.Id == _modem.VendingMachineId);
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
        if (string.IsNullOrWhiteSpace(ModemNumber))
        {
            System.Windows.MessageBox.Show("Укажите номер модема", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        IsBusy = true;
        try
        {
            if (_isEditMode)
            {
                var request = new UpdateModemRequest
                {
                    ModemNumber = ModemNumber,
                    Model = Model,
                    Status = SelectedStatus,
                    VendingMachineId = SelectedVendingMachine?.Id
                };
                await _apiService.PutAsync<UpdateModemRequest, object>($"modems/{_modem.Id}", request);
            }
            else
            {
                var request = new CreateModemRequest
                {
                    ModemNumber = ModemNumber,
                    Model = Model,
                    Status = SelectedStatus,
                    VendingMachineId = SelectedVendingMachine?.Id
                };
                await _apiService.PostAsync<CreateModemRequest, ModemDto>("modems", request);
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
