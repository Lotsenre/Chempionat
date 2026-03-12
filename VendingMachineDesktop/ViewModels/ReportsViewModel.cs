using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private int _selectedTabIndex;

    // Date filters
    [ObservableProperty]
    private DateTime _dateFrom = DateTime.Now.AddDays(-30);

    [ObservableProperty]
    private DateTime _dateTo = DateTime.Now;

    // Sales report
    [ObservableProperty]
    private ObservableCollection<SalesReportItem> _salesData = new();

    [ObservableProperty]
    private int _totalSales;

    [ObservableProperty]
    private decimal _totalRevenue;

    [ObservableProperty]
    private int _totalQuantity;

    [ObservableProperty]
    private decimal _averageCheck;

    // Maintenance report
    [ObservableProperty]
    private ObservableCollection<MaintenanceReportItem> _maintenanceData = new();

    [ObservableProperty]
    private int _maintenanceTotal;

    [ObservableProperty]
    private int _maintenanceCompleted;

    [ObservableProperty]
    private int _maintenancePending;

    // Revenue report
    [ObservableProperty]
    private ObservableCollection<DailyRevenueItem> _dailyRevenueData = new();

    [ObservableProperty]
    private ObservableCollection<MachineRevenueItem> _machineRevenueData = new();

    [ObservableProperty]
    private ObservableCollection<PaymentMethodRevenueItem> _paymentMethodData = new();

    [ObservableProperty]
    private bool _isLoading;

    public ReportsViewModel(IApiService apiService)
    {
        _apiService = apiService;
        LoadSalesReportAsync();
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        switch (value)
        {
            case 0: LoadSalesReportAsync(); break;
            case 1: LoadMaintenanceReportAsync(); break;
            case 2: LoadRevenueReportAsync(); break;
        }
    }

    [RelayCommand]
    private void ApplyFilter()
    {
        switch (SelectedTabIndex)
        {
            case 0: LoadSalesReportAsync(); break;
            case 1: LoadMaintenanceReportAsync(); break;
            case 2: LoadRevenueReportAsync(); break;
        }
    }

    private async void LoadSalesReportAsync()
    {
        IsLoading = true;
        try
        {
            var fromStr = DateFrom.ToString("yyyy-MM-dd");
            var toStr = DateTo.ToString("yyyy-MM-dd");
            var report = await _apiService.GetAsync<SalesReportResponse>($"reports/sales?from={fromStr}&to={toStr}");
            if (report != null)
            {
                SalesData = new ObservableCollection<SalesReportItem>(report.Data);
                TotalSales = report.Summary.TotalSales;
                TotalRevenue = report.Summary.TotalRevenue;
                TotalQuantity = report.Summary.TotalQuantity;
                AverageCheck = report.Summary.AverageCheck;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load sales report error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async void LoadMaintenanceReportAsync()
    {
        IsLoading = true;
        try
        {
            var fromStr = DateFrom.ToString("yyyy-MM-dd");
            var toStr = DateTo.ToString("yyyy-MM-dd");
            var report = await _apiService.GetAsync<MaintenanceReportResponse>($"reports/maintenance?from={fromStr}&to={toStr}");
            if (report != null)
            {
                MaintenanceData = new ObservableCollection<MaintenanceReportItem>(report.Data);
                MaintenanceTotal = report.Summary.Total;
                MaintenanceCompleted = report.Summary.Completed;
                MaintenancePending = report.Summary.Pending;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load maintenance report error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async void LoadRevenueReportAsync()
    {
        IsLoading = true;
        try
        {
            var days = (DateTo - DateFrom).Days;
            if (days <= 0) days = 30;
            var report = await _apiService.GetAsync<RevenueReportResponse>($"reports/revenue?days={days}");
            if (report != null)
            {
                DailyRevenueData = new ObservableCollection<DailyRevenueItem>(report.DailyRevenue);
                MachineRevenueData = new ObservableCollection<MachineRevenueItem>(report.ByMachine);
                PaymentMethodData = new ObservableCollection<PaymentMethodRevenueItem>(report.ByPaymentMethod);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load revenue report error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ExportCsv()
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv",
            FileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                using var writer = new StreamWriter(saveDialog.FileName);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                switch (SelectedTabIndex)
                {
                    case 0: csv.WriteRecords(SalesData); break;
                    case 1: csv.WriteRecords(MaintenanceData); break;
                    case 2: csv.WriteRecords(DailyRevenueData); break;
                }

                MessageBox.Show("Экспорт успешно выполнен", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
