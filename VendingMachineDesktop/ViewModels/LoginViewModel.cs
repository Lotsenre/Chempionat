using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using VendingMachineDesktop.Services;
using VendingMachineDesktop.Views;

namespace VendingMachineDesktop.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Email обязателен";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Пароль обязателен";
            return;
        }

        IsBusy = true;

        try
        {
            var response = await _authService.LoginAsync(Email, Password);

            if (response != null)
            {
                try
                {
                    // Инициализируем RoleHelper для проверки прав на страницах
                    RoleHelper.Initialize(_authService);

                    // Успешная авторизация - открываем главное окно
                    // Передаем текущий AuthService чтобы сохранить данные пользователя
                    var mainWindow = new MainWindow(_authService);
                    mainWindow.Show();

                    // Устанавливаем его как главное окно приложения
                    Application.Current.MainWindow = mainWindow;

                    // Получаем LoginWindow и закрываем его
                    var loginWindow = Application.Current.Windows.OfType<Window>()
                        .FirstOrDefault(w => w.GetType().Name == "LoginWindow");

                    if (loginWindow != null)
                    {
                        loginWindow.Close();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Ошибка открытия окна: {ex.Message}";
                    MessageBox.Show($"Ошибка:\n{ex.Message}\n\nStack trace:\n{ex.StackTrace}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                ErrorMessage = "Неверный email или пароль";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка входа: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
