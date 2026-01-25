using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using VendingMachineDesktop.Models;
using VendingMachineDesktop.Services;
using VendingMachineDesktop.Views.Dialogs;

namespace VendingMachineDesktop.ViewModels;

public partial class VendingMachinesViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private List<VendingMachine> _allMachines = new();

    [ObservableProperty]
    private ObservableCollection<VendingMachine> _vendingMachines = new();

    [ObservableProperty]
    private VendingMachine? _selectedVendingMachine;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _pageSize = 10;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalRecords;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isTableView = true;

    [ObservableProperty]
    private bool _isTileView;

    public string PageInfo => $"Показано {((CurrentPage - 1) * PageSize) + 1}-{Math.Min(CurrentPage * PageSize, TotalRecords)} из {TotalRecords}";

    public int StartRecord => TotalRecords > 0 ? ((CurrentPage - 1) * PageSize) + 1 : 0;
    public int EndRecord => Math.Min(CurrentPage * PageSize, TotalRecords);

    public ObservableCollection<int> PageNumbers
    {
        get
        {
            var totalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
            var pages = new ObservableCollection<int>();
            for (int i = 1; i <= totalPages; i++)
            {
                pages.Add(i);
            }
            return pages;
        }
    }

    public VendingMachinesViewModel(IApiService apiService)
    {
        _apiService = apiService;
        LoadDataAsync();
    }

    private async void LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var machines = await _apiService.GetAsync<List<VendingMachine>>("vendingmachines");
            if (machines != null)
            {
                _allMachines = machines;
                ApplyFiltersAndPagination();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        CurrentPage = 1;
        ApplyFiltersAndPagination();
    }

    partial void OnPageSizeChanged(int value)
    {
        CurrentPage = 1;
        ApplyFiltersAndPagination();
    }

    partial void OnIsTableViewChanged(bool value)
    {
        if (value) IsTileView = false;
    }

    partial void OnIsTileViewChanged(bool value)
    {
        if (value) IsTableView = false;
    }

    private void ApplyFiltersAndPagination()
    {
        var filtered = _allMachines.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(m => m.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        TotalRecords = filtered.Count();

        var paged = filtered
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        VendingMachines = new ObservableCollection<VendingMachine>(paged);
        OnPropertyChanged(nameof(PageInfo));
        OnPropertyChanged(nameof(PageNumbers));
        OnPropertyChanged(nameof(StartRecord));
        OnPropertyChanged(nameof(EndRecord));
    }

    [RelayCommand]
    private void Add()
    {
        var dialog = new AddEditVendingMachineDialog();
        if (dialog.ShowDialog() == true)
        {
            LoadDataAsync();
        }
    }

    [RelayCommand]
    private void Edit(VendingMachine? machine = null)
    {
        var machineToEdit = machine ?? SelectedVendingMachine;
        if (machineToEdit == null)
        {
            MessageBox.Show("Выберите автомат для редактирования", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dialog = new AddEditVendingMachineDialog(machineToEdit);
        if (dialog.ShowDialog() == true)
        {
            LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task Delete(VendingMachine? machine = null)
    {
        var machineToDelete = machine ?? SelectedVendingMachine;
        if (machineToDelete == null)
        {
            MessageBox.Show("Выберите автомат для удаления", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"Вы уверены, что хотите удалить автомат '{machineToDelete.Name}'?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _apiService.DeleteAsync($"vendingmachines/{machineToDelete.Id}");
                LoadDataAsync();
                MessageBox.Show("Автомат успешно удален", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private async Task DetachModem(VendingMachine? machine = null)
    {
        var machineToDetach = machine ?? SelectedVendingMachine;
        if (machineToDetach == null)
        {
            MessageBox.Show("Выберите автомат", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrEmpty(machineToDetach.KitOnlineId))
        {
            MessageBox.Show("У данного автомата нет привязанного модема", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var result = MessageBox.Show(
            $"Отвязать модем от автомата '{machineToDetach.Name}'?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _apiService.PostAsync<object, object>($"vendingmachines/{machineToDetach.Id}/detach-modem", new { });
                LoadDataAsync();
                MessageBox.Show("Модем успешно отвязан", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отвязки модема: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void ExportCsv()
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv",
            FileName = $"vending_machines_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                using var writer = new StreamWriter(saveDialog.FileName);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteRecords(_allMachines);
                MessageBox.Show("Экспорт успешно выполнен");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}");
            }
        }
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            ApplyFiltersAndPagination();
        }
    }

    [RelayCommand]
    private void NextPage()
    {
        var totalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
        if (CurrentPage < totalPages)
        {
            CurrentPage++;
            ApplyFiltersAndPagination();
        }
    }

    [RelayCommand]
    private void GoToPage(int pageNumber)
    {
        CurrentPage = pageNumber;
        ApplyFiltersAndPagination();
    }
}
