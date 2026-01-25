using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VendingMachineDesktop.Models;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class AddEditCompanyDialogViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly bool _isEditMode;
    private readonly Company _company;

    public event EventHandler<bool>? CloseRequested;

    [ObservableProperty]
    private ObservableCollection<Company> _parentCompanies = new();

    [ObservableProperty]
    private Company? _selectedParentCompany;

    [ObservableProperty]
    private string _companyName = string.Empty;

    [ObservableProperty]
    private string _companyAddress = string.Empty;

    [ObservableProperty]
    private string _companyContacts = string.Empty;

    [ObservableProperty]
    private string _companyNotes = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public string DialogTitle => _isEditMode ? "Редактирование компании" : "Добавление компании";
    public string ButtonText => _isEditMode ? "Сохранить" : "Добавить";

    public AddEditCompanyDialogViewModel(IApiService apiService, Company? company = null)
    {
        _apiService = apiService;
        _isEditMode = company != null;
        _company = company ?? new Company
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            WorkingSince = DateTime.Now
        };

        // Initialize fields from company
        if (_isEditMode)
        {
            CompanyName = _company.Name;
            CompanyAddress = _company.Address;
            CompanyContacts = _company.ContactInfo;
            CompanyNotes = _company.Notes;
        }

        LoadParentCompaniesAsync();
    }

    private async void LoadParentCompaniesAsync()
    {
        try
        {
            var companies = await _apiService.GetAsync<List<Company>>("companies");
            if (companies != null)
            {
                // Exclude current company from parent list
                var availableParents = companies.Where(c => c.Id != _company.Id).ToList();
                ParentCompanies = new ObservableCollection<Company>(availableParents);

                if (_isEditMode && _company.ParentCompanyId.HasValue)
                {
                    SelectedParentCompany = ParentCompanies.FirstOrDefault(c => c.Id == _company.ParentCompanyId);
                }
                else if (!_isEditMode && ParentCompanies.Count > 0)
                {
                    // Select first parent by default for new companies
                    SelectedParentCompany = ParentCompanies.FirstOrDefault();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load parent companies error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        // Validation
        if (SelectedParentCompany == null)
        {
            System.Windows.MessageBox.Show("Выберите вышестоящую компанию", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(CompanyName))
        {
            System.Windows.MessageBox.Show("Укажите название компании", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(CompanyAddress))
        {
            System.Windows.MessageBox.Show("Укажите адрес компании", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(CompanyContacts))
        {
            System.Windows.MessageBox.Show("Укажите контакты компании", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        IsBusy = true;
        try
        {
            // Create DTO for API
            var dto = new CompanyDto
            {
                Id = _company.Id,
                Name = CompanyName,
                Address = CompanyAddress,
                ContactInfo = CompanyContacts,
                Notes = CompanyNotes ?? string.Empty,
                ParentCompanyId = SelectedParentCompany.Id,
                ParentCompanyName = SelectedParentCompany.Name,
                WorkingSince = _isEditMode ? _company.WorkingSince : DateTime.UtcNow,
                CreatedAt = _isEditMode ? _company.CreatedAt : DateTime.UtcNow
            };

            if (_isEditMode)
            {
                await _apiService.PutAsync<CompanyDto, CompanyDto>($"companies/{_company.Id}", dto);
            }
            else
            {
                await _apiService.PostAsync<CompanyDto, CompanyDto>("companies", dto);
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
