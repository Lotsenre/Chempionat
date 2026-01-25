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

public partial class CompaniesViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private List<Company> _allCompanies = new();

    [ObservableProperty]
    private ObservableCollection<Company> _companies = new();

    [ObservableProperty]
    private Company? _selectedCompany;

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

    [ObservableProperty]
    private bool _isTileView = false;

    [ObservableProperty]
    private bool _isTableView = true;

    public int StartRecord => TotalRecords == 0 ? 0 : ((CurrentPage - 1) * PageSize) + 1;
    public int EndRecord => Math.Min(CurrentPage * PageSize, TotalRecords);

    public string TotalCountText => $"Всего найдено {TotalRecords} шт.";

    public bool HasNoData => TotalRecords == 0;

    public string PageInfo => $"Показано {StartRecord}-{EndRecord} из {TotalRecords}";

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

    public CompaniesViewModel(IApiService apiService)
    {
        _apiService = apiService;
        LoadDataAsync();
    }

    public async void LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var companies = await _apiService.GetAsync<List<Company>>("companies");
            if (companies != null)
            {
                _allCompanies = companies;
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

    partial void OnIsTileViewChanged(bool value)
    {
        if (value)
        {
            IsTableView = false;
        }
    }

    partial void OnIsTableViewChanged(bool value)
    {
        if (value)
        {
            IsTileView = false;
        }
    }

    private void ApplyFiltersAndPagination()
    {
        var filtered = _allCompanies.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(c =>
                c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                c.Address.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                c.ContactInfo.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        TotalRecords = filtered.Count();

        var paged = filtered
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        Companies = new ObservableCollection<Company>(paged);
        OnPropertyChanged(nameof(PageInfo));
        OnPropertyChanged(nameof(PageNumbers));
        OnPropertyChanged(nameof(TotalCountText));
        OnPropertyChanged(nameof(StartRecord));
        OnPropertyChanged(nameof(EndRecord));
        OnPropertyChanged(nameof(HasNoData));
    }

    [RelayCommand]
    private void Add()
    {
        var dialog = new AddEditCompanyDialog();
        if (dialog.ShowDialog() == true)
        {
            LoadDataAsync();
        }
    }

    [RelayCommand]
    private void Edit(Company? company)
    {
        var companyToEdit = company ?? SelectedCompany;
        if (companyToEdit == null)
        {
            MessageBox.Show("Выберите компанию для редактирования", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dialog = new AddEditCompanyDialog(companyToEdit);
        if (dialog.ShowDialog() == true)
        {
            LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task Delete(Company? company)
    {
        var companyToDelete = company ?? SelectedCompany;
        if (companyToDelete == null)
        {
            MessageBox.Show("Выберите компанию для удаления", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"Вы уверены, что хотите удалить компанию '{companyToDelete.Name}'?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _apiService.DeleteAsync($"companies/{companyToDelete.Id}");
                LoadDataAsync();
                MessageBox.Show("Компания успешно удалена", "Успех",
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
            FileName = $"companies_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                using var writer = new StreamWriter(saveDialog.FileName);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteRecords(_allCompanies);
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
