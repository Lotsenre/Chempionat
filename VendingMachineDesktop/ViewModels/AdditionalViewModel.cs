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

public partial class AdditionalViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private int _selectedTabIndex;

    // News
    [ObservableProperty]
    private ObservableCollection<NewsDto> _news = new();

    [ObservableProperty]
    private NewsDto? _selectedNews;

    // Contracts
    [ObservableProperty]
    private ObservableCollection<ContractDto> _contracts = new();

    [ObservableProperty]
    private ContractDto? _selectedContract;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public bool HasNoNews => News.Count == 0;
    public bool HasNoContracts => Contracts.Count == 0;

    public AdditionalViewModel(IApiService apiService)
    {
        _apiService = apiService;
        LoadNewsAsync();
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        switch (value)
        {
            case 0: LoadNewsAsync(); break;
            case 1: LoadContractsAsync(); break;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        // Client-side filter already loaded data
    }

    public async void LoadNewsAsync()
    {
        IsLoading = true;
        try
        {
            var news = await _apiService.GetAsync<List<NewsDto>>("news");
            if (news != null)
            {
                News = new ObservableCollection<NewsDto>(news);
                OnPropertyChanged(nameof(HasNoNews));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load news error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async void LoadContractsAsync()
    {
        IsLoading = true;
        try
        {
            var contracts = await _apiService.GetAsync<List<ContractDto>>("contracts");
            if (contracts != null)
            {
                Contracts = new ObservableCollection<ContractDto>(contracts);
                OnPropertyChanged(nameof(HasNoContracts));
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

    // News commands
    [RelayCommand]
    private void AddNews()
    {
        var dialog = new AddEditNewsDialog();
        if (dialog.ShowDialog() == true)
            LoadNewsAsync();
    }

    [RelayCommand]
    private void EditNews(NewsDto? news)
    {
        var n = news ?? SelectedNews;
        if (n == null)
        {
            MessageBox.Show("Выберите новость для редактирования", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var dialog = new AddEditNewsDialog(n);
        if (dialog.ShowDialog() == true)
            LoadNewsAsync();
    }

    [RelayCommand]
    private async Task DeleteNews(NewsDto? news)
    {
        var n = news ?? SelectedNews;
        if (n == null)
        {
            MessageBox.Show("Выберите новость для удаления", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"Вы уверены, что хотите удалить новость '{n.Title}'?",
            "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _apiService.DeleteAsync($"news/{n.Id}");
                LoadNewsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // Contract commands
    [RelayCommand]
    private void AddContract()
    {
        var dialog = new AddEditContractDialog();
        if (dialog.ShowDialog() == true)
            LoadContractsAsync();
    }

    [RelayCommand]
    private void EditContract(ContractDto? contract)
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
            LoadContractsAsync();
    }

    [RelayCommand]
    private async Task DeleteContract(ContractDto? contract)
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
                LoadContractsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private async Task SignContract(ContractDto? contract)
    {
        var c = contract ?? SelectedContract;
        if (c == null) return;

        var result = MessageBox.Show(
            $"Подписать контракт '{c.ContractNumber}'?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _apiService.PostAsync<object, object>($"contracts/{c.Id}/sign", new { });
                LoadContractsAsync();
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
            FileName = SelectedTabIndex == 0
                ? $"news_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                : $"contracts_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                using var writer = new StreamWriter(saveDialog.FileName);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                if (SelectedTabIndex == 0)
                    csv.WriteRecords(News);
                else
                    csv.WriteRecords(Contracts);

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
