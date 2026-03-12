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
using VendingMachineDesktop.Views.Dialogs;

namespace VendingMachineDesktop.ViewModels;

public partial class ContractsViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private List<ContractDto> _allContracts = new();

    [ObservableProperty]
    private ObservableCollection<ContractDto> _contracts = new();

    [ObservableProperty]
    private ContractDto? _selectedContract;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _pageSize = 50;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalRecords;

    [ObservableProperty]
    private bool _isLoading;

    public int StartRecord => TotalRecords == 0 ? 0 : ((CurrentPage - 1) * PageSize) + 1;
    public int EndRecord => Math.Min(CurrentPage * PageSize, TotalRecords);
    public string TotalCountText => $"Всего найдено {TotalRecords} шт.";
    public bool HasNoData => TotalRecords == 0;

    public ObservableCollection<int> PageNumbers
    {
        get
        {
            var totalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
            var pages = new ObservableCollection<int>();
            for (int i = 1; i <= totalPages; i++)
                pages.Add(i);
            return pages;
        }
    }

    public ContractsViewModel(IApiService apiService)
    {
        _apiService = apiService;
        LoadDataAsync();
    }

    public async void LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var contracts = await _apiService.GetAsync<List<ContractDto>>("contracts");
            if (contracts != null)
            {
                _allContracts = contracts;
                ApplyFiltersAndPagination();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load contracts error: {ex.Message}");
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

    private void ApplyFiltersAndPagination()
    {
        var filtered = _allContracts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(c =>
                c.ContractNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (c.CompanyName != null && c.CompanyName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                c.VendingMachineName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                c.Status.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        TotalRecords = filtered.Count();

        var paged = filtered
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        Contracts = new ObservableCollection<ContractDto>(paged);
        OnPropertyChanged(nameof(PageNumbers));
        OnPropertyChanged(nameof(TotalCountText));
        OnPropertyChanged(nameof(StartRecord));
        OnPropertyChanged(nameof(EndRecord));
        OnPropertyChanged(nameof(HasNoData));
    }

    [RelayCommand]
    private void Add()
    {
        var dialog = new AddEditContractDialog();
        if (dialog.ShowDialog() == true)
            LoadDataAsync();
    }

    [RelayCommand]
    private void Edit(ContractDto? contract)
    {
        var c = contract ?? SelectedContract;
        if (c == null)
        {
            MessageBox.Show("Выберите контракт для редактирования", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var dialog = new AddEditContractDialog(c);
        if (dialog.ShowDialog() == true)
            LoadDataAsync();
    }

    [RelayCommand]
    private async Task Delete(ContractDto? contract)
    {
        var c = contract ?? SelectedContract;
        if (c == null)
        {
            MessageBox.Show("Выберите контракт для удаления", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"Вы уверены, что хотите удалить контракт '{c.ContractNumber}'?",
            "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _apiService.DeleteAsync($"contracts/{c.Id}");
                LoadDataAsync();
                MessageBox.Show("Контракт успешно удален", "Успех",
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
    private async Task Sign(ContractDto? contract)
    {
        var c = contract ?? SelectedContract;
        if (c == null) return;

        if (c.Status == "Signed")
        {
            MessageBox.Show("Контракт уже подписан", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var result = MessageBox.Show(
            $"Подписать контракт '{c.ContractNumber}'?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _apiService.PostAsync<object, object>($"contracts/{c.Id}/sign", new { });
                LoadDataAsync();
                MessageBox.Show("Контракт подписан", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
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
            FileName = $"contracts_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                using var writer = new StreamWriter(saveDialog.FileName);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteRecords(_allContracts);
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

    [RelayCommand]
    private void PreviousPage()
    {
        if (CurrentPage > 1) { CurrentPage--; ApplyFiltersAndPagination(); }
    }

    [RelayCommand]
    private void NextPage()
    {
        var totalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
        if (CurrentPage < totalPages) { CurrentPage++; ApplyFiltersAndPagination(); }
    }

    [RelayCommand]
    private void GoToPage(int pageNumber)
    {
        CurrentPage = pageNumber;
        ApplyFiltersAndPagination();
    }
}
