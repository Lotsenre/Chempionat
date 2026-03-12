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

public partial class ModemsViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private List<ModemDto> _allModems = new();

    [ObservableProperty]
    private ObservableCollection<ModemDto> _modems = new();

    [ObservableProperty]
    private ModemDto? _selectedModem;

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

    public ModemsViewModel(IApiService apiService)
    {
        _apiService = apiService;
        LoadDataAsync();
    }

    public async void LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var modems = await _apiService.GetAsync<List<ModemDto>>("modems");
            if (modems != null)
            {
                _allModems = modems;
                ApplyFiltersAndPagination();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load modems error: {ex.Message}");
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
        var filtered = _allModems.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(m =>
                m.ModemNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (m.Model != null && m.Model.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                (m.VendingMachineName != null && m.VendingMachineName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }

        TotalRecords = filtered.Count();

        var paged = filtered
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        Modems = new ObservableCollection<ModemDto>(paged);
        OnPropertyChanged(nameof(PageNumbers));
        OnPropertyChanged(nameof(TotalCountText));
        OnPropertyChanged(nameof(StartRecord));
        OnPropertyChanged(nameof(EndRecord));
        OnPropertyChanged(nameof(HasNoData));
    }

    [RelayCommand]
    private void Add()
    {
        var dialog = new AddEditModemDialog();
        if (dialog.ShowDialog() == true)
            LoadDataAsync();
    }

    [RelayCommand]
    private void Edit(ModemDto? modem)
    {
        var m = modem ?? SelectedModem;
        if (m == null)
        {
            MessageBox.Show("Выберите модем для редактирования", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var dialog = new AddEditModemDialog(m);
        if (dialog.ShowDialog() == true)
            LoadDataAsync();
    }

    [RelayCommand]
    private async Task Delete(ModemDto? modem)
    {
        var m = modem ?? SelectedModem;
        if (m == null)
        {
            MessageBox.Show("Выберите модем для удаления", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"Вы уверены, что хотите удалить модем '{m.ModemNumber}'?",
            "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _apiService.DeleteAsync($"modems/{m.Id}");
                LoadDataAsync();
                MessageBox.Show("Модем успешно удален", "Успех",
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
    private void ExportCsv()
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv",
            FileName = $"modems_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                using var writer = new StreamWriter(saveDialog.FileName);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteRecords(_allModems);
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
