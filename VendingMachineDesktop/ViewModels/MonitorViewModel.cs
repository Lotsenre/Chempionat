using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using VendingMachineDesktop.Models;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class MonitorViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private List<VendingMachineMonitor> _allMonitorData = new();

    [ObservableProperty]
    private ObservableCollection<VendingMachineMonitor> _monitorData = new();

    [ObservableProperty]
    private ObservableCollection<Company> _companies = new();

    [ObservableProperty]
    private Company? _selectedCompany;

    // Время последнего обновления
    [ObservableProperty]
    private string _lastUpdateTime = string.Empty;

    // Фильтры по общему состоянию (статус ТА)
    [ObservableProperty]
    private bool _filterStatusGreen; // Работает

    [ObservableProperty]
    private bool _filterStatusRed; // Не работает

    [ObservableProperty]
    private bool _filterStatusBlue; // На обслуживании

    // Фильтры по типу подключения
    [ObservableProperty]
    private bool _filterConnectionMdb;

    [ObservableProperty]
    private bool _filterConnectionExePh;

    [ObservableProperty]
    private bool _filterConnectionExeSt;

    // Фильтры по дополнительным статусам
    [ObservableProperty]
    private bool _filterFewGoods; // Мало товара (красный)

    [ObservableProperty]
    private bool _filterFewGoodsYellow; // Мало товара (желтый)

    [ObservableProperty]
    private bool _filterFewChange; // Мало сдачи (красный)

    [ObservableProperty]
    private bool _filterFewChangeYellow; // Мало сдачи (желтый)

    [ObservableProperty]
    private bool _filterNoEncashment; // Нужна инкассация (красный)

    [ObservableProperty]
    private bool _filterNoEncashmentYellow; // Нужна инкассация (желтый)

    [ObservableProperty]
    private bool _filterNoService; // Нет обслуживания (красный)

    [ObservableProperty]
    private bool _filterNoServiceYellow; // Нет обслуживания (желтый)

    [ObservableProperty]
    private bool _filterNoFillup; // Нужна загрузка (красный)

    [ObservableProperty]
    private bool _filterNoFillupYellow; // Нужна загрузка (желтый)

    [ObservableProperty]
    private bool _filterHardwareProblems; // Проблемы с оборудованием

    // Варианты сортировки
    [ObservableProperty]
    private ObservableCollection<SortOption> _sortOptions = new()
    {
        new SortOption { Name = "По состоянию ТА", Value = "status" },
        new SortOption { Name = "По названию ТА", Value = "name" },
        new SortOption { Name = "По времени пинга ↑", Value = "ping_asc" },
        new SortOption { Name = "По времени пинга ↓", Value = "ping_desc" },
        new SortOption { Name = "По общей загрузке ↑", Value = "load_asc" },
        new SortOption { Name = "По общей загрузке ↓", Value = "load_desc" },
        new SortOption { Name = "По минимальной загрузке ↑", Value = "min_load_asc" },
        new SortOption { Name = "По минимальной загрузке ↓", Value = "min_load_desc" },
        new SortOption { Name = "По сумме монет ↑", Value = "coins_asc" },
        new SortOption { Name = "По сумме монет ↓", Value = "coins_desc" },
        new SortOption { Name = "По сумме купюр ↑", Value = "bills_asc" },
        new SortOption { Name = "По сумме купюр ↓", Value = "bills_desc" },
        new SortOption { Name = "По сумме сдачи ↑", Value = "change_asc" },
        new SortOption { Name = "По сумме сдачи ↓", Value = "change_desc" },
        new SortOption { Name = "По времени продажи ↑", Value = "sale_time_asc" },
        new SortOption { Name = "По времени продажи ↓", Value = "sale_time_desc" },
        new SortOption { Name = "По времени инкассации ↑", Value = "encashment_asc" },
        new SortOption { Name = "По времени инкассации ↓", Value = "encashment_desc" },
        new SortOption { Name = "По времени обслуживания ↑", Value = "service_asc" },
        new SortOption { Name = "По времени обслуживания ↓", Value = "service_desc" },
        new SortOption { Name = "По сумме продаж сегодня ↑", Value = "sales_today_asc" },
        new SortOption { Name = "По сумме продаж сегодня ↓", Value = "sales_today_desc" },
        new SortOption { Name = "По сумме продаж с обсл. ↑", Value = "sales_service_asc" },
        new SortOption { Name = "По сумме продаж с обсл. ↓", Value = "sales_service_desc" },
        new SortOption { Name = "По кол-ву продаж с обсл. ↑", Value = "sales_count_asc" },
        new SortOption { Name = "По кол-ву продаж с обсл. ↓", Value = "sales_count_desc" }
    };

    [ObservableProperty]
    private SortOption? _selectedSortOption;

    // Statistics
    [ObservableProperty]
    private int _totalMachines;

    [ObservableProperty]
    private int _onlineMachines;

    [ObservableProperty]
    private int _offlineMachines;

    [ObservableProperty]
    private decimal _totalCash;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _selectAll;

    // Показывать сообщение об отсутствии результатов
    [ObservableProperty]
    private bool _showNoResultsMessage;

    // Режим отображения: таблица или плитки
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTileView))]
    private bool _isTableView = true;

    public bool IsTileView => !IsTableView;

    [RelayCommand]
    private void SetTableView() => IsTableView = true;

    [RelayCommand]
    private void SetTileView() => IsTableView = false;

    partial void OnSelectAllChanged(bool value)
    {
        foreach (var item in MonitorData)
        {
            item.IsSelected = value;
        }
    }

    public MonitorViewModel(IApiService apiService)
    {
        _apiService = apiService;
        SelectedSortOption = SortOptions[0];
        UpdateLastUpdateTime();
        LoadDataAsync();
    }

    private void UpdateLastUpdateTime()
    {
        LastUpdateTime = $"данные актуальны на {DateTime.Now:HH:mm:ss} (UTC+3)";
    }

    private async void LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            // Load vending machines and convert to monitor data
            var machines = await _apiService.GetAsync<List<VendingMachine>>("vendingmachines");
            if (machines != null)
            {
                _allMonitorData = machines.Select(m => CreateMonitorData(m)).ToList();

                // Строим список компаний из уникальных значений
                var companyNames = _allMonitorData
                    .Select(m => m.Company)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                Companies = new ObservableCollection<Company>();
                Companies.Add(new Company { Id = Guid.Empty, Name = "Все компании", Code = "ALL" });
                foreach (var name in companyNames)
                {
                    Companies.Add(new Company { Id = Guid.NewGuid(), Name = name, Code = name });
                }
                SelectedCompany = Companies[0]; // "Все компании"

                ApplyFiltersInternal();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Monitor load error: {ex.Message}");
            GenerateMockData();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private VendingMachineMonitor CreateMonitorData(VendingMachine machine)
    {
        var random = Random.Shared;
        var operators = new[] { "МТС", "Билайн", "Мегафон", "Теле2" };
        var states = new[] { "Sale", "Service", "Encashment", "Bills", "Coins", "Change" };
        var connectionTypes = new[] { "MDB", "EXE_PH", "EXE_ST" };

        // Используем реальный статус из БД
        var machineStatus = machine.Status switch
        {
            "Working" => "Working",
            "NotWorking" => "NotWorking",
            _ => "OnMaintenance"
        };

        return new VendingMachineMonitor
        {
            Id = machine.Id,
            Code = machine.SerialNumber,
            Name = machine.Name,
            Address = !string.IsNullOrEmpty(machine.Location) ? machine.Location : "Адрес не указан",
            Company = !string.IsNullOrEmpty(machine.Company) ? machine.Company : "Без компании",
            IsSelected = false,
            MobileOperator = operators[random.Next(operators.Length)],
            SignalStrength = random.Next(1, 6),
            IsOnline = machineStatus == "Working" || random.Next(100) > 40,
            CurrentState = states[random.Next(states.Length)],
            ConnectionType = connectionTypes[random.Next(connectionTypes.Length)],
            MachineStatus = machineStatus,
            LoadPercent = random.Next(5, 100),
            LoadInfo = $"{random.Next(1, 10)} мин. {random.Next(10, 59)} сек.",
            HasLowGoods = random.Next(100) < 30,
            HasLowGoodsYellow = random.Next(100) < 20,
            HasFewChange = random.Next(100) < 20,
            HasFewChangeYellow = random.Next(100) < 15,
            NeedsFillup = random.Next(100) < 15,
            NeedsFillupYellow = random.Next(100) < 10,
            CashAmount = random.Next(100, 1500),
            CoinAmount = random.Next(10, 500),
            BillCount = random.Next(10, 100),
            CoinCount = random.Next(50, 300),
            LastEncashment = $"{random.Next(1, 7)} часа назад",
            NeedsEncashment = random.Next(100) < 20,
            NeedsEncashmentYellow = random.Next(100) < 15,
            SalesCount = random.Next(5, 50),
            ServiceCount = random.Next(0, 5),
            BillValidatorOk = random.Next(100) > 10,
            CoinAcceptorOk = random.Next(100) > 5,
            CashlessOk = random.Next(100) > 15,
            CashRegisterOk = random.Next(100) > 10,
            DisplayOk = random.Next(100) > 5,
            PowerOk = true,
            LastActivity = $"{random.Next(1, 10)} часа назад",
            LastActivityTime = DateTime.Now.AddMinutes(-random.Next(1, 600)),
            HasProblems = random.Next(100) < 15,
            NeedsService = random.Next(100) < 10,
            NeedsServiceYellow = random.Next(100) < 8,
            HasDifferentSettings = random.Next(100) < 5,
            NoSales = random.Next(100) < 10,
            SalesToday = random.Next(0, 30),
            SalesTodayAmount = random.Next(0, 5000),
            LastSaleTime = DateTime.Now.AddMinutes(-random.Next(1, 300)),
            LastServiceTime = DateTime.Now.AddDays(-random.Next(1, 30)),
            LastEncashmentTime = DateTime.Now.AddDays(-random.Next(1, 14)),
            // Загрузки ингредиентов
            CoffeeLoad = random.Next(0, 100),
            SugarLoad = random.Next(0, 100),
            MilkLoad = random.Next(0, 100),
            CupsLoad = random.Next(0, 100),
            LidsLoad = random.Next(0, 100),
            WaterLoad = random.Next(0, 100)
        };
    }

    private void GenerateMockData()
    {
        Companies = new ObservableCollection<Company>
        {
            new Company { Id = Guid.NewGuid(), Code = "300499", Name = "ООО Типовая Агенство" },
            new Company { Id = Guid.NewGuid(), Code = "300500", Name = "ООО Вендинг Групп" },
            new Company { Id = Guid.NewGuid(), Code = "300501", Name = "ИП Иванов" }
        };
        if (Companies.Count > 0)
        {
            SelectedCompany = Companies[0];
        }

        var mockMachines = new List<VendingMachineMonitor>();
        var names = new[] { "ТЦ 'Московский'", "ТП 'Гастроном'", "ТП 'Магнит'", "ДОСААФ", "Завод", "Офис центр", "ТЦ Галерея", "Вокзал", "Аэропорт", "БЦ Сити" };
        var codes = new[] { "90382S", "90282S", "90182S", "90325", "93626s", "90827", "90456", "90123", "90789", "90555" };
        var addresses = new[] { "Москва, Комсомольская пл.", "СПб, Невский пр.", "Казань, ул. Баумана", "Нижний Новгород, пл. Минина", "Екатеринбург, ул. Ленина" };
        var connectionTypes = new[] { "MDB", "EXE_PH", "EXE_ST" };
        var machineStatuses = new[] { "Working", "NotWorking", "OnMaintenance" };

        for (int i = 0; i < 10; i++)
        {
            var random = Random.Shared;
            var operators = new[] { "МТС", "Билайн", "Мегафон", "Теле2" };
            var states = new[] { "Sale", "Service", "Encashment", "Bills", "Coins", "Change" };

            mockMachines.Add(new VendingMachineMonitor
            {
                Id = Guid.NewGuid(),
                Code = codes[i],
                Name = names[i],
                Address = addresses[random.Next(addresses.Length)],
                IsSelected = false,
                MobileOperator = operators[random.Next(operators.Length)],
                SignalStrength = random.Next(1, 6),
                IsOnline = random.Next(100) > 20,
                CurrentState = states[random.Next(states.Length)],
                ConnectionType = connectionTypes[random.Next(connectionTypes.Length)],
                MachineStatus = machineStatuses[random.Next(machineStatuses.Length)],
                LoadPercent = random.Next(5, 100),
                LoadInfo = $"{random.Next(1, 10)} мин. {random.Next(10, 59)} сек.",
                HasLowGoods = random.Next(100) < 30,
                HasLowGoodsYellow = random.Next(100) < 20,
                HasFewChange = random.Next(100) < 20,
                HasFewChangeYellow = random.Next(100) < 15,
                NeedsFillup = random.Next(100) < 15,
                NeedsFillupYellow = random.Next(100) < 10,
                CashAmount = random.Next(100, 1500),
                CoinAmount = random.Next(10, 500),
                BillCount = random.Next(10, 100),
                CoinCount = random.Next(50, 300),
                LastEncashment = $"{random.Next(1, 7)} часа назад",
                NeedsEncashment = random.Next(100) < 20,
                NeedsEncashmentYellow = random.Next(100) < 15,
                SalesCount = random.Next(5, 50),
                ServiceCount = random.Next(0, 5),
                BillValidatorOk = random.Next(100) > 10,
                CoinAcceptorOk = random.Next(100) > 5,
                CashlessOk = random.Next(100) > 15,
                CashRegisterOk = random.Next(100) > 10,
                DisplayOk = random.Next(100) > 5,
                PowerOk = true,
                LastActivity = $"{random.Next(1, 10)} часа назад",
                LastActivityTime = DateTime.Now.AddMinutes(-random.Next(1, 600)),
                HasProblems = random.Next(100) < 15,
                NeedsService = random.Next(100) < 10,
                NeedsServiceYellow = random.Next(100) < 8,
                HasDifferentSettings = random.Next(100) < 5,
                NoSales = random.Next(100) < 10,
                SalesToday = random.Next(0, 30),
                SalesTodayAmount = random.Next(0, 5000),
                LastSaleTime = DateTime.Now.AddMinutes(-random.Next(1, 300)),
                LastServiceTime = DateTime.Now.AddDays(-random.Next(1, 30)),
                LastEncashmentTime = DateTime.Now.AddDays(-random.Next(1, 14)),
                CoffeeLoad = random.Next(0, 100),
                SugarLoad = random.Next(0, 100),
                MilkLoad = random.Next(0, 100),
                CupsLoad = random.Next(0, 100),
                LidsLoad = random.Next(0, 100),
                WaterLoad = random.Next(0, 100)
            });
        }

        _allMonitorData = mockMachines;
        ApplyFiltersInternal();
    }

    [RelayCommand]
    private void ApplyFilter()
    {
        ApplyFiltersInternal();
        UpdateLastUpdateTime();
    }

    [RelayCommand]
    private void ClearFilter()
    {
        // Сбросить все фильтры
        FilterStatusGreen = false;
        FilterStatusRed = false;
        FilterStatusBlue = false;
        FilterConnectionMdb = false;
        FilterConnectionExePh = false;
        FilterConnectionExeSt = false;
        FilterFewGoods = false;
        FilterFewGoodsYellow = false;
        FilterFewChange = false;
        FilterFewChangeYellow = false;
        FilterNoEncashment = false;
        FilterNoEncashmentYellow = false;
        FilterNoService = false;
        FilterNoServiceYellow = false;
        FilterNoFillup = false;
        FilterNoFillupYellow = false;
        FilterHardwareProblems = false;
        SelectedSortOption = SortOptions[0];
        ApplyFiltersInternal();
        UpdateLastUpdateTime();
    }

    private void ApplyFiltersInternal()
    {
        var filtered = _allMonitorData.AsEnumerable();

        // Фильтр по компании
        if (SelectedCompany != null && SelectedCompany.Code != "ALL")
        {
            filtered = filtered.Where(m => m.Company == SelectedCompany.Name);
        }

        // Проверяем, активны ли фильтры по состоянию
        bool hasStatusFilter = FilterStatusGreen || FilterStatusRed || FilterStatusBlue;
        if (hasStatusFilter)
        {
            filtered = filtered.Where(m =>
                (FilterStatusGreen && m.MachineStatus == "Working") ||
                (FilterStatusRed && m.MachineStatus == "NotWorking") ||
                (FilterStatusBlue && m.MachineStatus == "OnMaintenance"));
        }

        // Фильтры по типу подключения
        bool hasConnectionFilter = FilterConnectionMdb || FilterConnectionExePh || FilterConnectionExeSt;
        if (hasConnectionFilter)
        {
            filtered = filtered.Where(m =>
                (FilterConnectionMdb && m.ConnectionType == "MDB") ||
                (FilterConnectionExePh && m.ConnectionType == "EXE_PH") ||
                (FilterConnectionExeSt && m.ConnectionType == "EXE_ST"));
        }

        // Фильтры по дополнительным статусам
        bool hasAdditionalFilter = FilterFewGoods || FilterFewGoodsYellow || FilterFewChange ||
                                   FilterFewChangeYellow || FilterNoEncashment || FilterNoEncashmentYellow ||
                                   FilterNoService || FilterNoServiceYellow || FilterNoFillup ||
                                   FilterNoFillupYellow || FilterHardwareProblems;

        if (hasAdditionalFilter)
        {
            filtered = filtered.Where(m =>
                (FilterFewGoods && m.HasLowGoods) ||
                (FilterFewGoodsYellow && m.HasLowGoodsYellow) ||
                (FilterFewChange && m.HasFewChange) ||
                (FilterFewChangeYellow && m.HasFewChangeYellow) ||
                (FilterNoEncashment && m.NeedsEncashment) ||
                (FilterNoEncashmentYellow && m.NeedsEncashmentYellow) ||
                (FilterNoService && m.NeedsService) ||
                (FilterNoServiceYellow && m.NeedsServiceYellow) ||
                (FilterNoFillup && m.NeedsFillup) ||
                (FilterNoFillupYellow && m.NeedsFillupYellow) ||
                (FilterHardwareProblems && m.HasProblems));
        }

        // Применяем сортировку
        var sortedResult = ApplySorting(filtered);

        var result = sortedResult.ToList();

        MonitorData = new ObservableCollection<VendingMachineMonitor>(result);
        ShowNoResultsMessage = result.Count == 0;

        // Update statistics from all data
        TotalMachines = _allMonitorData.Count;
        OnlineMachines = _allMonitorData.Count(m => m.IsOnline);
        OfflineMachines = _allMonitorData.Count(m => !m.IsOnline);
        TotalCash = _allMonitorData.Sum(m => m.CoinAmount);
    }

    private IEnumerable<VendingMachineMonitor> ApplySorting(IEnumerable<VendingMachineMonitor> data)
    {
        if (SelectedSortOption == null)
            return data;

        return SelectedSortOption.Value switch
        {
            "status" => data.OrderByDescending(m => m.MachineStatus == "Working")
                           .ThenBy(m => m.MachineStatus == "OnMaintenance")
                           .ThenBy(m => m.Name),
            "name" => data.OrderBy(m => m.Name),
            "ping_asc" => data.OrderBy(m => m.LastActivityTime),
            "ping_desc" => data.OrderByDescending(m => m.LastActivityTime),
            "load_asc" => data.OrderBy(m => m.LoadPercent),
            "load_desc" => data.OrderByDescending(m => m.LoadPercent),
            "min_load_asc" => data.OrderBy(m => Math.Min(Math.Min(m.CoffeeLoad, m.SugarLoad), Math.Min(m.MilkLoad, m.CupsLoad))),
            "min_load_desc" => data.OrderByDescending(m => Math.Min(Math.Min(m.CoffeeLoad, m.SugarLoad), Math.Min(m.MilkLoad, m.CupsLoad))),
            "coins_asc" => data.OrderBy(m => m.CoinAmount),
            "coins_desc" => data.OrderByDescending(m => m.CoinAmount),
            "bills_asc" => data.OrderBy(m => m.CashAmount),
            "bills_desc" => data.OrderByDescending(m => m.CashAmount),
            "change_asc" => data.OrderBy(m => m.CoinAmount),
            "change_desc" => data.OrderByDescending(m => m.CoinAmount),
            "sale_time_asc" => data.OrderBy(m => m.LastSaleTime),
            "sale_time_desc" => data.OrderByDescending(m => m.LastSaleTime),
            "encashment_asc" => data.OrderBy(m => m.LastEncashmentTime),
            "encashment_desc" => data.OrderByDescending(m => m.LastEncashmentTime),
            "service_asc" => data.OrderBy(m => m.LastServiceTime),
            "service_desc" => data.OrderByDescending(m => m.LastServiceTime),
            "sales_today_asc" => data.OrderBy(m => m.SalesTodayAmount),
            "sales_today_desc" => data.OrderByDescending(m => m.SalesTodayAmount),
            "sales_service_asc" => data.OrderBy(m => m.SalesCount),
            "sales_service_desc" => data.OrderByDescending(m => m.SalesCount),
            "sales_count_asc" => data.OrderBy(m => m.SalesToday),
            "sales_count_desc" => data.OrderByDescending(m => m.SalesToday),
            _ => data
        };
    }

    [RelayCommand]
    private void Export()
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
            FileName = $"monitor_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
            DefaultExt = ".csv"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                // Используем UTF-8 с BOM для корректного отображения кириллицы в Excel
                using var writer = new StreamWriter(saveDialog.FileName, false, new UTF8Encoding(true));

                // Write header (используем точку с запятой как разделитель для Excel)
                writer.WriteLine("Код;Название;Адрес;Оператор;Сигнал;Статус;Загрузка %;Купюры р.;Монеты шт.;Сдача р.;Продажи;Обслуживание;Последняя активность");

                // Write data
                foreach (var item in MonitorData)
                {
                    var status = item.MachineStatus switch
                    {
                        "Working" => "Работает",
                        "NotWorking" => "Не работает",
                        "OnMaintenance" => "На обслуживании",
                        _ => item.MachineStatus
                    };

                    // Экранируем точки с запятой и кавычки в тексте
                    var name = EscapeCsvField(item.Name);
                    var address = EscapeCsvField(item.Address);
                    var lastActivity = EscapeCsvField(item.LastActivity);

                    writer.WriteLine($"{item.Code};{name};{address};{item.MobileOperator};{item.SignalStrength};{status};{item.LoadPercent};{item.CashAmount:N0};{item.CoinCount};{item.CoinAmount:N0};{item.SalesCount};{item.ServiceCount};{lastActivity}");
                }

                MessageBox.Show($"Экспорт успешно выполнен!\nФайл: {saveDialog.FileName}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return string.Empty;

        // Если поле содержит разделитель, кавычки или перенос строки - оборачиваем в кавычки
        if (field.Contains(';') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            // Экранируем кавычки удвоением
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }

    partial void OnSelectedCompanyChanged(Company? value)
    {
        ApplyFiltersInternal();
    }

    partial void OnSelectedSortOptionChanged(SortOption? value)
    {
        ApplyFiltersInternal();
    }
}

// Модель для опций сортировки
public class SortOption
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public override string ToString() => Name;
}
