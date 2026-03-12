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

    // ── Роли ──────────────────────────────────────────
    public bool IsAdmin => UserRole == "Admin";
    public bool IsEngineer => UserRole == "Engineer";
    public bool IsFranchisee => UserRole == "Franchisee";

    // ── Видимость разделов ────────────────────────────
    /// <summary>Отчеты, ТМЦ, ТА, Модемы — Admin + Engineer</summary>
    public bool CanAccessReports => IsAdmin || IsEngineer;
    public bool CanAccessInventory => IsAdmin || IsEngineer;
    public bool CanAccessVendingMachines => IsAdmin || IsEngineer;
    public bool CanAccessModems => IsAdmin || IsEngineer;

    /// <summary>Компании, Контракты, Пользователи — только Admin</summary>
    public bool CanAccessCompanies => IsAdmin;
    public bool CanAccessContracts => IsAdmin;
    public bool CanAccessUsers => IsAdmin;

    /// <summary>Показывать раздел «Администрирование» если есть хотя бы один подпункт</summary>
    public bool CanAccessAdmin => IsAdmin || IsEngineer;

    /// <summary>Можно ли редактировать/удалять данные (Admin + Engineer)</summary>
    public bool CanEdit => IsAdmin || IsEngineer;

    /// <summary>Можно ли удалять данные (только Admin)</summary>
    public bool CanDelete => IsAdmin;

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
