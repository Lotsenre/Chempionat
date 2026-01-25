using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace VendingMachineDesktop.Services;

/// <summary>
/// Сервис для работы с REST API.
///
/// Реализует паттерн HTTP-клиента для взаимодействия с VendingMachineAPI.
/// Предоставляет типизированные методы для CRUD операций.
///
/// Особенности реализации:
/// - Статический токен (_authToken) сохраняется между экземплярами класса,
///   что позволяет сохранять авторизацию при создании новых экземпляров сервиса
/// - JSON сериализация с camelCase (соответствует API)
/// - Автоматическая десериализация ответов в типизированные объекты
/// - Обработка ошибок с выбрасыванием HttpRequestException
///
/// Использование:
/// - Регистрируется как Singleton в DI контейнере (App.xaml.cs)
/// - Внедряется в ViewModel через конструктор
/// </summary>
public class ApiService : IApiService
{
    /// <summary>HTTP клиент для выполнения запросов к API</summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Настройки JSON сериализации:
    /// - PropertyNameCaseInsensitive: игнорировать регистр имён свойств
    /// - CamelCase: преобразовывать PascalCase в camelCase
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Статический токен авторизации.
    ///
    /// ВАЖНО: Используется static для сохранения токена между экземплярами.
    /// При каждом создании нового ApiService токен автоматически применяется.
    /// Это решает проблему потери авторизации при создании новых ViewModel.
    /// </summary>
    private static string? _authToken;

    /// <summary>
    /// Конструктор инициализирует HTTP клиент и JSON настройки.
    ///
    /// Base Address: http://localhost:5000/api/
    /// При наличии сохранённого токена - автоматически добавляет его в заголовки.
    /// </summary>
    public ApiService()
    {
        // Создание HTTP клиента с базовым адресом API
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000/api/")
        };

        // Настройки JSON для корректной работы с API
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,    // "FullName" == "fullName"
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase  // C# PascalCase → JSON camelCase
        };

        // Применение сохранённого токена при создании нового экземпляра
        // Это гарантирует, что все запросы будут авторизованы
        if (!string.IsNullOrEmpty(_authToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authToken);
        }
    }

    /// <summary>
    /// Устанавливает JWT токен для авторизации запросов.
    ///
    /// Вызывается после успешного входа в систему.
    /// Токен сохраняется в статическом поле и добавляется в заголовки HTTP клиента.
    /// </summary>
    /// <param name="token">JWT токен, полученный от API при login</param>
    public void SetAuthToken(string token)
    {
        // Сохранение в статическое поле для новых экземпляров
        _authToken = token;
        // Добавление в заголовки текущего экземпляра
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Выполняет GET запрос к API с автоматической десериализацией ответа.
    ///
    /// Примеры использования:
    /// - await GetAsync&lt;List&lt;VendingMachine&gt;&gt;("vending-machines")
    /// - await GetAsync&lt;DashboardData&gt;("dashboard")
    /// </summary>
    /// <typeparam name="T">Тип ожидаемого ответа (модель или список)</typeparam>
    /// <param name="endpoint">Путь к эндпоинту относительно базового URL (без /api/)</param>
    /// <returns>Десериализованный объект типа T или null</returns>
    /// <exception cref="HttpRequestException">При ошибке API (4xx, 5xx)</exception>
    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);

            // Проверка успешности ответа (200-299)
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error: {response.StatusCode} - {error}");
            }

            // Автоматическая десериализация JSON в объект
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            // Оборачивание исключения с дополнительной информацией
            throw new Exception($"GET request failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Выполняет POST запрос к API с телом запроса и десериализацией ответа.
    ///
    /// Используется для:
    /// - Создания новых сущностей
    /// - Аутентификации (login, register)
    /// - Действий (detach-modem)
    /// </summary>
    /// <typeparam name="TRequest">Тип объекта запроса</typeparam>
    /// <typeparam name="TResponse">Тип ожидаемого ответа</typeparam>
    /// <param name="endpoint">Путь к эндпоинту</param>
    /// <param name="data">Объект данных для отправки (будет сериализован в JSON)</param>
    /// <returns>Десериализованный ответ или null</returns>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            // PostAsJsonAsync автоматически сериализует data в JSON
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonOptions);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error: {response.StatusCode} - {error}");
            }

            return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
        }
        catch (Exception ex)
        {
            throw new Exception($"POST request failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Выполняет PUT запрос к API для обновления существующей сущности.
    ///
    /// Особенности:
    /// - Обрабатывает ответ 204 No Content (возвращает default)
    /// - Десериализует JSON ответ если он есть
    /// </summary>
    /// <typeparam name="TRequest">Тип объекта запроса с данными для обновления</typeparam>
    /// <typeparam name="TResponse">Тип ожидаемого ответа (может быть null при 204)</typeparam>
    /// <param name="endpoint">Путь к эндпоинту (обычно включает ID: vending-machines/{id})</param>
    /// <param name="data">Объект с обновляемыми полями</param>
    /// <returns>Обновлённый объект или default при 204</returns>
    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data, _jsonOptions);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error: {response.StatusCode} - {error}");
            }

            // Обработка пустого ответа (204 No Content)
            // API возвращает 204 при успешном обновлении без тела ответа
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return default;  // Возврат null/default значения
            }

            return JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            throw new Exception($"PUT request failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Выполняет DELETE запрос к API для удаления сущности.
    ///
    /// Возвращает true при успешном удалении (204 No Content).
    /// </summary>
    /// <param name="endpoint">Путь к удаляемому ресурсу (например: vending-machines/{id})</param>
    /// <returns>true при успехе, исключение при ошибке</returns>
    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error: {response.StatusCode} - {error}");
            }

            return true;  // Успешное удаление
        }
        catch (Exception ex)
        {
            throw new Exception($"DELETE request failed: {ex.Message}", ex);
        }
    }
}
