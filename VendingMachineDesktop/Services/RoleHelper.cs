using System.ComponentModel;

namespace VendingMachineDesktop.Services;

/// <summary>
/// Синглтон-хелпер для проверки ролей текущего пользователя.
/// Используется в XAML для управления видимостью CRUD-кнопок через x:Static.
/// </summary>
public class RoleHelper : INotifyPropertyChanged
{
    private static readonly RoleHelper _instance = new();
    public static RoleHelper Instance => _instance;

    private IAuthService? _authService;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Инициализация хелпера с экземпляром AuthService.
    /// Вызывается один раз при успешном логине.
    /// </summary>
    public static void Initialize(IAuthService authService)
    {
        _instance._authService = authService;
        _instance.NotifyAll();
    }

    public string CurrentRole => _authService?.GetCurrentUser()?.Role ?? "";

    public bool IsAdmin => CurrentRole == "Admin";
    public bool IsEngineer => CurrentRole == "Engineer";
    public bool IsFranchisee => CurrentRole == "Franchisee";

    /// <summary>Admin + Engineer могут редактировать</summary>
    public bool CanEdit => IsAdmin || IsEngineer;

    /// <summary>Только Admin может удалять</summary>
    public bool CanDelete => IsAdmin;

    /// <summary>Admin + Engineer — доступ к отчетам, ТМЦ, ТА, модемам</summary>
    public bool CanAccessReports => IsAdmin || IsEngineer;

    /// <summary>Только Admin — компании, контракты, пользователи</summary>
    public bool CanAccessAdminOnly => IsAdmin;

    private void NotifyAll()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentRole)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAdmin)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEngineer)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFranchisee)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanEdit)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanDelete)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanAccessReports)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanAccessAdminOnly)));
    }
}
