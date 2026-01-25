# Как создать REST API сервер на ASP.NET Core 8 с нуля

Это пошаговое руководство для начинающих разработчиков. Мы создадим полноценный API сервер с авторизацией, базой данных и документацией.

---

## Содержание

1. [Что мы будем создавать](#1-что-мы-будем-создавать)
2. [Необходимые инструменты](#2-необходимые-инструменты)
3. [Создание проекта](#3-создание-проекта)
4. [Структура проекта](#4-структура-проекта)
5. [Установка NuGet пакетов](#5-установка-nuget-пакетов)
6. [Настройка конфигурации](#6-настройка-конфигурации)
7. [Создание моделей данных](#7-создание-моделей-данных)
8. [Создание контекста базы данных](#8-создание-контекста-базы-данных)
9. [Создание DTO классов](#9-создание-dto-классов)
10. [Создание утилит](#10-создание-утилит)
11. [Настройка Program.cs](#11-настройка-programcs)
12. [Создание контроллеров](#12-создание-контроллеров)
13. [Создание базы данных](#13-создание-базы-данных)
14. [Запуск и тестирование](#14-запуск-и-тестирование)
15. [Частые ошибки и решения](#15-частые-ошибки-и-решения)

---

## 1. Что мы будем создавать

**REST API** — это сервер, который принимает HTTP запросы и возвращает данные в формате JSON.

Наш API будет иметь:
- **Авторизацию** через JWT токены (безопасный вход в систему)
- **Базу данных** PostgreSQL (хранение данных)
- **CRUD операции** (Создание, Чтение, Обновление, Удаление)
- **Swagger документацию** (удобное тестирование API в браузере)
- **Логирование** (запись действий для отладки)

### Технологии:
| Технология | Для чего |
|------------|----------|
| ASP.NET Core 8 | Фреймворк для создания API |
| Entity Framework Core | Работа с базой данных |
| PostgreSQL | База данных |
| JWT | Авторизация |
| BCrypt | Хеширование паролей |
| Swagger | Документация API |
| Serilog | Логирование |

---

## 2. Необходимые инструменты

### 2.1 Установите .NET SDK 8.0

1. Перейдите на https://dotnet.microsoft.com/download
2. Скачайте **.NET 8.0 SDK**
3. Установите и перезагрузите компьютер

**Проверка установки:**
```bash
dotnet --version
```
Должно показать версию 8.x.x

### 2.2 Установите PostgreSQL

1. Перейдите на https://www.postgresql.org/download/
2. Скачайте и установите PostgreSQL
3. Запомните пароль для пользователя `postgres`!

### 2.3 Установите редактор кода

Рекомендую один из:
- **Visual Studio 2022** (полная IDE, бесплатная Community версия)
- **Visual Studio Code** + расширение C# Dev Kit
- **JetBrains Rider** (платная, но очень мощная)

---

## 3. Создание проекта

### 3.1 Откройте терминал/командную строку

**Windows:** Win + R → cmd или PowerShell
**Mac/Linux:** Terminal

### 3.2 Перейдите в папку для проекта

```bash
cd C:\Users\ВашеИмя\Desktop
```

### 3.3 Создайте новый проект

```bash
dotnet new webapi -n VendingMachineAPI
cd VendingMachineAPI
```

Команда `dotnet new webapi` создаёт шаблон Web API проекта.

### 3.4 Откройте проект в редакторе

```bash
code .
```
(для VS Code)

---

## 4. Структура проекта

Создайте следующую структуру папок:

```
VendingMachineAPI/
├── Controllers/          # Контроллеры (обработчики запросов)
├── Models/
│   ├── Entities/         # Модели базы данных
│   └── DTOs/             # Объекты передачи данных
├── Data/                 # Контекст базы данных
├── Utilities/            # Вспомогательные классы
├── Logs/                 # Папка для логов (создастся автоматически)
├── Program.cs            # Точка входа и настройка
├── appsettings.json      # Конфигурация
└── VendingMachineAPI.csproj  # Файл проекта
```

**Создание папок через терминал:**
```bash
mkdir Controllers Models Data Utilities
mkdir Models\Entities Models\DTOs
```

---

## 5. Установка NuGet пакетов

NuGet — это менеджер пакетов для .NET (аналог npm для Node.js).

### 5.1 Откройте файл VendingMachineAPI.csproj

Замените содержимое на:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <!-- PostgreSQL - работа с базой данных -->
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />

    <!-- JWT Authentication - авторизация -->
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />

    <!-- BCrypt - хеширование паролей -->
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />

    <!-- Swagger - документация API -->
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />

    <!-- Serilog - логирование -->
    <PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />

    <!-- EF Core Tools - инструменты для миграций -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

</Project>
```

### 5.2 Восстановите пакеты

```bash
dotnet restore
```

**Что делает каждый пакет:**
- `Npgsql.EntityFrameworkCore.PostgreSQL` — подключение к PostgreSQL
- `Microsoft.AspNetCore.Authentication.JwtBearer` — проверка JWT токенов
- `BCrypt.Net-Next` — безопасное хеширование паролей
- `Swashbuckle.AspNetCore` — генерация Swagger документации
- `Serilog.*` — структурированное логирование в консоль и файлы

---

## 6. Настройка конфигурации

### 6.1 Создайте файл appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=vending_db;Username=postgres;Password=ВАШ_ПАРОЛЬ"
  },
  "JwtSettings": {
    "SecretKey": "МойСуперСекретныйКлючДляJWTМинимум32Символа!",
    "Issuer": "VendingMachineAPI",
    "Audience": "VendingMachineClient",
    "ExpirationHours": 24
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  },
  "AllowedHosts": "*"
}
```

**ВАЖНО:**
1. Замените `ВАШ_ПАРОЛЬ` на пароль от PostgreSQL
2. `SecretKey` должен быть минимум 32 символа!
3. Никогда не храните секреты в Git! (добавьте `appsettings.json` в `.gitignore` для production)

### Что означают настройки:

| Параметр | Описание |
|----------|----------|
| `ConnectionStrings:DefaultConnection` | Строка подключения к PostgreSQL |
| `JwtSettings:SecretKey` | Секретный ключ для подписи токенов |
| `JwtSettings:Issuer` | Кто выдал токен (наш API) |
| `JwtSettings:Audience` | Для кого токен (клиент) |
| `JwtSettings:ExpirationHours` | Время жизни токена в часах |

---

## 7. Создание моделей данных

Модели — это классы, которые представляют таблицы в базе данных.

### 7.1 Модель User (Пользователь)

Создайте файл `Models/Entities/User.cs`:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachineAPI.Models.Entities;

/// <summary>
/// Модель пользователя системы.
/// Атрибут [Table] указывает имя таблицы в базе данных.
/// </summary>
[Table("users")]
public class User
{
    /// <summary>
    /// Уникальный идентификатор (первичный ключ).
    /// Guid генерируется автоматически и уникален глобально.
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Полное имя пользователя.
    /// [Required] - обязательное поле (NOT NULL в БД).
    /// [MaxLength] - максимальная длина строки.
    /// </summary>
    [Required]
    [Column("full_name")]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email пользователя (используется для входа).
    /// </summary>
    [Required]
    [Column("email")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Телефон (опционально).
    /// Знак ? означает, что поле может быть null.
    /// </summary>
    [Column("phone")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    /// <summary>
    /// Хеш пароля (не сам пароль!).
    /// BCrypt создаёт хеш, который нельзя расшифровать обратно.
    /// </summary>
    [Column("password_hash")]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Роль пользователя: User, Admin, Manager и т.д.
    /// </summary>
    [Required]
    [Column("role")]
    [MaxLength(50)]
    public string Role { get; set; } = "User";

    /// <summary>
    /// Активен ли аккаунт.
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Дата создания аккаунта.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления.
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего входа.
    /// </summary>
    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }
}
```

### 7.2 Модель VendingMachine (Торговый автомат)

Создайте файл `Models/Entities/VendingMachine.cs`:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VendingMachineAPI.Models.Entities;

/// <summary>
/// Модель торгового автомата.
/// </summary>
[Table("vending_machines")]
public class VendingMachine
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Серийный номер (уникальный).
    /// </summary>
    [Required]
    [Column("serial_number")]
    [MaxLength(50)]
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Название автомата.
    /// </summary>
    [Required]
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Модель автомата.
    /// </summary>
    [Required]
    [Column("model")]
    [MaxLength(255)]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Статус: Working, NotWorking, OnMaintenance.
    /// </summary>
    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Working";

    /// <summary>
    /// Адрес установки.
    /// </summary>
    [Required]
    [Column("location")]
    [MaxLength(500)]
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Место (например: "1 этаж, у входа").
    /// </summary>
    [Column("place")]
    [MaxLength(500)]
    public string? Place { get; set; }

    /// <summary>
    /// GPS координаты.
    /// </summary>
    [Column("coordinates")]
    [MaxLength(100)]
    public string? Coordinates { get; set; }

    /// <summary>
    /// Дата установки.
    /// </summary>
    [Required]
    [Column("install_date")]
    public DateTime InstallDate { get; set; }

    /// <summary>
    /// Дата последнего обслуживания.
    /// </summary>
    [Column("last_maintenance_date")]
    public DateTime? LastMaintenanceDate { get; set; }

    /// <summary>
    /// Режим работы (например: "08:00-22:00").
    /// </summary>
    [Column("working_hours")]
    [MaxLength(50)]
    public string? WorkingHours { get; set; }

    /// <summary>
    /// Часовой пояс.
    /// </summary>
    [Column("timezone")]
    [MaxLength(50)]
    public string? Timezone { get; set; }

    /// <summary>
    /// Общий доход.
    /// [Precision] указывает точность для decimal.
    /// </summary>
    [Column("total_income")]
    [Precision(18, 2)]
    public decimal TotalIncome { get; set; } = 0;

    /// <summary>
    /// Заметки.
    /// </summary>
    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

### Разбор атрибутов:

| Атрибут | Описание |
|---------|----------|
| `[Table("имя")]` | Имя таблицы в БД |
| `[Key]` | Первичный ключ |
| `[Column("имя")]` | Имя колонки в БД |
| `[Required]` | NOT NULL |
| `[MaxLength(n)]` | Максимальная длина |
| `[Precision(p, s)]` | Точность decimal (p цифр, s после запятой) |

---

## 8. Создание контекста базы данных

DbContext — это "мост" между кодом C# и базой данных.

### 8.1 Создайте файл Data/ApplicationDbContext.cs

```csharp
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Models.Entities;

namespace VendingMachineAPI.Data;

/// <summary>
/// Контекст базы данных.
/// Наследуется от DbContext — базового класса Entity Framework.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Конструктор получает настройки подключения через DI.
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet представляет таблицу в базе данных.
    /// Через него мы делаем запросы к таблице Users.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Таблица торговых автоматов.
    /// </summary>
    public DbSet<VendingMachine> VendingMachines { get; set; }

    /// <summary>
    /// Метод вызывается при создании модели БД.
    /// Здесь мы настраиваем индексы, связи и ограничения.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка таблицы Users
        modelBuilder.Entity<User>(entity =>
        {
            // Уникальный индекс на Email (два пользователя не могут иметь одинаковый email)
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Настройка таблицы VendingMachines
        modelBuilder.Entity<VendingMachine>(entity =>
        {
            // Уникальный индекс на SerialNumber
            entity.HasIndex(e => e.SerialNumber).IsUnique();
        });
    }
}
```

### Что делает DbContext:
1. **Отслеживает изменения** — помнит, какие объекты вы изменили
2. **Генерирует SQL** — преобразует LINQ запросы в SQL
3. **Управляет транзакциями** — SaveChanges() сохраняет всё атомарно

---

## 9. Создание DTO классов

**DTO (Data Transfer Object)** — объекты для передачи данных между клиентом и сервером.

Зачем нужны DTO:
- Не передавать лишние поля (например, хеш пароля)
- Валидация входных данных
- Разделение внутренней структуры от внешнего API

### 9.1 LoginRequest.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace VendingMachineAPI.Models.DTOs;

/// <summary>
/// Запрос на вход в систему.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Email пользователя.
    /// [EmailAddress] проверяет корректность формата.
    /// </summary>
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль.
    /// </summary>
    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; } = string.Empty;
}
```

### 9.2 LoginResponse.cs

```csharp
namespace VendingMachineAPI.Models.DTOs;

/// <summary>
/// Ответ при успешном входе.
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT токен для авторизации.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// ID пользователя.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Email пользователя.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Роль пользователя.
    /// </summary>
    public string Role { get; set; } = string.Empty;
}
```

### 9.3 RegisterRequest.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace VendingMachineAPI.Models.DTOs;

/// <summary>
/// Запрос на регистрацию.
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен быть минимум 6 символов")]
    public string Password { get; set; } = string.Empty;

    public string? FullName { get; set; }

    public string? Role { get; set; }
}
```

### 9.4 VendingMachineDto.cs

```csharp
namespace VendingMachineAPI.Models.DTOs;

/// <summary>
/// DTO для торгового автомата.
/// </summary>
public class VendingMachineDto
{
    public Guid Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Place { get; set; }
    public string? Coordinates { get; set; }
    public DateTime InstallDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public string? WorkingHours { get; set; }
    public string? Timezone { get; set; }
    public decimal TotalIncome { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Запрос на создание автомата.
/// </summary>
public class CreateVendingMachineRequest
{
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Place { get; set; }
    public string? Coordinates { get; set; }
    public DateTime InstallDate { get; set; }
    public string? WorkingHours { get; set; }
    public string? Timezone { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Запрос на обновление автомата.
/// Все поля опциональные — обновляются только переданные.
/// </summary>
public class UpdateVendingMachineRequest
{
    public string? Name { get; set; }
    public string? SerialNumber { get; set; }
    public string? Model { get; set; }
    public string? Status { get; set; }
    public string? Location { get; set; }
    public string? Place { get; set; }
    public string? Coordinates { get; set; }
    public string? WorkingHours { get; set; }
    public string? Timezone { get; set; }
    public string? Notes { get; set; }
}
```

---

## 10. Создание утилит

### 10.1 JwtSettings.cs (настройки JWT)

```csharp
namespace VendingMachineAPI.Utilities;

/// <summary>
/// Класс для маппинга настроек JWT из appsettings.json.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Секретный ключ для подписи токенов.
    /// ВАЖНО: Минимум 32 символа для HMAC SHA256!
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Издатель токена (наш API).
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Получатель токена (клиентское приложение).
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Время жизни токена в часах.
    /// </summary>
    public int ExpirationHours { get; set; }
}
```

### 10.2 JwtTokenGenerator.cs (генератор токенов)

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VendingMachineAPI.Models.Entities;

namespace VendingMachineAPI.Utilities;

/// <summary>
/// Генератор JWT токенов.
///
/// JWT токен состоит из трёх частей:
/// 1. Header (заголовок) — алгоритм и тип токена
/// 2. Payload (данные) — Claims пользователя
/// 3. Signature (подпись) — проверка подлинности
///
/// Формат: xxxxx.yyyyy.zzzzz
/// </summary>
public class JwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    /// <summary>
    /// Генерирует JWT токен для пользователя.
    /// </summary>
    /// <param name="user">Пользователь из базы данных</param>
    /// <returns>Строка JWT токена</returns>
    public string GenerateToken(User user)
    {
        // Claims — это "утверждения" о пользователе, которые хранятся в токене
        var claims = new[]
        {
            // Sub (Subject) — ID пользователя
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            // Email пользователя
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            // Роль для авторизации
            new Claim(ClaimTypes.Role, user.Role),
            // Имя пользователя
            new Claim("full_name", user.FullName),
            // Уникальный ID токена (защита от повторного использования)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Создаём ключ из секретной строки
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        // Указываем алгоритм подписи — HMAC SHA256
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Создаём токен
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,           // Кто выдал
            audience: _jwtSettings.Audience,        // Для кого
            claims: claims,                         // Данные пользователя
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours), // Когда истечёт
            signingCredentials: credentials         // Подпись
        );

        // Преобразуем в строку
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### 10.3 PasswordHasher.cs (хеширование паролей)

```csharp
namespace VendingMachineAPI.Utilities;

/// <summary>
/// Утилита для хеширования паролей с помощью BCrypt.
///
/// НИКОГДА не храните пароли в открытом виде!
/// BCrypt автоматически добавляет "соль" (случайные данные),
/// поэтому два одинаковых пароля дадут разные хеши.
/// </summary>
public static class PasswordHasher
{
    /// <summary>
    /// Хеширует пароль.
    /// </summary>
    /// <param name="password">Исходный пароль</param>
    /// <returns>Хеш пароля (60 символов)</returns>
    public static string HashPassword(string password)
    {
        // BCrypt.HashPassword автоматически генерирует соль
        // и включает её в результат
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Проверяет пароль на соответствие хешу.
    /// </summary>
    /// <param name="password">Введённый пароль</param>
    /// <param name="hash">Хеш из базы данных</param>
    /// <returns>true если пароль верный</returns>
    public static bool VerifyPassword(string password, string hash)
    {
        // BCrypt извлекает соль из хеша и сравнивает
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

---

## 11. Настройка Program.cs

Это главный файл приложения. Здесь мы настраиваем все сервисы и middleware.

### 11.1 Замените содержимое Program.cs

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using VendingMachineAPI.Data;
using VendingMachineAPI.Utilities;

// ==================== СОЗДАНИЕ BUILDER ====================
var builder = WebApplication.CreateBuilder(args);

// ==================== НАСТРОЙКА SERILOG ====================
// Serilog — продвинутая система логирования
// Настройки читаются из appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Заменяем стандартное логирование на Serilog
builder.Host.UseSerilog();

// ==================== ДОБАВЛЕНИЕ СЕРВИСОВ ====================

// Добавляем поддержку контроллеров
builder.Services.AddControllers();

// Добавляем OpenAPI/Swagger для документации
builder.Services.AddEndpointsApiExplorer();

// ==================== НАСТРОЙКА SWAGGER ====================
builder.Services.AddSwaggerGen(c =>
{
    // Основная информация об API
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vending Machine API",
        Version = "v1",
        Description = "API для управления торговыми автоматами"
    });

    // Добавляем поддержку JWT авторизации в Swagger UI
    // Это позволит тестировать защищённые эндпоинты
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT авторизация. Введите 'Bearer' [пробел] и ваш токен",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ==================== НАСТРОЙКА POSTGRESQL ====================
// Entity Framework будет использовать строку подключения из appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==================== НАСТРОЙКА JWT ====================
// Читаем настройки JWT из конфигурации
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (jwtSettings == null)
    throw new InvalidOperationException("JWT Settings не настроены в appsettings.json");

// Регистрируем JwtSettings как Singleton (один экземпляр на всё приложение)
builder.Services.AddSingleton(jwtSettings);
// Регистрируем генератор токенов
builder.Services.AddSingleton<JwtTokenGenerator>();

// Настраиваем аутентификацию через JWT Bearer
builder.Services.AddAuthentication(options =>
{
    // Схема по умолчанию для аутентификации
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // Схема по умолчанию для "вызова" (когда требуется авторизация)
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Параметры валидации токена
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,           // Проверять издателя
        ValidateAudience = true,         // Проверять получателя
        ValidateLifetime = true,         // Проверять срок действия
        ValidateIssuerSigningKey = true, // Проверять подпись
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        // Ключ для проверки подписи
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

// Добавляем авторизацию
builder.Services.AddAuthorization();

// ==================== НАСТРОЙКА CORS ====================
// CORS (Cross-Origin Resource Sharing) — разрешение запросов с других доменов
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Разрешить любой домен
              .AllowAnyMethod()   // Разрешить любой HTTP метод
              .AllowAnyHeader();  // Разрешить любые заголовки
    });
});

// ==================== СОЗДАНИЕ ПРИЛОЖЕНИЯ ====================
var app = builder.Build();

// ==================== НАСТРОЙКА MIDDLEWARE PIPELINE ====================
// Middleware выполняется по порядку для каждого запроса

// Swagger доступен только в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vending Machine API v1");
        c.RoutePrefix = string.Empty; // Swagger на главной странице
    });
}

// Логирование всех HTTP запросов
app.UseSerilogRequestLogging();

// Перенаправление HTTP на HTTPS
app.UseHttpsRedirection();

// Применяем политику CORS
app.UseCors("AllowAll");

// ВАЖНО: Authentication должен быть ДО Authorization!
app.UseAuthentication(); // Определяет КТО пользователь
app.UseAuthorization();  // Определяет ЧТО пользователь может делать

// Маршрутизация к контроллерам
app.MapControllers();

// ==================== ЗАПУСК ПРИЛОЖЕНИЯ ====================
try
{
    Log.Information("Запуск Vending Machine API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение завершилось с ошибкой");
}
finally
{
    Log.CloseAndFlush();
}
```

### Порядок Middleware важен!

```
Запрос → UseSerilogRequestLogging → UseHttpsRedirection → UseCors
       → UseAuthentication → UseAuthorization → Контроллер → Ответ
```

---

## 12. Создание контроллеров

Контроллеры обрабатывают HTTP запросы и возвращают ответы.

### 12.1 AuthController.cs (аутентификация)

Создайте файл `Controllers/AuthController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;
using VendingMachineAPI.Models.DTOs;
using VendingMachineAPI.Models.Entities;
using VendingMachineAPI.Utilities;

namespace VendingMachineAPI.Controllers;

/// <summary>
/// Контроллер аутентификации.
///
/// [ApiController] — включает автоматическую валидацию и форматирование ответов.
/// [Route("api/[controller]")] — маршрут: api/auth
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Конструктор. Зависимости внедряются автоматически через DI.
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
    /// Регистрация нового пользователя.
    ///
    /// POST /api/auth/register
    ///
    /// Body:
    /// {
    ///     "email": "user@example.com",
    ///     "password": "secret123",
    ///     "fullName": "Иван Иванов"
    /// }
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Попытка регистрации: {Email}", request.Email);

        // Проверяем, не занят ли email
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            return BadRequest(new { message = "Пользователь с таким email уже существует" });
        }

        // Создаём нового пользователя
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            // Хешируем пароль перед сохранением!
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            FullName = request.FullName ?? request.Email,
            Role = request.Role ?? "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Добавляем в контекст и сохраняем
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Пользователь зарегистрирован: {Email}", request.Email);

        return Ok(new { message = "Регистрация успешна", userId = user.Id });
    }

    /// <summary>
    /// Вход в систему.
    ///
    /// POST /api/auth/login
    ///
    /// Body:
    /// {
    ///     "email": "user@example.com",
    ///     "password": "secret123"
    /// }
    ///
    /// Response:
    /// {
    ///     "token": "eyJhbGciOiJIUzI1NiIs...",
    ///     "userId": "...",
    ///     "email": "user@example.com",
    ///     "fullName": "Иван Иванов",
    ///     "role": "User"
    /// }
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Попытка входа: {Email}", request.Email);

        // Ищем пользователя по email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        // Пользователь не найден
        if (user == null)
        {
            _logger.LogWarning("Пользователь не найден: {Email}", request.Email);
            // Не говорим, что именно неверно — это защита от перебора
            return Unauthorized(new { message = "Неверный email или пароль" });
        }

        // Проверяем пароль
        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Неверный пароль для: {Email}", request.Email);
            return Unauthorized(new { message = "Неверный email или пароль" });
        }

        // Проверяем, активен ли аккаунт
        if (!user.IsActive)
        {
            _logger.LogWarning("Попытка входа в неактивный аккаунт: {Email}", request.Email);
            return Unauthorized(new { message = "Аккаунт заблокирован" });
        }

        // Обновляем время последнего входа
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Генерируем JWT токен
        var token = _jwtTokenGenerator.GenerateToken(user);

        _logger.LogInformation("Успешный вход: {Email}", request.Email);

        // Возвращаем токен и данные пользователя
        return Ok(new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        });
    }
}
```

### 12.2 VendingMachinesController.cs (CRUD)

Создайте файл `Controllers/VendingMachinesController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;
using VendingMachineAPI.Models.DTOs;
using VendingMachineAPI.Models.Entities;

namespace VendingMachineAPI.Controllers;

/// <summary>
/// Контроллер для работы с торговыми автоматами.
///
/// [Authorize] — все методы требуют авторизации (JWT токен).
/// Маршрут: api/vendingmachines
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Требуется JWT токен для всех методов
public class VendingMachinesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VendingMachinesController> _logger;

    public VendingMachinesController(
        ApplicationDbContext context,
        ILogger<VendingMachinesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Получить список всех автоматов с пагинацией и поиском.
    ///
    /// GET /api/vendingmachines?page=1&pageSize=10&search=кофе
    ///
    /// Параметры:
    /// - page: номер страницы (начиная с 1)
    /// - pageSize: количество записей на странице
    /// - search: строка поиска (по имени, серийному номеру, адресу)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VendingMachineDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        // Начинаем запрос
        var query = _context.VendingMachines.AsQueryable();

        // Применяем поиск, если указана строка
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(vm =>
                vm.Name.Contains(search) ||
                vm.SerialNumber.Contains(search) ||
                vm.Location.Contains(search));
        }

        // Считаем общее количество (для пагинации)
        var total = await query.CountAsync();

        // Применяем пагинацию и преобразуем в DTO
        var machines = await query
            .Skip((page - 1) * pageSize)  // Пропускаем записи предыдущих страниц
            .Take(pageSize)                // Берём только pageSize записей
            .Select(vm => new VendingMachineDto
            {
                Id = vm.Id,
                SerialNumber = vm.SerialNumber,
                Name = vm.Name,
                Model = vm.Model,
                Status = vm.Status,
                Location = vm.Location,
                Place = vm.Place,
                Coordinates = vm.Coordinates,
                InstallDate = vm.InstallDate,
                LastMaintenanceDate = vm.LastMaintenanceDate,
                WorkingHours = vm.WorkingHours,
                Timezone = vm.Timezone,
                TotalIncome = vm.TotalIncome,
                Notes = vm.Notes
            })
            .ToListAsync();

        // Добавляем заголовок с общим количеством
        Response.Headers.Append("X-Total-Count", total.ToString());

        return Ok(machines);
    }

    /// <summary>
    /// Получить автомат по ID.
    ///
    /// GET /api/vendingmachines/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VendingMachineDto>> GetById(Guid id)
    {
        var machine = await _context.VendingMachines
            .Where(vm => vm.Id == id)
            .Select(vm => new VendingMachineDto
            {
                Id = vm.Id,
                SerialNumber = vm.SerialNumber,
                Name = vm.Name,
                Model = vm.Model,
                Status = vm.Status,
                Location = vm.Location,
                Place = vm.Place,
                Coordinates = vm.Coordinates,
                InstallDate = vm.InstallDate,
                LastMaintenanceDate = vm.LastMaintenanceDate,
                WorkingHours = vm.WorkingHours,
                Timezone = vm.Timezone,
                TotalIncome = vm.TotalIncome,
                Notes = vm.Notes
            })
            .FirstOrDefaultAsync();

        if (machine == null)
            return NotFound(new { message = "Автомат не найден" });

        return Ok(machine);
    }

    /// <summary>
    /// Создать новый автомат.
    ///
    /// POST /api/vendingmachines
    ///
    /// Body:
    /// {
    ///     "name": "Кофейный автомат #1",
    ///     "serialNumber": "VM-001",
    ///     "model": "CoffeeMaster 3000",
    ///     "location": "г. Москва, ул. Ленина, 1"
    /// }
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VendingMachineDto>> Create(
        [FromBody] CreateVendingMachineRequest request)
    {
        // Создаём новую сущность
        var machine = new VendingMachine
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            SerialNumber = request.SerialNumber,
            Model = request.Model,
            Location = request.Location,
            Place = request.Place,
            Coordinates = request.Coordinates,
            InstallDate = request.InstallDate,
            WorkingHours = request.WorkingHours,
            Timezone = request.Timezone,
            Notes = request.Notes,
            Status = "Working",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.VendingMachines.Add(machine);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Создан автомат: {Id}, Имя: {Name}", machine.Id, machine.Name);

        // Преобразуем в DTO для ответа
        var result = new VendingMachineDto
        {
            Id = machine.Id,
            SerialNumber = machine.SerialNumber,
            Name = machine.Name,
            Model = machine.Model,
            Status = machine.Status,
            Location = machine.Location,
            Place = machine.Place,
            InstallDate = machine.InstallDate
        };

        // 201 Created с Location header
        return CreatedAtAction(nameof(GetById), new { id = machine.Id }, result);
    }

    /// <summary>
    /// Обновить автомат.
    ///
    /// PUT /api/vendingmachines/{id}
    ///
    /// Обновляются только переданные поля (partial update).
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateVendingMachineRequest request)
    {
        var machine = await _context.VendingMachines.FindAsync(id);

        if (machine == null)
            return NotFound(new { message = "Автомат не найден" });

        // Обновляем только переданные поля
        if (!string.IsNullOrEmpty(request.Name))
            machine.Name = request.Name;
        if (!string.IsNullOrEmpty(request.SerialNumber))
            machine.SerialNumber = request.SerialNumber;
        if (!string.IsNullOrEmpty(request.Model))
            machine.Model = request.Model;
        if (!string.IsNullOrEmpty(request.Status))
            machine.Status = request.Status;
        if (!string.IsNullOrEmpty(request.Location))
            machine.Location = request.Location;
        if (request.Place != null)
            machine.Place = request.Place;
        if (request.Coordinates != null)
            machine.Coordinates = request.Coordinates;
        if (request.WorkingHours != null)
            machine.WorkingHours = request.WorkingHours;
        if (request.Timezone != null)
            machine.Timezone = request.Timezone;
        if (request.Notes != null)
            machine.Notes = request.Notes;

        machine.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Обновлён автомат: {Id}", id);

        return NoContent(); // 204 No Content
    }

    /// <summary>
    /// Удалить автомат.
    ///
    /// DELETE /api/vendingmachines/{id}
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var machine = await _context.VendingMachines.FindAsync(id);

        if (machine == null)
            return NotFound(new { message = "Автомат не найден" });

        _context.VendingMachines.Remove(machine);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Удалён автомат: {Id}", id);

        return NoContent(); // 204 No Content
    }
}
```

### Разбор атрибутов контроллеров:

| Атрибут | Описание |
|---------|----------|
| `[ApiController]` | Включает автоматическую валидацию модели |
| `[Route("api/[controller]")]` | Базовый маршрут (имя контроллера без "Controller") |
| `[Authorize]` | Требует JWT токен |
| `[HttpGet]` | Обрабатывает GET запросы |
| `[HttpPost]` | Обрабатывает POST запросы |
| `[HttpPut("{id}")]` | PUT запрос с параметром id в URL |
| `[HttpDelete("{id}")]` | DELETE запрос |
| `[FromBody]` | Параметр из тела запроса (JSON) |
| `[FromQuery]` | Параметр из query string (?param=value) |

---

## 13. Создание базы данных

### 13.1 Установите EF Core Tools

```bash
dotnet tool install --global dotnet-ef
```

### 13.2 Создайте базу данных в PostgreSQL

**Вариант 1: Через pgAdmin**
1. Откройте pgAdmin
2. ПКМ на "Databases" → Create → Database
3. Введите имя: `vending_db`
4. Нажмите Save

**Вариант 2: Через командную строку**
```bash
psql -U postgres
CREATE DATABASE vending_db;
\q
```

### 13.3 Создайте миграцию

Миграция — это код, который создаёт/изменяет структуру БД.

```bash
dotnet ef migrations add InitialCreate
```

Будет создана папка `Migrations` с файлами миграции.

### 13.4 Примените миграцию

```bash
dotnet ef database update
```

Это создаст таблицы `users` и `vending_machines` в базе данных.

### Команды для работы с миграциями:

| Команда | Описание |
|---------|----------|
| `dotnet ef migrations add ИмяМиграции` | Создать новую миграцию |
| `dotnet ef database update` | Применить все миграции |
| `dotnet ef database update ИмяМиграции` | Откатить/применить до конкретной |
| `dotnet ef migrations remove` | Удалить последнюю миграцию |
| `dotnet ef migrations list` | Список всех миграций |

---

## 14. Запуск и тестирование

### 14.1 Запустите приложение

```bash
dotnet run
```

Вы увидите:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7XXX
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5XXX
```

### 14.2 Откройте Swagger

Перейдите в браузере на https://localhost:7XXX (или http://localhost:5XXX)

Вы увидите Swagger UI с документацией всех эндпоинтов.

### 14.3 Протестируйте API

**Шаг 1: Регистрация**

1. Найдите `POST /api/auth/register`
2. Нажмите "Try it out"
3. Введите тело запроса:
```json
{
  "email": "admin@test.com",
  "password": "password123",
  "fullName": "Admin User",
  "role": "Admin"
}
```
4. Нажмите "Execute"
5. Должен вернуться статус 200 OK

**Шаг 2: Вход**

1. Найдите `POST /api/auth/login`
2. Введите:
```json
{
  "email": "admin@test.com",
  "password": "password123"
}
```
3. Скопируйте `token` из ответа

**Шаг 3: Авторизация в Swagger**

1. Нажмите кнопку "Authorize" (замок вверху страницы)
2. Введите: `Bearer ВАШ_ТОКЕН`
3. Нажмите "Authorize"

**Шаг 4: Создание автомата**

1. Найдите `POST /api/vendingmachines`
2. Введите:
```json
{
  "name": "Кофейный автомат #1",
  "serialNumber": "VM-001",
  "model": "CoffeeMaster 3000",
  "location": "г. Москва, ул. Ленина, 1",
  "installDate": "2024-01-15T10:00:00Z"
}
```
3. Нажмите "Execute"

**Шаг 5: Получение списка**

1. Найдите `GET /api/vendingmachines`
2. Нажмите "Execute"
3. Вы увидите созданный автомат

---

## 15. Частые ошибки и решения

### Ошибка: "Connection refused" (PostgreSQL)

**Причина:** PostgreSQL не запущен или неверные данные подключения.

**Решение:**
1. Проверьте, что PostgreSQL запущен (Windows: Services → postgresql)
2. Проверьте пароль в appsettings.json
3. Проверьте порт (по умолчанию 5432)

### Ошибка: "Invalid token"

**Причина:** Неверный или истёкший JWT токен.

**Решение:**
1. Убедитесь, что вводите `Bearer ` перед токеном (с пробелом)
2. Получите новый токен через /api/auth/login
3. Проверьте SecretKey в appsettings.json (минимум 32 символа)

### Ошибка: "401 Unauthorized"

**Причина:** Запрос к защищённому эндпоинту без токена.

**Решение:**
1. Сначала выполните login
2. Добавьте токен в заголовок Authorization

### Ошибка миграции: "No DbContext was found"

**Причина:** EF Core не может найти DbContext.

**Решение:**
```bash
dotnet ef migrations add InitialCreate --project VendingMachineAPI
```

### Ошибка: "A table with this name already exists"

**Причина:** Таблица уже существует в БД.

**Решение:**
1. Удалите базу данных: `DROP DATABASE vending_db;`
2. Создайте заново
3. Примените миграции

---

## Дополнительные материалы

### Полезные ссылки:
- [Документация ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [JWT.io](https://jwt.io) — для декодирования токенов

### Следующие шаги:
1. Добавить больше моделей (Product, Sale, Maintenance)
2. Реализовать роли и политики авторизации
3. Добавить валидацию с FluentValidation
4. Настроить HTTPS сертификат для production
5. Добавить Docker для деплоя

---

## Структура готового проекта

```
VendingMachineAPI/
├── Controllers/
│   ├── AuthController.cs
│   └── VendingMachinesController.cs
├── Models/
│   ├── Entities/
│   │   ├── User.cs
│   │   └── VendingMachine.cs
│   └── DTOs/
│       ├── LoginRequest.cs
│       ├── LoginResponse.cs
│       ├── RegisterRequest.cs
│       └── VendingMachineDto.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Utilities/
│   ├── JwtSettings.cs
│   ├── JwtTokenGenerator.cs
│   └── PasswordHasher.cs
├── Migrations/
│   └── (файлы миграций)
├── Logs/
│   └── log-YYYYMMDD.txt
├── Program.cs
├── appsettings.json
└── VendingMachineAPI.csproj
```

---

**Поздравляю!** Вы создали полноценный REST API с нуля.

Этот API включает:
- JWT авторизацию
- Хеширование паролей
- PostgreSQL базу данных
- CRUD операции
- Swagger документацию
- Структурированное логирование
- Пагинацию и поиск
