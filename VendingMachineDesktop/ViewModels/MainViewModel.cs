using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _currentPageName = "Главная";

    [ObservableProperty]
    private object? _currentPage;

    [ObservableProperty]
    private bool _isPaneOpen = true;

    public string UserFullName => _authService.GetCurrentUser()?.FullName ?? "Пользователь";
    public string UserRole => _authService.GetCurrentUser()?.Role ?? "Роль";
    public string UserInitial => !string.IsNullOrEmpty(UserFullName) && UserFullName != "Пользователь"
        ? UserFullName[0].ToString().ToUpper()
        : "А";

    public MainViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    [RelayCommand]
    private void NavigateTo(string pageName)
    {
        CurrentPageName = pageName;
        // В реальной реализации здесь будет навигация по страницам
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.ClearToken();
        // Закрыть главное окно и открыть окно входа
    }
}
