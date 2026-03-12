using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class AddEditUserDialogViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly bool _isEditMode;
    private readonly UserDto _user;

    public event EventHandler<bool>? CloseRequested;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string? _phone;

    [ObservableProperty]
    private string _selectedRole = "Engineer";

    [ObservableProperty]
    private bool _isBusy;

    public ObservableCollection<string> Roles { get; } = new() { "Admin", "Engineer", "Franchisee" };

    public string DialogTitle => _isEditMode ? "Редактирование пользователя" : "Добавление пользователя";
    public string ButtonText => _isEditMode ? "Сохранить" : "Добавить";
    public bool IsPasswordVisible => !_isEditMode;

    public AddEditUserDialogViewModel(IApiService apiService, UserDto? user = null)
    {
        _apiService = apiService;
        _isEditMode = user != null;
        _user = user ?? new UserDto { Id = Guid.NewGuid() };

        if (_isEditMode)
        {
            FullName = _user.FullName;
            Email = _user.Email;
            Phone = _user.Phone;
            SelectedRole = _user.Role;
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(FullName))
        {
            System.Windows.MessageBox.Show("Укажите ФИО", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            System.Windows.MessageBox.Show("Укажите Email", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (!_isEditMode && string.IsNullOrWhiteSpace(Password))
        {
            System.Windows.MessageBox.Show("Укажите пароль", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        IsBusy = true;
        try
        {
            if (_isEditMode)
            {
                var request = new UpdateUserRequest
                {
                    FullName = FullName,
                    Email = Email,
                    Phone = Phone,
                    Role = SelectedRole
                };
                await _apiService.PutAsync<UpdateUserRequest, object>($"users/{_user.Id}", request);
            }
            else
            {
                var request = new CreateUserRequest
                {
                    FullName = FullName,
                    Email = Email,
                    Password = Password,
                    Phone = Phone,
                    Role = SelectedRole
                };
                await _apiService.PostAsync<CreateUserRequest, UserDto>("users", request);
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
