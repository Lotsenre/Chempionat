using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using VendingMachineDesktop.Models;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private double _networkEfficiency;

    [ObservableProperty]
    private int _totalMachines;

    [ObservableProperty]
    private string _salesDateRange = "";

    [ObservableProperty]
    private Summary _summary = new();

    [ObservableProperty]
    private Models.NetworkStatus _networkStatus = new();

    [ObservableProperty]
    private ObservableCollection<News> _newsList = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSalesByQuantity))]
    private bool _isSalesByAmount = true;

    public bool IsSalesByQuantity => !IsSalesByAmount;

    // LiveCharts Series
    [ObservableProperty]
    private ISeries[] _efficiencyGaugeSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private ISeries[] _networkStatusSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private ObservableCollection<ISeries> _salesSeries = new();

    [ObservableProperty]
    private ObservableCollection<Axis> _salesXAxes = new();

    [ObservableProperty]
    private ObservableCollection<Axis> _salesYAxes = new();

    private List<SalesDynamics> _originalSalesData = new();

    // Метод для получения данных продаж из code-behind
    public List<SalesDynamics> GetSalesData() => _originalSalesData;

    public DashboardViewModel(IApiService apiService)
    {
        _apiService = apiService;
        InitializeCharts();
        LoadDataAsync();
    }

    private void InitializeCharts()
    {
        // Initialize with mock data
        LoadMockData();
    }

    private void LoadMockData()
    {
        // Mock network efficiency - 9 автоматов, 8 работает = 88.9%
        NetworkEfficiency = 88.9;
        TotalMachines = 9;

        // Mock Summary data - реалистичные данные для сети из 9 автоматов
        Summary = new Summary
        {
            TotalSales = 27889,      // Денег в ТА
            ChangeAmount = 12319,    // Сдача в ТА
            YesterdayRevenue = 9860, // Выручка вчера
            SalesCount = 324,        // Продаж вчера (реалистичнее для 9 автоматов)
            TotalCollection = 2461,  // Инкассация вчера
            MaintenanceCount = 0,    // Обслуживание по плану
            CollectionCount = 3      // Всего по плану
        };

        // Mock Network Status - 7 работает, 1 не работает, 1 на обслуживании
        NetworkStatus = new Models.NetworkStatus
        {
            Working = 7,
            NotWorking = 1,
            OnMaintenance = 1
        };

        // Update Efficiency Gauge
        UpdateEfficiencyGauge(NetworkEfficiency);

        // Update Network Status Pie Chart
        UpdateNetworkStatusChart(NetworkStatus);

        // Mock Sales Dynamics (last 10 days) - реалистичные данные продаж
        var today = DateTime.Now;
        _originalSalesData = new List<SalesDynamics>
        {
            new SalesDynamics { Date = today.AddDays(-9), Amount = 8540, Quantity = 285 },
            new SalesDynamics { Date = today.AddDays(-8), Amount = 12350, Quantity = 412 },
            new SalesDynamics { Date = today.AddDays(-7), Amount = 15420, Quantity = 514 },
            new SalesDynamics { Date = today.AddDays(-6), Amount = 9870, Quantity = 329 },
            new SalesDynamics { Date = today.AddDays(-5), Amount = 11200, Quantity = 373 },
            new SalesDynamics { Date = today.AddDays(-4), Amount = 14650, Quantity = 488 },
            new SalesDynamics { Date = today.AddDays(-3), Amount = 16800, Quantity = 560 },
            new SalesDynamics { Date = today.AddDays(-2), Amount = 13450, Quantity = 448 },
            new SalesDynamics { Date = today.AddDays(-1), Amount = 9860, Quantity = 324 },
            new SalesDynamics { Date = today, Amount = 7250, Quantity = 242 }
        };

        SalesDateRange = $"Данные по продажам с {_originalSalesData.First().Date:dd.MM.yyyy} по {_originalSalesData.Last().Date:dd.MM.yyyy}";

        // Update Sales Chart
        UpdateSalesChart(true);

        // Mock News - актуальные новости франчайзера
        NewsList.Clear();
        NewsList.Add(new News { Date = new DateTime(2026, 1, 4), Title = "Обновление системы мониторинга торговых автоматов v2.5" });
        NewsList.Add(new News { Date = new DateTime(2026, 1, 3), Title = "Заключен договор с новым поставщиком кофе Premium Arabica" });
        NewsList.Add(new News { Date = new DateTime(2026, 1, 2), Title = "Расширение сети: добавлено 3 новых точки в ТЦ 'Мега'" });
        NewsList.Add(new News { Date = new DateTime(2025, 12, 28), Title = "Праздничные скидки на обслуживание ТА до 15%" });
        NewsList.Add(new News { Date = new DateTime(2025, 12, 25), Title = "Новая CRM-система KIT Shop доступна для всех партнеров" });
        NewsList.Add(new News { Date = new DateTime(2025, 12, 20), Title = "Получена сертификация ISO 9001 на сервисное обслуживание" });
        NewsList.Add(new News { Date = new DateTime(2025, 12, 15), Title = "Запуск мобильного приложения для инженеров v1.0" });
        NewsList.Add(new News { Date = new DateTime(2025, 12, 10), Title = "Интеграция с платежной системой СБП завершена" });
    }

    private void UpdateEfficiencyGauge(double efficiency)
    {
        var remainingValue = 100 - efficiency;

        EfficiencyGaugeSeries = new ISeries[]
        {
            new PieSeries<double>
            {
                Values = new double[] { efficiency },
                Fill = new SolidColorPaint(new SKColor(76, 175, 80)), // Green
                DataLabelsSize = 0,
                InnerRadius = 50,
                MaxRadialColumnWidth = 15,
                IsHoverable = false
            },
            new PieSeries<double>
            {
                Values = new double[] { remainingValue },
                Fill = new SolidColorPaint(new SKColor(224, 224, 224)), // Light gray
                DataLabelsSize = 0,
                InnerRadius = 50,
                MaxRadialColumnWidth = 15,
                IsHoverable = false
            }
        };
        OnPropertyChanged(nameof(EfficiencyGaugeSeries));
    }

    private void UpdateNetworkStatusChart(Models.NetworkStatus status)
    {
        NetworkStatusSeries = new ISeries[]
        {
            new PieSeries<int>
            {
                Values = new int[] { status.Working },
                Name = "Работает",
                Fill = new SolidColorPaint(new SKColor(76, 175, 80)), // #4CAF50
                DataLabelsSize = 0
            },
            new PieSeries<int>
            {
                Values = new int[] { status.NotWorking },
                Name = "Не работает",
                Fill = new SolidColorPaint(new SKColor(244, 67, 54)), // #F44336
                DataLabelsSize = 0
            },
            new PieSeries<int>
            {
                Values = new int[] { status.OnMaintenance },
                Name = "На обслуживании",
                Fill = new SolidColorPaint(new SKColor(33, 150, 243)), // #2196F3
                DataLabelsSize = 0
            }
        };
        OnPropertyChanged(nameof(NetworkStatusSeries));
    }

    private async void LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            // Даем время токену установиться
            await Task.Delay(100);

            var data = await _apiService.GetAsync<DashboardData>("dashboard");
            if (data != null)
            {
                NetworkEfficiency = data.NetworkEfficiency;
                TotalMachines = data.NetworkStatus.Working + data.NetworkStatus.NotWorking + data.NetworkStatus.OnMaintenance;
                Summary = data.Summary;
                NetworkStatus = data.NetworkStatus;

                // Update charts with real data
                UpdateEfficiencyGauge(data.NetworkEfficiency);
                UpdateNetworkStatusChart(data.NetworkStatus);

                // Store original sales data - только если API вернул данные
                if (data.SalesDynamics != null && data.SalesDynamics.Count > 0)
                {
                    _originalSalesData = data.SalesDynamics;
                    SalesDateRange = $"Данные по продажам с {_originalSalesData.First().Date:dd.MM.yyyy} по {_originalSalesData.Last().Date:dd.MM.yyyy}";
                    // Update Sales Dynamics Chart (default: by amount)
                    UpdateSalesChart(true);
                }
                // Иначе оставляем мок-данные загруженные в LoadMockData()

                // Update News List - только если API вернул новости
                if (data.News != null && data.News.Count > 0)
                {
                    NewsList.Clear();
                    foreach (var news in data.News)
                    {
                        NewsList.Add(news);
                    }
                }
                // Иначе оставляем мок-данные
            }
        }
        catch (Exception ex)
        {
            // Логирование ошибки - используем мок данные
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки Dashboard: {ex.Message}. Используются демо-данные.");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateSalesChart(bool byAmount)
    {
        if (_originalSalesData.Count == 0) return;

        var salesValues = byAmount
            ? _originalSalesData.Select(s => (double)s.Amount).ToArray()
            : _originalSalesData.Select(s => (double)s.Quantity).ToArray();

        var labels = _originalSalesData.Select(s => s.Date.ToString("dd.MM")).ToArray();

        // Очищаем и добавляем заново для ObservableCollection
        SalesSeries.Clear();
        SalesSeries.Add(new ColumnSeries<double>
        {
            Values = salesValues,
            Name = byAmount ? "Сумма" : "Количество",
            Fill = new SolidColorPaint(new SKColor(33, 150, 243)),
            MaxBarWidth = 40
        });

        SalesXAxes.Clear();
        SalesXAxes.Add(new Axis
        {
            Labels = labels,
            LabelsRotation = 0,
            TextSize = 11,
            SeparatorsPaint = new SolidColorPaint(new SKColor(240, 240, 240))
        });

        SalesYAxes.Clear();
        SalesYAxes.Add(new Axis
        {
            Name = byAmount ? "Сумма (₽)" : "Количество (шт.)",
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 15),
            TextSize = 11,
            SeparatorsPaint = new SolidColorPaint(new SKColor(240, 240, 240))
        });
    }

    partial void OnIsSalesByAmountChanged(bool value)
    {
        UpdateSalesChart(value);
    }

    [RelayCommand]
    private void SetSalesByAmount()
    {
        IsSalesByAmount = true;
    }

    [RelayCommand]
    private void SetSalesByQuantity()
    {
        IsSalesByAmount = false;
    }
}
