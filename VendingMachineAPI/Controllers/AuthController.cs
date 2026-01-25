using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;
using VendingMachineAPI.Models.DTOs;
using VendingMachineAPI.Utilities;

namespace VendingMachineAPI.Controllers;

/// <summary>
/// Контроллер аутентификации пользователей.
///
/// Обеспечивает функционал:
/// - Регистрация новых пользователей (POST /api/auth/register)
/// - Вход в систему с получением JWT токена (POST /api/auth/login)
///
/// Не требует авторизации ([Authorize] не используется),
/// так как эти эндпоинты должны быть доступны анонимным пользователям.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    /// <summary>Контекст базы данных для работы с таблицей Users</summary>
    private readonly ApplicationDbContext _context;

    /// <summary>Генератор JWT токенов для создания токенов после успешной аутентификации</summary>
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    /// <summary>Логгер для записи событий аутентификации (Serilog)</summary>
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Конструктор с внедрением зависимостей через DI контейнер.
    /// Все зависимости регистрируются в Program.cs.
    /// </summary>
    public AuthController(
        ApplicationDbContext context,
        JwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthController> logger)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Эндпоинт регистрации нового пользователя.
    ///
    /// POST /api/auth/register
    ///
    /// Алгоритм работы:
    /// 1. Логирует попытку регистрации
    /// 2. Проверяет существование пользователя с таким email
    /// 3. Хеширует пароль с помощью BCrypt
    /// 4. Создаёт запись пользователя в БД
    /// 5. Возвращает ID созданного пользователя
    ///
    /// Примечание: Этот эндпоинт предназначен для тестирования и первоначальной настройки.
    /// В production-среде регистрация обычно происходит через админ-панель.
    /// </summary>
    /// <param name="request">Данные для регистрации: Email, Password, FullName, Role</param>
    /// <returns>200 OK с userId при успехе, 400 BadRequest если пользователь существует</returns>
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            return BadRequest(new { message = "User already exists" });
        }

        var user = new Models.Entities.User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            FullName = request.FullName ?? request.Email,
            Role = request.Role ?? "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User registered successfully: {Email}", request.Email);

        return Ok(new { message = "User registered successfully", userId = user.Id });
    }

    /// <summary>
    /// Эндпоинт входа в систему (аутентификация).
    ///
    /// POST /api/auth/login
    ///
    /// Алгоритм работы:
    /// 1. Логирует попытку входа (structured logging с параметром Email)
    /// 2. Ищет пользователя по email в базе данных
    /// 3. Проверяет пароль через BCrypt.Verify (сравнение хешей)
    /// 4. Проверяет активность аккаунта (IsActive = true)
    /// 5. Обновляет время последнего входа (LastLoginAt)
    /// 6. Генерирует JWT токен с данными пользователя
    /// 7. Возвращает токен и информацию о пользователе
    ///
    /// Безопасность:
    /// - Пароль никогда не логируется
    /// - Одинаковое сообщение об ошибке для несуществующего пользователя и неверного пароля
    ///   (предотвращает перебор email-адресов)
    /// - Проверка IsActive предотвращает вход заблокированных пользователей
    /// </summary>
    /// <param name="request">Данные для входа: Email, Password</param>
    /// <returns>
    /// 200 OK с LoginResponse (Token, UserId, Email, FullName, Role) при успехе,
    /// 401 Unauthorized при ошибке аутентификации
    /// </returns>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        // Structured logging - параметры подставляются в шаблон для Serilog
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        // Поиск пользователя по email (уникальное поле в БД)
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        // Проверка существования пользователя
        if (user == null)
        {
            _logger.LogWarning("User not found: {Email}", request.Email);
            // Универсальное сообщение для безопасности (не раскрывает, что email не найден)
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // Проверка пароля через BCrypt
        // BCrypt автоматически извлекает salt из хеша и сравнивает
        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Invalid password for user: {Email}", request.Email);
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // Проверка активности аккаунта
        // Заблокированные пользователи (IsActive = false) не могут войти
        if (!user.IsActive)
        {
            _logger.LogWarning("Inactive user attempted login: {Email}", request.Email);
            return Unauthorized(new { message = "Account is inactive" });
        }

        // Обновление времени последнего входа для отслеживания активности
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Генерация JWT токена с claims пользователя
        var token = _jwtTokenGenerator.GenerateToken(user);

        _logger.LogInformation("User logged in successfully: {Email}", request.Email);

        // Возврат токена и данных пользователя для клиентского приложения
        return Ok(new LoginResponse
        {
            Token = token,           // JWT токен для заголовка Authorization
            UserId = user.Id,        // GUID пользователя
            Email = user.Email,      // Email для отображения
            FullName = user.FullName, // Полное имя для UI
            Role = user.Role         // Роль для проверки прав на клиенте
        });
    }
}
