using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VendingMachineAPI.Models.Entities;

namespace VendingMachineAPI.Utilities;

/// <summary>
/// Генератор JWT токенов для аутентификации пользователей.
/// Использует алгоритм HMAC SHA256 для подписи токенов.
/// </summary>
public class JwtTokenGenerator
{
    /// <summary>
    /// Настройки JWT из конфигурации приложения (appsettings.json).
    /// Содержит SecretKey, Issuer, Audience и ExpirationHours.
    /// </summary>
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Конструктор с внедрением зависимости настроек JWT.
    /// </summary>
    /// <param name="jwtSettings">Настройки JWT из DI контейнера</param>
    public JwtTokenGenerator(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    /// <summary>
    /// Генерирует JWT токен для аутентифицированного пользователя.
    ///
    /// Алгоритм работы:
    /// 1. Создаёт массив Claims (утверждений) с данными пользователя
    /// 2. Генерирует симметричный ключ из SecretKey
    /// 3. Создаёт подпись с использованием HMAC SHA256
    /// 4. Формирует JWT токен с указанными параметрами
    /// 5. Сериализует токен в строку
    ///
    /// Структура Claims в токене:
    /// - Sub (Subject) - уникальный идентификатор пользователя (GUID)
    /// - Email - электронная почта для идентификации
    /// - Role - роль пользователя для авторизации (User, Admin, Manager)
    /// - full_name - полное имя для отображения в интерфейсе
    /// - Jti (JWT ID) - уникальный ID токена для предотвращения повторного использования
    /// </summary>
    /// <param name="user">Сущность пользователя из базы данных</param>
    /// <returns>Строка JWT токена в формате header.payload.signature</returns>
    public string GenerateToken(User user)
    {
        // Создание Claims - данных, зашифрованных в токене
        // Эти данные доступны на клиенте после декодирования токена
        var claims = new[]
        {
            // Sub - стандартный claim для идентификатора субъекта
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            // Email пользователя для быстрого доступа без запроса к БД
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            // Роль для проверки прав доступа на защищённых эндпоинтах
            new Claim(ClaimTypes.Role, user.Role),
            // Кастомный claim с полным именем для UI
            new Claim("full_name", user.FullName),
            // Jti - уникальный ID для каждого токена (защита от replay-атак)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Создание симметричного ключа из строки SecretKey
        // Важно: ключ должен быть минимум 256 бит (32 символа) для HMAC SHA256
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        // SigningCredentials определяют алгоритм подписи токена
        // HMAC SHA256 - симметричный алгоритм, один ключ для подписи и проверки
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Создание JWT токена с указанными параметрами
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,      // Издатель токена (наш API)
            audience: _jwtSettings.Audience,   // Получатель токена (клиентское приложение)
            claims: claims,                    // Данные пользователя
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours), // Время истечения
            signingCredentials: credentials    // Подпись
        );

        // Сериализация токена в компактный формат (три части через точку)
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
