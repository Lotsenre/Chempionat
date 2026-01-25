using VendingMachineDesktop.Models.DTOs;

namespace VendingMachineDesktop.Services;

/// <summary>
/// Сервис аутентификации пользователей.
///
/// Управляет процессом входа в систему и хранением данных текущего пользователя.
/// Интегрируется с ApiService для выполнения запросов к API.
///
/// Ответственности:
/// - Выполнение входа через API
/// - Хранение JWT токена
/// - Хранение данных текущего пользователя (FullName, Role, Email)
/// - Передача токена в ApiService для авторизации последующих запросов
///
/// Жизненный цикл:
/// 1. LoginAsync() → получение токена от API
/// 2. SetToken() → сохранение токена
/// 3. SetCurrentUser() → сохранение данных пользователя
/// 4. GetCurrentUser() → использование в UI для отображения имени/роли
/// 5. ClearToken() → выход из системы
///
/// Использование:
/// - Регистрируется как Singleton в DI контейнере
/// - Внедряется в LoginViewModel и MainViewModel
/// </summary>
public class AuthService : IAuthService
{
    /// <summary>Сервис API для выполнения HTTP запросов</summary>
    private readonly IApiService _apiService;

    /// <summary>JWT токен текущей сессии</summary>
    private string? _token;

    /// <summary>
    /// Данные текущего пользователя (из ответа login).
    /// Содержит: Token, UserId, Email, FullName, Role
    /// </summary>
    private LoginResponse? _currentUser;

    /// <summary>
    /// Конструктор с внедрением зависимости ApiService.
    /// </summary>
    /// <param name="apiService">Сервис для работы с API</param>
    public AuthService(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <summary>
    /// Выполняет асинхронный вход в систему.
    ///
    /// Алгоритм:
    /// 1. Формирует LoginRequest с email и паролем
    /// 2. Отправляет POST запрос на auth/login
    /// 3. При успехе сохраняет токен и данные пользователя
    /// 4. Передаёт токен в ApiService для авторизации будущих запросов
    ///
    /// После успешного входа:
    /// - GetToken() вернёт JWT токен
    /// - GetCurrentUser() вернёт данные пользователя
    /// - Все запросы через ApiService будут авторизованы
    /// </summary>
    /// <param name="email">Email пользователя</param>
    /// <param name="password">Пароль</param>
    /// <returns>LoginResponse с токеном и данными при успехе, null при ошибке</returns>
    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        // Формирование объекта запроса
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        // Отправка запроса на API
        var response = await _apiService.PostAsync<LoginRequest, LoginResponse>("auth/login", request);

        // При успешном входе сохраняем данные
        if (response != null)
        {
            SetToken(response.Token);           // Сохранение токена в AuthService
            SetCurrentUser(response);           // Сохранение данных пользователя
            _apiService.SetAuthToken(response.Token);  // Передача токена в ApiService
        }

        return response;
    }

    /// <summary>Получение текущего JWT токена</summary>
    public string? GetToken() => _token;

    /// <summary>Установка JWT токена</summary>
    public void SetToken(string token) => _token = token;

    /// <summary>
    /// Очистка данных сессии (выход из системы).
    /// Вызывается при logout для сброса состояния.
    /// </summary>
    public void ClearToken()
    {
        _token = null;
        _currentUser = null;
    }

    /// <summary>Получение данных текущего пользователя</summary>
    public LoginResponse? GetCurrentUser() => _currentUser;

    /// <summary>Установка данных текущего пользователя</summary>
    public void SetCurrentUser(LoginResponse user) => _currentUser = user;
}
