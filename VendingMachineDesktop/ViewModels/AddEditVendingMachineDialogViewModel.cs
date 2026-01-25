using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VendingMachineDesktop.Models;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class AddEditVendingMachineDialogViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly bool _isEditMode;

    public event EventHandler<bool>? CloseRequested;

    [ObservableProperty]
    private VendingMachine _machine;

    [ObservableProperty]
    private bool _isBusy;

    // Dropdown collections
    [ObservableProperty]
    private ObservableCollection<string> _manufacturers = new()
    {
        "Necta", "Bianchi", "Saeco", "Jofemar", "Rheavendors", "Unicum", "FAS"
    };

    [ObservableProperty]
    private ObservableCollection<string> _models = new()
    {
        "Любая", "BVM 972", "Kikko Max", "Cristallo 400", "Coffemar G250", "Luce ES", "Food Box", "Perla"
    };

    [ObservableProperty]
    private ObservableCollection<string> _manufacturersSlave = new()
    {
        "-", "Necta", "Bianchi", "Saeco", "Jofemar", "Rheavendors"
    };

    [ObservableProperty]
    private ObservableCollection<string> _modelsSlave = new()
    {
        "-", "BVM 972", "Kikko Max", "Cristallo 400"
    };

    [ObservableProperty]
    private ObservableCollection<string> _workModes = new()
    {
        "Стандартный", "Мастер", "Slave", "Автономный"
    };

    [ObservableProperty]
    private ObservableCollection<string> _timezones = new()
    {
        "UTC + 2", "UTC + 3", "UTC + 4", "UTC + 5", "UTC + 6", "UTC + 7"
    };

    [ObservableProperty]
    private ObservableCollection<string> _productMatrices = new()
    {
        "Не установлена", "Bianchi BVM 972", "Necta Kikko Max", "Стандартная кофе", "Стандартная снеки"
    };

    [ObservableProperty]
    private ObservableCollection<string> _criticalThresholdTemplates = new()
    {
        "Стандартный", "Высокий трафик", "Низкий трафик", "Не установлен"
    };

    [ObservableProperty]
    private ObservableCollection<string> _notificationTemplates = new()
    {
        "Стандартный", "Критический", "Минимальный", "Не установлен"
    };

    [ObservableProperty]
    private ObservableCollection<string> _clients = new()
    {
        "Не задан"
    };

    [ObservableProperty]
    private ObservableCollection<string> _managers = new()
    {
        "Не задан"
    };

    [ObservableProperty]
    private ObservableCollection<string> _engineers = new()
    {
        "Не задан"
    };

    [ObservableProperty]
    private ObservableCollection<string> _operators = new()
    {
        "Не задан"
    };

    [ObservableProperty]
    private ObservableCollection<string> _servicePriorities = new()
    {
        "Низкий", "Средний", "Высокий", "Критический"
    };

    // Selected items
    [ObservableProperty]
    private string? _selectedManufacturer;

    [ObservableProperty]
    private string? _selectedModel;

    [ObservableProperty]
    private string? _selectedManufacturerSlave;

    [ObservableProperty]
    private string? _selectedModelSlave;

    // UI properties
    public string DialogTitle => _isEditMode ? "Редактирование торгового автомата" : "Создание торгового автомата";
    public string SaveButtonText => _isEditMode ? "Сохранить" : "Создать";
    public bool ShowCancelButton => !_isEditMode;

    public AddEditVendingMachineDialogViewModel(IApiService apiService, VendingMachine? machine = null)
    {
        _apiService = apiService;
        _isEditMode = machine != null;

        if (machine != null)
        {
            // Create a copy for editing to avoid modifying the original on cancel
            _machine = new VendingMachine
            {
                Id = machine.Id,
                SerialNumber = machine.SerialNumber,
                Name = machine.Name,
                Manufacturer = machine.Manufacturer,
                Model = machine.Model,
                Status = machine.Status,
                ManufacturerSlave = machine.ManufacturerSlave,
                ModelSlave = machine.ModelSlave,
                WorkMode = machine.WorkMode,
                Location = machine.Location,
                Place = machine.Place,
                Coordinates = machine.Coordinates,
                InstallDate = machine.InstallDate,
                LastMaintenanceDate = machine.LastMaintenanceDate,
                WorkingHours = machine.WorkingHours,
                Timezone = machine.Timezone,
                ProductMatrix = machine.ProductMatrix,
                CriticalThresholdTemplate = machine.CriticalThresholdTemplate,
                NotificationTemplate = machine.NotificationTemplate,
                Client = machine.Client,
                Manager = machine.Manager,
                Engineer = machine.Engineer,
                Operator = machine.Operator,
                Technician = machine.Technician,
                HasCoinAcceptor = machine.HasCoinAcceptor,
                HasBillAcceptor = machine.HasBillAcceptor,
                HasCashlessModule = machine.HasCashlessModule,
                HasQrPayment = machine.HasQrPayment,
                PaymentType = machine.PaymentType,
                RfidService = machine.RfidService,
                RfidCashCollection = machine.RfidCashCollection,
                RfidLoading = machine.RfidLoading,
                KitOnlineId = machine.KitOnlineId,
                ModemNumber = machine.ModemNumber,
                ServicePriority = machine.ServicePriority,
                TotalIncome = machine.TotalIncome,
                UserId = machine.UserId,
                CompanyId = machine.CompanyId,
                Company = machine.Company,
                CompanyName = machine.CompanyName,
                Notes = machine.Notes,
                Description = machine.Description,
                Address = machine.Address,
                OperatingSince = machine.OperatingSince
            };

            SelectedManufacturer = machine.Manufacturer;
            SelectedModel = machine.Model;
            SelectedManufacturerSlave = machine.ManufacturerSlave ?? "-";
            SelectedModelSlave = machine.ModelSlave ?? "-";
        }
        else
        {
            _machine = new VendingMachine
            {
                Id = Guid.NewGuid(),
                InstallDate = DateTime.UtcNow,
                OperatingSince = DateTime.UtcNow,
                Status = "Working",
                WorkMode = "Стандартный",
                Timezone = "UTC + 3",
                ServicePriority = "Средний",
                CriticalThresholdTemplate = "Стандартный",
                NotificationTemplate = "Стандартный",
                ProductMatrix = "Не установлена"
            };

            SelectedManufacturer = "Necta";
            SelectedModel = "Любая";
            SelectedManufacturerSlave = "-";
            SelectedModelSlave = "-";
        }

        LoadUsersAsync();
    }

    private async void LoadUsersAsync()
    {
        try
        {
            var users = await _apiService.GetAsync<List<User>>("users");
            if (users != null)
            {
                var managersList = users.Where(u => u.IsManager).Select(u => u.FullName).ToList();
                var engineersList = users.Where(u => u.IsEngineer).Select(u => u.FullName).ToList();
                var operatorsList = users.Where(u => u.IsOperator).Select(u => u.FullName).ToList();

                Managers = new ObservableCollection<string>(new[] { "Не задан" }.Concat(managersList));
                Engineers = new ObservableCollection<string>(new[] { "Не задан" }.Concat(engineersList));
                Operators = new ObservableCollection<string>(new[] { "Не задан" }.Concat(operatorsList));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load users error: {ex.Message}");
        }
    }

    partial void OnSelectedManufacturerChanged(string? value)
    {
        if (Machine != null && value != null)
        {
            Machine.Manufacturer = value;
            UpdateModelsForManufacturer(value);
        }
    }

    partial void OnSelectedModelChanged(string? value)
    {
        if (Machine != null && value != null)
        {
            Machine.Model = value;
        }
    }

    partial void OnSelectedManufacturerSlaveChanged(string? value)
    {
        if (Machine != null)
        {
            Machine.ManufacturerSlave = value == "-" ? null : value;
        }
    }

    partial void OnSelectedModelSlaveChanged(string? value)
    {
        if (Machine != null)
        {
            Machine.ModelSlave = value == "-" ? null : value;
        }
    }

    private void UpdateModelsForManufacturer(string manufacturer)
    {
        Models = manufacturer switch
        {
            "Necta" => new ObservableCollection<string> { "Kikko Max", "Korinto ES", "Krea" },
            "Bianchi" => new ObservableCollection<string> { "BVM 972", "BVM 952", "Lei 700" },
            "Saeco" => new ObservableCollection<string> { "Cristallo 400", "Cristallo 600", "Phedra" },
            "Jofemar" => new ObservableCollection<string> { "Coffemar G250", "Coffemar G500" },
            "Rheavendors" => new ObservableCollection<string> { "Luce ES", "Luce X2" },
            "Unicum" => new ObservableCollection<string> { "Food Box", "Rosso" },
            "FAS" => new ObservableCollection<string> { "Perla", "Fashion" },
            _ => new ObservableCollection<string> { "Любая" }
        };
    }

    [RelayCommand]
    private async Task Save()
    {
        // Validation
        if (string.IsNullOrWhiteSpace(Machine.Name))
        {
            System.Windows.MessageBox.Show("Укажите название ТА", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(Machine.SerialNumber))
        {
            System.Windows.MessageBox.Show("Укажите номер автомата", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(Machine.Location))
        {
            System.Windows.MessageBox.Show("Укажите адрес", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (!Machine.HasCoinAcceptor && !Machine.HasBillAcceptor && !Machine.HasCashlessModule && !Machine.HasQrPayment)
        {
            System.Windows.MessageBox.Show("Выберите хотя бы одну платежную систему", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        IsBusy = true;
        try
        {
            // Build PaymentType string from checkboxes
            var paymentTypes = new List<string>();
            if (Machine.HasCoinAcceptor) paymentTypes.Add("Монетоприёмник");
            if (Machine.HasBillAcceptor) paymentTypes.Add("Купюроприёмник");
            if (Machine.HasCashlessModule) paymentTypes.Add("Безналичная оплата");
            if (Machine.HasQrPayment) paymentTypes.Add("QR-платежи");
            Machine.PaymentType = string.Join(", ", paymentTypes);

            // Ensure all DateTime values are in UTC for PostgreSQL
            if (Machine.InstallDate.Kind != DateTimeKind.Utc)
                Machine.InstallDate = DateTime.SpecifyKind(Machine.InstallDate, DateTimeKind.Utc);
            if (Machine.OperatingSince.Kind != DateTimeKind.Utc)
                Machine.OperatingSince = DateTime.SpecifyKind(Machine.OperatingSince, DateTimeKind.Utc);
            if (Machine.LastMaintenanceDate.HasValue && Machine.LastMaintenanceDate.Value.Kind != DateTimeKind.Utc)
                Machine.LastMaintenanceDate = DateTime.SpecifyKind(Machine.LastMaintenanceDate.Value, DateTimeKind.Utc);

            if (_isEditMode)
            {
                var updateRequest = new UpdateVendingMachineRequest
                {
                    Name = Machine.Name,
                    SerialNumber = Machine.SerialNumber,
                    Manufacturer = Machine.Manufacturer,
                    Model = Machine.Model,
                    Status = Machine.Status,
                    ManufacturerSlave = Machine.ManufacturerSlave,
                    ModelSlave = Machine.ModelSlave,
                    WorkMode = Machine.WorkMode,
                    Location = Machine.Location,
                    Place = Machine.Place,
                    Coordinates = Machine.Coordinates,
                    WorkingHours = Machine.WorkingHours,
                    Timezone = Machine.Timezone,
                    ProductMatrix = Machine.ProductMatrix,
                    CriticalThresholdTemplate = Machine.CriticalThresholdTemplate,
                    NotificationTemplate = Machine.NotificationTemplate,
                    Client = Machine.Client,
                    Manager = Machine.Manager,
                    Engineer = Machine.Engineer,
                    Operator = Machine.Operator,
                    PaymentType = Machine.PaymentType,
                    RfidService = Machine.RfidService,
                    RfidCashCollection = Machine.RfidCashCollection,
                    RfidLoading = Machine.RfidLoading,
                    KitOnlineId = Machine.KitOnlineId,
                    ModemNumber = Machine.ModemNumber,
                    ServicePriority = Machine.ServicePriority,
                    Notes = Machine.Notes,
                    Company = Machine.Company
                };

                await _apiService.PutAsync<UpdateVendingMachineRequest, object>($"vendingmachines/{Machine.Id}", updateRequest);
                System.Windows.MessageBox.Show("Торговый автомат успешно обновлён", "Успех",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            else
            {
                var createRequest = new CreateVendingMachineRequest
                {
                    Name = Machine.Name,
                    SerialNumber = Machine.SerialNumber,
                    Manufacturer = Machine.Manufacturer,
                    Model = Machine.Model,
                    ManufacturerSlave = Machine.ManufacturerSlave,
                    ModelSlave = Machine.ModelSlave,
                    WorkMode = Machine.WorkMode,
                    Location = Machine.Location,
                    Place = Machine.Place,
                    Coordinates = Machine.Coordinates,
                    InstallDate = Machine.InstallDate,
                    WorkingHours = Machine.WorkingHours,
                    Timezone = Machine.Timezone,
                    ProductMatrix = Machine.ProductMatrix,
                    CriticalThresholdTemplate = Machine.CriticalThresholdTemplate,
                    NotificationTemplate = Machine.NotificationTemplate,
                    Client = Machine.Client,
                    Manager = Machine.Manager,
                    Engineer = Machine.Engineer,
                    Operator = Machine.Operator,
                    PaymentType = Machine.PaymentType,
                    RfidService = Machine.RfidService,
                    RfidCashCollection = Machine.RfidCashCollection,
                    RfidLoading = Machine.RfidLoading,
                    KitOnlineId = Machine.KitOnlineId,
                    ModemNumber = Machine.ModemNumber,
                    ServicePriority = Machine.ServicePriority,
                    Notes = Machine.Notes,
                    Company = Machine.Company
                };

                await _apiService.PostAsync<CreateVendingMachineRequest, VendingMachineDto>("vendingmachines", createRequest);
                System.Windows.MessageBox.Show("Торговый автомат успешно создан", "Успех",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
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
