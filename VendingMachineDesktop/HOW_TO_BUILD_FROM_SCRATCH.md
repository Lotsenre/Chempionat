# Руководство по созданию WPF приложения VendingMachineDesktop с нуля

Данное руководство описывает полный процесс создания десктоп-приложения для управления вендинговыми автоматами с использованием WPF, .NET 8.0 и паттерна MVVM.

---

## Содержание

1. [Обзор приложения](#1-обзор-приложения)
2. [Требования к системе](#2-требования-к-системе)
3. [Установка необходимого ПО](#3-установка-необходимого-по)
4. [Создание проекта](#4-создание-проекта)
5. [Установка NuGet-пакетов](#5-установка-nuget-пакетов)
6. [Структура проекта](#6-структура-проекта)
7. [Создание моделей данных](#7-создание-моделей-данных)
8. [Создание сервисов](#8-создание-сервисов)
9. [Создание ViewModels](#9-создание-viewmodels)
10. [Создание Views (XAML)](#10-создание-views-xaml)
11. [Настройка Dependency Injection](#11-настройка-dependency-injection)
12. [Настройка Material Design](#12-настройка-material-design)
13. [Запуск приложения](#13-запуск-приложения)
14. [Полезные команды](#14-полезные-команды)

---

## 1. Обзор приложения

### Что это за приложение?

VendingMachineDesktop — это десктоп-приложение для управления сетью вендинговых автоматов. Оно позволяет:

- **Авторизоваться** через JWT-токены
- **Мониторить** состояние вендинговых автоматов в реальном времени
- **Управлять** автоматами (CRUD операции)
- **Управлять** компаниями-клиентами
- **Просматривать** дашборд с аналитикой и KPI
- **Экспортировать** данные в CSV

### Технологический стек

| Технология | Версия | Назначение |
|------------|--------|------------|
| .NET | 8.0 | Платформа |
| WPF | - | UI Framework |
| CommunityToolkit.Mvvm | 8.4.0 | MVVM инфраструктура |
| MaterialDesignThemes | 5.3.0 | UI компоненты |
| MahApps.Metro | 2.4.11 | Современные WPF контролы |
| Newtonsoft.Json | 13.0.4 | JSON сериализация |
| CsvHelper | 33.1.0 | Экспорт в CSV |
| LiveChartsCore | 2.0.0-rc2.2 | Графики и диаграммы |

### Архитектура

```
┌─────────────────────────────────────────────────────────────┐
│                        Views (XAML)                         │
│    LoginWindow │ MainWindow │ Pages │ Dialogs              │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      ViewModels                             │
│  LoginVM │ MainVM │ DashboardVM │ VendingMachinesVM │ etc. │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                       Services                              │
│            ApiService (HTTP) │ AuthService (JWT)            │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                     REST API Backend                        │
│                  http://localhost:5000/api/                 │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. Требования к системе

### Минимальные требования

- **ОС**: Windows 10 версии 1903 или выше / Windows 11
- **RAM**: 4 ГБ (рекомендуется 8 ГБ)
- **Диск**: 5 ГБ свободного места
- **Интернет**: Для загрузки пакетов

### Требуемое ПО

1. **.NET 8.0 SDK** — для компиляции и запуска
2. **Visual Studio 2022** или **VS Code** — IDE для разработки
3. **Git** — для контроля версий (опционально)

---

## 3. Установка необходимого ПО

### Шаг 3.1: Установка .NET 8.0 SDK

#### Windows (через winget)
```powershell
winget install Microsoft.DotNet.SDK.8
```

#### Windows (ручная установка)
1. Перейдите на https://dotnet.microsoft.com/download/dotnet/8.0
2. Скачайте **SDK 8.0.x** для Windows (x64)
3. Запустите установщик и следуйте инструкциям

#### Проверка установки
```powershell
dotnet --version
# Должно вывести: 8.0.xxx
```

### Шаг 3.2: Установка Visual Studio 2022

1. Скачайте **Visual Studio 2022 Community** (бесплатная) с https://visualstudio.microsoft.com/
2. В установщике выберите рабочую нагрузку:
   - **".NET Desktop Development"** (обязательно!)
3. Убедитесь, что выбран компонент:
   - Windows 10 SDK или выше
4. Нажмите "Установить"

### Шаг 3.3: Альтернатива — VS Code

```powershell
# Установка VS Code
winget install Microsoft.VisualStudioCode

# Установка расширения C#
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.csdevkit
```

---

## 4. Создание проекта

### Шаг 4.1: Создание WPF проекта

```powershell
# Перейдите в нужную директорию
cd C:\Projects

# Создайте новый WPF проект
dotnet new wpf -n VendingMachineDesktop -f net8.0

# Перейдите в папку проекта
cd VendingMachineDesktop
```

### Шаг 4.2: Создание структуры папок

```powershell
# Создание основных папок
mkdir Models
mkdir Models\DTOs
mkdir ViewModels
mkdir Views
mkdir Views\Pages
mkdir Views\Dialogs
mkdir Services
mkdir Utilities
mkdir Utilities\Converters
mkdir Resources
mkdir Assets
mkdir Assets\Icons
```

### Шаг 4.3: Настройка файла проекта

Откройте `VendingMachineDesktop.csproj` и замените содержимое:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWPF>true</UseWPF>
    <ApplicationIcon>Resources\Logo.ico</ApplicationIcon>
  </PropertyGroup>

  <!-- NuGet пакеты будут добавлены на следующем шаге -->

  <ItemGroup>
    <Resource Include="Resources\**\*" />
    <Resource Include="Assets\**\*" />
  </ItemGroup>

</Project>
```

---

## 5. Установка NuGet-пакетов

### Шаг 5.1: Установка через командную строку

Выполните все команды в папке проекта:

```powershell
# MVVM Framework (обязательно!)
dotnet add package CommunityToolkit.Mvvm --version 8.4.0

# Material Design UI
dotnet add package MaterialDesignThemes --version 5.3.0
dotnet add package MaterialDesignColors --version 5.3.0

# Современные WPF контролы
dotnet add package MahApps.Metro --version 2.4.11

# Графики и диаграммы
dotnet add package LiveChartsCore.SkiaSharpView.WPF --version 2.0.0-rc2.2

# CSV экспорт
dotnet add package CsvHelper --version 33.1.0

# JSON сериализация
dotnet add package Newtonsoft.Json --version 13.0.4

# Dependency Injection
dotnet add package Microsoft.Extensions.DependencyInjection --version 10.0.1

# HTTP JSON расширения
dotnet add package System.Net.Http.Json --version 10.0.1

# Логирование
dotnet add package Serilog.Sinks.File --version 7.0.0
```

### Шаг 5.2: Проверка установки

```powershell
dotnet restore
dotnet build
```

После установки ваш `.csproj` файл должен содержать секцию `<ItemGroup>` с PackageReference.

---

## 6. Структура проекта

После создания всех файлов, структура должна выглядеть так:

```
VendingMachineDesktop/
│
├── App.xaml                          # Ресурсы приложения
├── App.xaml.cs                       # Точка входа и DI
├── AssemblyInfo.cs                   # Информация о сборке
├── VendingMachineDesktop.csproj      # Файл проекта
│
├── Models/                           # Модели данных
│   ├── User.cs
│   ├── Company.cs
│   ├── VendingMachine.cs
│   ├── VendingMachineMonitor.cs
│   ├── Dashboard.cs
│   └── DTOs/                         # Data Transfer Objects
│       ├── LoginRequest.cs
│       ├── LoginResponse.cs
│       ├── VendingMachineDto.cs
│       └── CompanyDto.cs
│
├── ViewModels/                       # Логика представления
│   ├── LoginViewModel.cs
│   ├── MainViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── VendingMachinesViewModel.cs
│   ├── CompaniesViewModel.cs
│   └── MonitorViewModel.cs
│
├── Views/                            # Пользовательский интерфейс
│   ├── LoginWindow.xaml / .xaml.cs
│   ├── MainWindow.xaml / .xaml.cs
│   ├── Pages/
│   │   ├── DashboardPage.xaml / .xaml.cs
│   │   ├── VendingMachinesPage.xaml / .xaml.cs
│   │   ├── CompaniesPage.xaml / .xaml.cs
│   │   └── MonitorPage.xaml / .xaml.cs
│   └── Dialogs/
│       ├── AddEditVendingMachineDialog.xaml / .xaml.cs
│       └── AddEditCompanyDialog.xaml / .xaml.cs
│
├── Services/                         # Сервисы
│   ├── IApiService.cs               # Интерфейс API
│   ├── ApiService.cs                # Реализация HTTP клиента
│   ├── IAuthService.cs              # Интерфейс авторизации
│   └── AuthService.cs               # Реализация JWT auth
│
├── Utilities/
│   └── Converters/                  # XAML конвертеры
│       ├── BoolToVisibilityConverter.cs
│       ├── InverseBooleanConverter.cs
│       └── StatusToColorConverter.cs
│
├── Resources/                        # Ресурсы (изображения)
│   └── Logo.png
│
└── Assets/
    └── Icons/                        # Иконки
        ├── Hardware/
        ├── Modem/
        └── States/
```

---

## 7. Создание моделей данных

### Шаг 7.1: Создание базовых моделей

#### `Models/User.cs`
```csharp
namespace VendingMachineDesktop.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
```

#### `Models/Company.cs`
```csharp
namespace VendingMachineDesktop.Models;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Inn { get; set; } = string.Empty;
    public string Kpp { get; set; } = string.Empty;
    public string Ogrn { get; set; } = string.Empty;
    public string LegalAddress { get; set; } = string.Empty;
    public string ActualAddress { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string Bik { get; set; } = string.Empty;
    public string CheckingAccount { get; set; } = string.Empty;
    public string CorrespondentAccount { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int UserId { get; set; }
    public int MachineCount { get; set; }
}
```

#### `Models/VendingMachine.cs`
```csharp
namespace VendingMachineDesktop.Models;

public class VendingMachine
{
    // Идентификация
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;

    // Статус и местоположение
    public string Status { get; set; } = "Active";
    public string Location { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Coordinates { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;

    // Даты
    public DateTime? InstallDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public string WorkingHours { get; set; } = string.Empty;

    // Конфигурация
    public string WorkMode { get; set; } = string.Empty;
    public string ProductMatrix { get; set; } = string.Empty;
    public string CriticalThresholdTemplate { get; set; } = string.Empty;
    public string NotificationTemplate { get; set; } = string.Empty;

    // Персонал
    public string Client { get; set; } = string.Empty;
    public string Manager { get; set; } = string.Empty;
    public string Engineer { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string Technician { get; set; } = string.Empty;

    // Платёжные системы
    public string CoinAcceptor { get; set; } = string.Empty;
    public string BillAcceptor { get; set; } = string.Empty;
    public string CashlessModule { get; set; } = string.Empty;
    public string QRPayment { get; set; } = string.Empty;

    // Связи
    public int UserId { get; set; }
    public int? CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;

    // Финансы
    public decimal TotalIncome { get; set; }
}
```

### Шаг 7.2: Создание DTO

#### `Models/DTOs/LoginRequest.cs`
```csharp
namespace VendingMachineDesktop.Models.DTOs;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

#### `Models/DTOs/LoginResponse.cs`
```csharp
namespace VendingMachineDesktop.Models.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
}

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
```

---

## 8. Создание сервисов

### Шаг 8.1: Интерфейс авторизации

#### `Services/IAuthService.cs`
```csharp
using VendingMachineDesktop.Models;
using VendingMachineDesktop.Models.DTOs;

namespace VendingMachineDesktop.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(string email, string password);
    void Logout();
    bool IsAuthenticated { get; }
    User? CurrentUser { get; }
    string? Token { get; }
}
```

### Шаг 8.2: Реализация авторизации

#### `Services/AuthService.cs`
```csharp
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using VendingMachineDesktop.Models;
using VendingMachineDesktop.Models.DTOs;

namespace VendingMachineDesktop.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://localhost:5000/api/";

    private static string? _token;
    private static User? _currentUser;

    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
    public User? CurrentUser => _currentUser;
    public string? Token => _token;

    public AuthService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl)
        };
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        try
        {
            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var loginResponse = await response.Content
                    .ReadFromJsonAsync<LoginResponse>(options);

                if (loginResponse != null)
                {
                    _token = loginResponse.Token;
                    _currentUser = new User
                    {
                        Id = loginResponse.User.Id,
                        Email = loginResponse.User.Email,
                        Name = loginResponse.User.Name,
                        Role = loginResponse.User.Role
                    };
                }

                return loginResponse;
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            return null;
        }
    }

    public void Logout()
    {
        _token = null;
        _currentUser = null;
    }
}
```

### Шаг 8.3: Интерфейс API сервиса

#### `Services/IApiService.cs`
```csharp
using VendingMachineDesktop.Models;

namespace VendingMachineDesktop.Services;

public interface IApiService
{
    void SetAuthToken(string token);

    // Vending Machines
    Task<List<VendingMachine>> GetVendingMachinesAsync();
    Task<VendingMachine?> GetVendingMachineAsync(int id);
    Task<bool> CreateVendingMachineAsync(VendingMachine machine);
    Task<bool> UpdateVendingMachineAsync(VendingMachine machine);
    Task<bool> DeleteVendingMachineAsync(int id);

    // Companies
    Task<List<Company>> GetCompaniesAsync();
    Task<Company?> GetCompanyAsync(int id);
    Task<bool> CreateCompanyAsync(Company company);
    Task<bool> UpdateCompanyAsync(Company company);
    Task<bool> DeleteCompanyAsync(int id);
}
```

### Шаг 8.4: Реализация API сервиса

#### `Services/ApiService.cs`
```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using VendingMachineDesktop.Models;

namespace VendingMachineDesktop.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://localhost:5000/api/";
    private static string? _authToken;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl)
        };

        if (!string.IsNullOrEmpty(_authToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authToken);
        }
    }

    public void SetAuthToken(string token)
    {
        _authToken = token;
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    #region Vending Machines

    public async Task<List<VendingMachine>> GetVendingMachinesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("vendingmachines");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content
                    .ReadFromJsonAsync<List<VendingMachine>>(_jsonOptions)
                    ?? new List<VendingMachine>();
            }
            return new List<VendingMachine>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            return new List<VendingMachine>();
        }
    }

    public async Task<VendingMachine?> GetVendingMachineAsync(int id)
    {
        try
        {
            return await _httpClient
                .GetFromJsonAsync<VendingMachine>($"vendingmachines/{id}", _jsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> CreateVendingMachineAsync(VendingMachine machine)
    {
        try
        {
            var response = await _httpClient
                .PostAsJsonAsync("vendingmachines", machine, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateVendingMachineAsync(VendingMachine machine)
    {
        try
        {
            var response = await _httpClient
                .PutAsJsonAsync($"vendingmachines/{machine.Id}", machine, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteVendingMachineAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"vendingmachines/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Companies

    public async Task<List<Company>> GetCompaniesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("companies");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content
                    .ReadFromJsonAsync<List<Company>>(_jsonOptions)
                    ?? new List<Company>();
            }
            return new List<Company>();
        }
        catch
        {
            return new List<Company>();
        }
    }

    public async Task<Company?> GetCompanyAsync(int id)
    {
        try
        {
            return await _httpClient
                .GetFromJsonAsync<Company>($"companies/{id}", _jsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> CreateCompanyAsync(Company company)
    {
        try
        {
            var response = await _httpClient
                .PostAsJsonAsync("companies", company, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateCompanyAsync(Company company)
    {
        try
        {
            var response = await _httpClient
                .PutAsJsonAsync($"companies/{company.Id}", company, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteCompanyAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"companies/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    #endregion
}
```

---

## 9. Создание ViewModels

### Шаг 9.1: LoginViewModel

#### `ViewModels/LoginViewModel.cs`
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasError;

    public event Action? LoginSuccessful;

    public LoginViewModel(IAuthService authService, IApiService apiService)
    {
        _authService = authService;
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Введите email и пароль";
            HasError = true;
            return;
        }

        IsLoading = true;
        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _authService.LoginAsync(Email, Password);

            if (result != null)
            {
                _apiService.SetAuthToken(result.Token);
                LoginSuccessful?.Invoke();
            }
            else
            {
                ErrorMessage = "Неверный email или пароль";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка подключения: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

### Шаг 9.2: MainViewModel

#### `ViewModels/MainViewModel.cs`
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VendingMachineDesktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private object? _currentPage;

    [ObservableProperty]
    private string _currentPageTitle = "Дашборд";

    [ObservableProperty]
    private int _selectedMenuIndex = 0;

    public event Action? LogoutRequested;

    [RelayCommand]
    private void NavigateTo(string pageName)
    {
        CurrentPageTitle = pageName switch
        {
            "Dashboard" => "Дашборд",
            "Monitor" => "Мониторинг",
            "VendingMachines" => "Вендинговые автоматы",
            "Companies" => "Компании",
            "Admin" => "Администрирование",
            _ => pageName
        };
    }

    [RelayCommand]
    private void Logout()
    {
        LogoutRequested?.Invoke();
    }
}
```

### Шаг 9.3: VendingMachinesViewModel

#### `ViewModels/VendingMachinesViewModel.cs`
```csharp
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VendingMachineDesktop.Models;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class VendingMachinesViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private List<VendingMachine> _allMachines = new();

    [ObservableProperty]
    private ObservableCollection<VendingMachine> _vendingMachines = new();

    [ObservableProperty]
    private VendingMachine? _selectedMachine;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalPages = 1;

    [ObservableProperty]
    private int _pageSize = 10;

    public VendingMachinesViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsLoading = true;

        try
        {
            _allMachines = await _apiService.GetVendingMachinesAsync();
            ApplyFiltersAndPagination();
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        CurrentPage = 1;
        ApplyFiltersAndPagination();
    }

    private void ApplyFiltersAndPagination()
    {
        var filtered = _allMachines.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.ToLower();
            filtered = filtered.Where(m =>
                m.Name.ToLower().Contains(search) ||
                m.SerialNumber.ToLower().Contains(search) ||
                m.Location.ToLower().Contains(search));
        }

        var filteredList = filtered.ToList();
        TotalPages = (int)Math.Ceiling(filteredList.Count / (double)PageSize);

        if (CurrentPage > TotalPages) CurrentPage = Math.Max(1, TotalPages);

        var paged = filteredList
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        VendingMachines = new ObservableCollection<VendingMachine>(paged);
    }

    [RelayCommand]
    private void NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            ApplyFiltersAndPagination();
        }
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            ApplyFiltersAndPagination();
        }
    }

    [RelayCommand]
    private async Task DeleteMachineAsync(VendingMachine? machine)
    {
        if (machine == null) return;

        var result = await _apiService.DeleteVendingMachineAsync(machine.Id);
        if (result)
        {
            await LoadDataAsync();
        }
    }
}
```

---

## 10. Создание Views (XAML)

### Шаг 10.1: LoginWindow

#### `Views/LoginWindow.xaml`
```xml
<Window x:Class="VendingMachineDesktop.Views.LoginWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:materialDesign="http://materialdesigninxaml.net/winfx/xaml/themes"
        Title="Вход в систему"
        Width="400" Height="500"
        WindowStartupLocation="CenterScreen"
        ResizeMode="NoResize"
        Background="{DynamicResource MaterialDesignPaper}">

    <Grid>
        <StackPanel VerticalAlignment="Center" Margin="40">

            <!-- Логотип -->
            <Image Source="/Resources/Logo.png"
                   Width="100" Height="100"
                   Margin="0,0,0,30"/>

            <TextBlock Text="Вход в систему"
                       Style="{StaticResource MaterialDesignHeadline5TextBlock}"
                       HorizontalAlignment="Center"
                       Margin="0,0,0,30"/>

            <!-- Email -->
            <TextBox materialDesign:HintAssist.Hint="Email"
                     Style="{StaticResource MaterialDesignOutlinedTextBox}"
                     Text="{Binding Email, UpdateSourceTrigger=PropertyChanged}"
                     Margin="0,0,0,15"/>

            <!-- Пароль -->
            <PasswordBox materialDesign:HintAssist.Hint="Пароль"
                         Style="{StaticResource MaterialDesignOutlinedPasswordBox}"
                         x:Name="PasswordBox"
                         Margin="0,0,0,15"/>

            <!-- Ошибка -->
            <TextBlock Text="{Binding ErrorMessage}"
                       Foreground="Red"
                       Visibility="{Binding HasError, Converter={StaticResource BoolToVisibilityConverter}}"
                       TextWrapping="Wrap"
                       Margin="0,0,0,15"/>

            <!-- Кнопка входа -->
            <Button Content="ВОЙТИ"
                    Style="{StaticResource MaterialDesignRaisedButton}"
                    Command="{Binding LoginCommand}"
                    IsEnabled="{Binding IsLoading, Converter={StaticResource InverseBooleanConverter}}"
                    Height="45"/>

            <!-- Индикатор загрузки -->
            <ProgressBar IsIndeterminate="True"
                         Visibility="{Binding IsLoading, Converter={StaticResource BoolToVisibilityConverter}}"
                         Margin="0,15,0,0"/>

        </StackPanel>
    </Grid>
</Window>
```

#### `Views/LoginWindow.xaml.cs`
```csharp
using System.Windows;
using System.Windows.Controls;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views;

public partial class LoginWindow : Window
{
    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        viewModel.LoginSuccessful += OnLoginSuccessful;

        // Привязка пароля (PasswordBox не поддерживает Binding напрямую)
        PasswordBox.PasswordChanged += (s, e) =>
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        };
    }

    private void OnLoginSuccessful()
    {
        var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
        Close();
    }
}
```

### Шаг 10.2: MainWindow

#### `Views/MainWindow.xaml`
```xml
<Window x:Class="VendingMachineDesktop.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:materialDesign="http://materialdesigninxaml.net/winfx/xaml/themes"
        Title="VendingMachine Desktop"
        Width="1400" Height="900"
        WindowStartupLocation="CenterScreen"
        Background="{DynamicResource MaterialDesignPaper}">

    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="250"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>

        <!-- Боковое меню -->
        <Border Grid.Column="0"
                Background="{DynamicResource PrimaryHueMidBrush}">
            <DockPanel>

                <!-- Логотип -->
                <StackPanel DockPanel.Dock="Top" Margin="20">
                    <Image Source="/Resources/Logo.png"
                           Width="60" Height="60"/>
                    <TextBlock Text="VendingMachine"
                               Foreground="White"
                               FontSize="18"
                               FontWeight="Bold"
                               HorizontalAlignment="Center"
                               Margin="0,10,0,0"/>
                </StackPanel>

                <!-- Меню навигации -->
                <ListBox SelectedIndex="{Binding SelectedMenuIndex}"
                         Background="Transparent"
                         Foreground="White">

                    <ListBoxItem>
                        <StackPanel Orientation="Horizontal">
                            <materialDesign:PackIcon Kind="ViewDashboard" Margin="0,0,15,0"/>
                            <TextBlock Text="Дашборд"/>
                        </StackPanel>
                    </ListBoxItem>

                    <ListBoxItem>
                        <StackPanel Orientation="Horizontal">
                            <materialDesign:PackIcon Kind="Monitor" Margin="0,0,15,0"/>
                            <TextBlock Text="Мониторинг"/>
                        </StackPanel>
                    </ListBoxItem>

                    <ListBoxItem>
                        <StackPanel Orientation="Horizontal">
                            <materialDesign:PackIcon Kind="CoffeeMaker" Margin="0,0,15,0"/>
                            <TextBlock Text="Автоматы"/>
                        </StackPanel>
                    </ListBoxItem>

                    <ListBoxItem>
                        <StackPanel Orientation="Horizontal">
                            <materialDesign:PackIcon Kind="Domain" Margin="0,0,15,0"/>
                            <TextBlock Text="Компании"/>
                        </StackPanel>
                    </ListBoxItem>

                </ListBox>

                <!-- Кнопка выхода -->
                <Button DockPanel.Dock="Bottom"
                        Content="ВЫХОД"
                        Command="{Binding LogoutCommand}"
                        Style="{StaticResource MaterialDesignOutlinedButton}"
                        Foreground="White"
                        BorderBrush="White"
                        Margin="20"/>

            </DockPanel>
        </Border>

        <!-- Основной контент -->
        <Grid Grid.Column="1">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
            </Grid.RowDefinitions>

            <!-- Заголовок страницы -->
            <Border Grid.Row="0"
                    Background="White"
                    Padding="20">
                <TextBlock Text="{Binding CurrentPageTitle}"
                           Style="{StaticResource MaterialDesignHeadline5TextBlock}"/>
            </Border>

            <!-- Контент страницы -->
            <Frame Grid.Row="1"
                   x:Name="MainFrame"
                   NavigationUIVisibility="Hidden"/>
        </Grid>
    </Grid>
</Window>
```

---

## 11. Настройка Dependency Injection

### `App.xaml.cs`
```csharp
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using VendingMachineDesktop.Services;
using VendingMachineDesktop.ViewModels;
using VendingMachineDesktop.Views;

namespace VendingMachineDesktop;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        // Показываем окно входа
        var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Сервисы (Singleton - один экземпляр на всё приложение)
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IApiService, ApiService>();

        // ViewModels (Transient - новый экземпляр каждый раз)
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<VendingMachinesViewModel>();
        services.AddTransient<CompaniesViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<MonitorViewModel>();

        // Windows
        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindow>();
    }
}
```

---

## 12. Настройка Material Design

### `App.xaml`
```xml
<Application x:Class="VendingMachineDesktop.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:materialDesign="http://materialdesigninxaml.net/winfx/xaml/themes"
             xmlns:converters="clr-namespace:VendingMachineDesktop.Utilities.Converters">

    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>

                <!-- Material Design -->
                <materialDesign:BundledTheme BaseTheme="Light"
                                             PrimaryColor="Blue"
                                             SecondaryColor="Amber"/>
                <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesign2.Defaults.xaml"/>

            </ResourceDictionary.MergedDictionaries>

            <!-- Конвертеры -->
            <converters:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter"/>
            <converters:InverseBooleanConverter x:Key="InverseBooleanConverter"/>

        </ResourceDictionary>
    </Application.Resources>

</Application>
```

### Конвертеры

#### `Utilities/Converters/BoolToVisibilityConverter.cs`
```csharp
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VendingMachineDesktop.Utilities.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility visibility && visibility == Visibility.Visible;
    }
}
```

#### `Utilities/Converters/InverseBooleanConverter.cs`
```csharp
using System.Globalization;
using System.Windows.Data;

namespace VendingMachineDesktop.Utilities.Converters;

public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }
}
```

---

## 13. Запуск приложения

### Шаг 13.1: Предварительные требования

1. **Убедитесь, что API сервер запущен**:
```powershell
cd VendingMachineAPI
dotnet run
# API должен быть доступен на http://localhost:5000
```

2. **Проверьте подключение**:
```powershell
curl http://localhost:5000/api/health
# Или откройте в браузере
```

### Шаг 13.2: Запуск десктоп-приложения

```powershell
# Перейдите в папку проекта
cd VendingMachineDesktop

# Восстановите зависимости
dotnet restore

# Соберите проект
dotnet build

# Запустите приложение
dotnet run
```

### Шаг 13.3: Альтернативный запуск через Visual Studio

1. Откройте `VendingMachineDesktop.sln` в Visual Studio
2. Нажмите `F5` или кнопку "Start"
3. Или `Ctrl+F5` для запуска без отладки

---

## 14. Полезные команды

### Сборка и запуск

```powershell
# Сборка в Debug режиме
dotnet build

# Сборка в Release режиме
dotnet build -c Release

# Запуск
dotnet run

# Запуск в Release режиме
dotnet run -c Release
```

### Публикация

```powershell
# Публикация как самодостаточное приложение (включает .NET Runtime)
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish

# Публикация без рантайма (требует установленный .NET на целевой машине)
dotnet publish -c Release -r win-x64 --self-contained false -o ./publish
```

### Управление пакетами

```powershell
# Список установленных пакетов
dotnet list package

# Обновление пакета
dotnet add package PackageName --version X.X.X

# Удаление пакета
dotnet remove package PackageName

# Восстановление пакетов
dotnet restore
```

### Очистка

```powershell
# Очистка bin и obj папок
dotnet clean

# Полная очистка (удаление bin/obj вручную)
Remove-Item -Recurse -Force bin, obj
```

---

## Приложение A: Полный список API эндпоинтов

| Метод | Эндпоинт | Описание |
|-------|----------|----------|
| POST | `/api/auth/login` | Авторизация |
| GET | `/api/vendingmachines` | Список автоматов |
| GET | `/api/vendingmachines/{id}` | Автомат по ID |
| POST | `/api/vendingmachines` | Создать автомат |
| PUT | `/api/vendingmachines/{id}` | Обновить автомат |
| DELETE | `/api/vendingmachines/{id}` | Удалить автомат |
| GET | `/api/companies` | Список компаний |
| GET | `/api/companies/{id}` | Компания по ID |
| POST | `/api/companies` | Создать компанию |
| PUT | `/api/companies/{id}` | Обновить компанию |
| DELETE | `/api/companies/{id}` | Удалить компанию |
| GET | `/api/dashboard` | Данные дашборда |
| GET | `/api/monitor` | Данные мониторинга |

---

## Приложение B: Полезные ресурсы

### Документация

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [WPF Documentation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [Material Design in XAML](http://materialdesigninxaml.net/)
- [MahApps.Metro](https://mahapps.com/)
- [LiveCharts2](https://lvcharts.com/docs/WPF/2.0.0-rc2/gallery)

### Обучение

- [WPF Tutorial](https://www.wpftutorial.net/)
- [MVVM Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)

---

## Заключение

Данное руководство охватывает все необходимые шаги для создания WPF-приложения управления вендинговыми автоматами. Приложение использует:

- **MVVM архитектуру** для разделения логики и представления
- **Material Design** для современного UI
- **Dependency Injection** для управления зависимостями
- **REST API** для взаимодействия с backend
- **JWT авторизацию** для безопасности

При возникновении вопросов обращайтесь к официальной документации или создавайте Issue в репозитории проекта.
