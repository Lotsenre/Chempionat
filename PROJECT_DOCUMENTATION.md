# Документация проекта "Система управления торговыми автоматами"

## Оглавление

1. [Обзор проекта](#1-обзор-проекта)
2. [Архитектура системы](#2-архитектура-системы)
3. [Технологический стек](#3-технологический-стек)
4. [Структура файлов](#4-структура-файлов)
5. [База данных](#5-база-данных)
6. [API (VendingMachineAPI)](#6-api-vendingmachineapi)
7. [Desktop приложение (VendingMachineDesktop)](#7-desktop-приложение-vendingmachinedesktop)
8. [Web приложение (VendingMachineWeb)](#8-web-приложение-vendingmachineweb)
9. [Python скрипты импорта](#9-python-скрипты-импорта)
10. [Команды запуска](#10-команды-запуска)
11. [Руководство по развёртыванию](#11-руководство-по-развёртыванию)

---

## 1. Обзор проекта

### Назначение
Система управления сетью торговых автоматов (вендинговых машин) для продажи кофейных напитков. Позволяет отслеживать состояние автоматов, продажи, обслуживание и управлять персоналом.

### Основные функции
- **Мониторинг автоматов** - отслеживание статуса, связи, уровня ингредиентов
- **Управление продажами** - статистика, динамика, отчёты
- **Управление персоналом** - операторы, инженеры, менеджеры
- **Техническое обслуживание** - планирование, история ремонтов
- **Аналитика** - эффективность сети, графики продаж
- **Договоры аренды** - оформление франчайзи

### Компоненты системы
| Компонент | Технология | Назначение |
|-----------|------------|------------|
| VendingMachineAPI | ASP.NET Core 8 | REST API сервер |
| VendingMachineDesktop | WPF (.NET 8) | Desktop клиент для Windows |
| VendingMachineWeb | Laravel 12 (PHP) | Web клиент |
| Python скрипты | Python 3 | Импорт данных |

---

## 2. Архитектура системы

```
┌─────────────────────────────────────────────────────────────────┐
│                        КЛИЕНТЫ                                   │
├─────────────────────┬───────────────────────────────────────────┤
│  Desktop (WPF)      │           Web (Laravel)                   │
│  - Windows App      │           - Браузер                       │
│  - MVVM Pattern     │           - Blade + Alpine.js             │
│  - LiveCharts       │           - Tailwind CSS                  │
└─────────┬───────────┴─────────────────────┬─────────────────────┘
          │                                 │
          │         HTTP/REST + JWT         │
          │                                 │
          ▼                                 ▼
┌─────────────────────────────────────────────────────────────────┐
│                    VendingMachineAPI                             │
│                    (ASP.NET Core 8)                              │
├─────────────────────────────────────────────────────────────────┤
│  Controllers:                                                    │
│  - AuthController (login, register)                              │
│  - DashboardController (статистика)                              │
│  - VendingMachinesController (CRUD)                              │
│  - CompaniesController (CRUD)                                    │
├─────────────────────────────────────────────────────────────────┤
│  Middleware: JWT Authentication, CORS, Serilog                   │
└─────────────────────────────┬───────────────────────────────────┘
                              │
                              │ Entity Framework Core
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    PostgreSQL Database                           │
│                    База данных: "13"                             │
├─────────────────────────────────────────────────────────────────┤
│  Таблицы: users, companies, vending_machines, products,          │
│           sales, maintenance, events, news, machine_status,      │
│           contracts, available_machines                          │
└─────────────────────────────────────────────────────────────────┘
```

---

## 3. Технологический стек

### 3.1 Backend (VendingMachineAPI)

| Технология | Версия | Назначение |
|------------|--------|------------|
| **.NET** | 8.0 | Платформа |
| **ASP.NET Core** | 8.0 | Web framework |
| **Entity Framework Core** | 8.0 | ORM |
| **Npgsql** | 8.0.0 | PostgreSQL провайдер |
| **JWT Bearer** | 8.0.0 | Аутентификация |
| **BCrypt.Net-Next** | 4.0.3 | Хеширование паролей |
| **Serilog** | 8.0.0 | Логирование |
| **Swashbuckle** | 6.5.0 | Swagger документация |
| **AutoMapper** | 12.0.1 | Маппинг объектов |
| **FluentValidation** | 11.3.0 | Валидация |

### 3.2 Desktop (VendingMachineDesktop)

| Технология | Версия | Назначение |
|------------|--------|------------|
| **.NET** | 8.0-Windows | Платформа |
| **WPF** | - | UI Framework |
| **CommunityToolkit.Mvvm** | 8.4.0 | MVVM framework |
| **LiveChartsCore.SkiaSharp** | 2.0.0-rc2.2 | Графики и диаграммы |
| **MaterialDesignThemes** | 5.3.0 | Material Design UI |
| **CsvHelper** | 33.1.0 | CSV экспорт |
| **Newtonsoft.Json** | 13.0.4 | JSON сериализация |
| **Microsoft.Extensions.DI** | 10.0.1 | Dependency Injection |

### 3.3 Web (VendingMachineWeb)

| Технология | Версия | Назначение |
|------------|--------|------------|
| **PHP** | 8.2+ | Язык |
| **Laravel** | 12.0 | Framework |
| **SQLite** | - | Локальная БД (кэш) |
| **Tailwind CSS** | 4.0 | Стилизация |
| **Alpine.js** | 3.x | Интерактивность |
| **Vite** | 7.0 | Сборка assets |
| **Axios** | 1.11 | HTTP клиент |

### 3.4 Python скрипты

| Библиотека | Назначение |
|------------|------------|
| **psycopg2** | PostgreSQL драйвер |
| **json** | Работа с JSON |
| **pathlib** | Работа с путями |

### 3.5 База данных

| Технология | Версия | Назначение |
|------------|--------|------------|
| **PostgreSQL** | 14+ | Основная БД |
| **SQLite** | - | Локальная БД для Laravel |

---

## 4. Структура файлов

```
Chempionat/
│
├── VendingMachineAPI/              # ASP.NET Core API
│   ├── Controllers/                # Контроллеры API
│   │   ├── AuthController.cs       # Аутентификация (login, register)
│   │   ├── DashboardController.cs  # Статистика и аналитика
│   │   ├── VendingMachinesController.cs  # CRUD автоматов
│   │   └── CompaniesController.cs  # CRUD компаний
│   ├── Models/
│   │   ├── Entities/               # Сущности БД
│   │   │   ├── User.cs             # Пользователь
│   │   │   ├── Company.cs          # Компания/франчайзи
│   │   │   ├── VendingMachine.cs   # Торговый автомат
│   │   │   ├── Product.cs          # Товар
│   │   │   ├── Sale.cs             # Продажа
│   │   │   ├── Maintenance.cs      # Обслуживание
│   │   │   ├── Event.cs            # Событие
│   │   │   ├── News.cs             # Новость
│   │   │   ├── MachineStatus.cs    # Статус автомата
│   │   │   ├── Contract.cs         # Договор
│   │   │   └── AvailableMachine.cs # Доступный автомат
│   │   └── DTOs/                   # Data Transfer Objects
│   │       ├── LoginRequest.cs
│   │       ├── LoginResponse.cs
│   │       ├── RegisterRequest.cs
│   │       └── VendingMachineDto.cs
│   ├── Data/
│   │   └── ApplicationDbContext.cs # EF Core контекст
│   ├── Utilities/
│   │   ├── JwtSettings.cs          # Настройки JWT
│   │   ├── JwtTokenGenerator.cs    # Генератор токенов
│   │   └── PasswordHasher.cs       # Хеширование паролей
│   ├── Program.cs                  # Точка входа, конфигурация
│   ├── appsettings.json            # Настройки приложения
│   └── VendingMachineAPI.csproj    # Файл проекта
│
├── VendingMachineDesktop/          # WPF Desktop приложение
│   ├── Views/                      # XAML страницы
│   │   ├── LoginWindow.xaml        # Окно входа
│   │   ├── MainWindow.xaml         # Главное окно
│   │   ├── DashboardPage.xaml      # Главная страница
│   │   ├── MonitorPage.xaml        # Мониторинг автоматов
│   │   ├── VendingMachinesPage.xaml # Управление автоматами
│   │   ├── CompaniesPage.xaml      # Управление компаниями
│   │   └── AdminPage.xaml          # Администрирование
│   ├── ViewModels/                 # MVVM ViewModels
│   │   ├── LoginViewModel.cs
│   │   ├── MainViewModel.cs
│   │   ├── DashboardViewModel.cs
│   │   ├── MonitorViewModel.cs
│   │   ├── VendingMachinesViewModel.cs
│   │   └── CompaniesViewModel.cs
│   ├── Models/                     # Модели данных
│   │   ├── VendingMachine.cs
│   │   ├── VendingMachineMonitor.cs
│   │   ├── Company.cs
│   │   └── User.cs
│   ├── Services/                   # Сервисы
│   │   ├── IApiService.cs
│   │   ├── ApiService.cs           # HTTP клиент для API
│   │   ├── IAuthService.cs
│   │   └── AuthService.cs          # Аутентификация
│   ├── Utilities/                  # Конвертеры и утилиты
│   │   ├── BoolToVisibilityConverter.cs
│   │   ├── StatusToColorConverter.cs
│   │   └── BoolToWidthConverter.cs
│   ├── App.xaml                    # Ресурсы приложения
│   ├── App.xaml.cs                 # DI конфигурация
│   └── VendingMachineDesktop.csproj
│
├── VendingMachineWeb/              # Laravel Web приложение
│   ├── app/
│   │   ├── Http/Controllers/
│   │   │   ├── AuthController.php      # Аутентификация
│   │   │   ├── DashboardController.php # Главная страница
│   │   │   ├── VendingMachineController.php # CRUD
│   │   │   └── ContractController.php  # Договоры
│   │   ├── Models/
│   │   │   ├── User.php
│   │   │   ├── VendingMachine.php
│   │   │   └── Modem.php
│   │   └── Services/
│   │       └── VendingApiService.php   # HTTP клиент для API
│   ├── resources/views/
│   │   ├── layouts/app.blade.php       # Основной layout
│   │   ├── dashboard.blade.php         # Главная
│   │   ├── auth/login.blade.php        # Вход
│   │   └── vending-machines/
│   │       └── index.blade.php         # Список автоматов
│   ├── routes/web.php                  # Маршруты
│   ├── database/migrations/            # Миграции БД
│   ├── composer.json                   # PHP зависимости
│   └── .env                            # Переменные окружения
│
├── users/                          # JSON файлы пользователей
│   └── user_*.json                 # 20 файлов с данными
│
├── Общие ресурсы/                  # Графические ресурсы
│   ├── Logo.png                    # Логотип
│   └── Иконки/                     # Font Awesome, Hardware icons
│
├── import_products.py              # Импорт продуктов в БД
├── import_users.py                 # Импорт пользователей в БД
├── products.json                   # 50 товаров (кофейные напитки)
├── sales.csv                       # 100 записей о продажах
├── vending_machines.csv            # 10 торговых автоматов
├── maintenance.csv                 # 100 записей обслуживания
└── PROJECT_DOCUMENTATION.md        # Этот файл
```

---

## 5. База данных

### 5.1 Схема базы данных

#### Таблица `users` - Пользователи
| Поле | Тип | Описание |
|------|-----|----------|
| id | UUID | Первичный ключ |
| full_name | VARCHAR(255) | ФИО |
| email | VARCHAR(255) | Email (уникальный) |
| phone | VARCHAR(50) | Телефон |
| password_hash | VARCHAR(255) | BCrypt хеш пароля |
| role | VARCHAR(50) | Роль (User, Admin, Manager) |
| image | TEXT | Base64 аватарка |
| is_active | BOOLEAN | Активность аккаунта |
| is_manager | BOOLEAN | Флаг менеджера |
| is_engineer | BOOLEAN | Флаг инженера |
| is_operator | BOOLEAN | Флаг оператора |
| company_id | UUID | FK на companies |
| franchisee_code | VARCHAR(50) | Код франчайзи |
| created_at | TIMESTAMP | Дата создания |
| updated_at | TIMESTAMP | Дата обновления |
| last_login_at | TIMESTAMP | Последний вход |

#### Таблица `companies` - Компании
| Поле | Тип | Описание |
|------|-----|----------|
| id | UUID | Первичный ключ |
| name | VARCHAR(255) | Название |
| address | VARCHAR(500) | Адрес |
| contact_info | VARCHAR(500) | Контакты |
| notes | TEXT | Примечания |
| parent_company_id | UUID | FK на companies (иерархия) |
| working_since | TIMESTAMP | Дата начала работы |
| created_at | TIMESTAMP | Дата создания |
| updated_at | TIMESTAMP | Дата обновления |

#### Таблица `vending_machines` - Торговые автоматы
| Поле | Тип | Описание |
|------|-----|----------|
| id | UUID | Первичный ключ |
| serial_number | VARCHAR(50) | Серийный номер (уникальный) |
| name | VARCHAR(255) | Название |
| model | VARCHAR(255) | Модель (Rheavendors, Unicum, Bianchi) |
| status | VARCHAR(50) | Статус (Working, NotWorking, OnMaintenance) |
| location | VARCHAR(500) | Адрес расположения |
| place | VARCHAR(500) | Место (этаж, здание) |
| coordinates | VARCHAR(100) | GPS координаты |
| install_date | TIMESTAMP | Дата установки |
| last_maintenance_date | TIMESTAMP | Последнее ТО |
| working_hours | VARCHAR(50) | Часы работы |
| timezone | VARCHAR(50) | Часовой пояс |
| work_mode | VARCHAR(100) | Режим работы |
| total_income | DECIMAL(18,2) | Общий доход |
| manager | VARCHAR(255) | Менеджер |
| engineer | VARCHAR(255) | Инженер |
| operator | VARCHAR(255) | Оператор |
| technician | VARCHAR(255) | Техник |
| payment_type | VARCHAR(255) | Типы оплаты |
| kit_online_id | VARCHAR(50) | ID модема |
| rfid_service | VARCHAR(50) | RFID сервиса |
| rfid_cash_collection | VARCHAR(50) | RFID инкассации |
| rfid_loading | VARCHAR(50) | RFID загрузки |
| company | VARCHAR(255) | Компания |
| critical_threshold_template | VARCHAR(100) | Шаблон критического уровня |
| notification_template | VARCHAR(100) | Шаблон уведомлений |
| service_priority | VARCHAR(50) | Приоритет обслуживания |
| notes | TEXT | Примечания |
| user_id | UUID | FK на users |
| created_at | TIMESTAMP | Дата создания |
| updated_at | TIMESTAMP | Дата обновления |

#### Таблица `products` - Товары
| Поле | Тип | Описание |
|------|-----|----------|
| id | UUID | Первичный ключ |
| name | VARCHAR(255) | Название товара |
| description | TEXT | Описание |
| price | DECIMAL(10,2) | Цена |
| min_stock | INTEGER | Минимальный запас |
| category | VARCHAR(100) | Категория |
| quantity_available | INTEGER | Количество на складе |
| sales_trend | DECIMAL(5,2) | Тренд продаж |
| vending_machine_id | UUID | FK на vending_machines |
| created_at | TIMESTAMP | Дата создания |
| updated_at | TIMESTAMP | Дата обновления |

#### Таблица `sales` - Продажи
| Поле | Тип | Описание |
|------|-----|----------|
| id | UUID | Первичный ключ |
| vending_machine_id | UUID | FK на vending_machines |
| product_id | UUID | FK на products |
| quantity | INTEGER | Количество |
| total_price | DECIMAL(10,2) | Сумма |
| payment_method | VARCHAR(50) | Способ оплаты |
| timestamp | TIMESTAMP | Время продажи |

#### Таблица `maintenance` - Обслуживание
| Поле | Тип | Описание |
|------|-----|----------|
| id | UUID | Первичный ключ |
| vending_machine_id | UUID | FK на vending_machines |
| date | TIMESTAMP | Дата обслуживания |
| work_description | TEXT | Описание работ |
| issues_found | TEXT | Найденные проблемы |
| technician_id | UUID | ID техника |
| full_name | VARCHAR(255) | ФИО техника |
| status | VARCHAR(50) | Статус (Completed, Pending) |
| created_at | TIMESTAMP | Дата создания |
| updated_at | TIMESTAMP | Дата обновления |

### 5.2 Диаграмма связей

```
users ──────────────────┬──────────────── vending_machines
  │                     │                        │
  │                     │                        ├── products
  │                     │                        │      │
  │                     │                        │      └── sales
  │                     │                        │
  │                     │                        ├── maintenance
  │                     │                        │
  │                     │                        ├── events
  │                     │                        │
  │                     │                        ├── machine_status (1:1)
  │                     │                        │
  │                     │                        └── contracts
  │                     │
  └── news (author)     │
                        │
companies ──────────────┴── (self-reference: parent_company)
    │
    └── contracts
```

---

## 6. API (VendingMachineAPI)

### 6.1 Эндпоинты

#### Аутентификация (`/api/auth`)
| Метод | URL | Описание | Авторизация |
|-------|-----|----------|-------------|
| POST | `/api/auth/register` | Регистрация пользователя | Нет |
| POST | `/api/auth/login` | Вход в систему | Нет |

**Пример запроса login:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Пример ответа:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "fullName": "Иванов Иван",
  "role": "Admin"
}
```

#### Dashboard (`/api/dashboard`)
| Метод | URL | Описание |
|-------|-----|----------|
| GET | `/api/dashboard` | Общая статистика |
| GET | `/api/dashboard/network-efficiency` | Эффективность сети |
| GET | `/api/dashboard/sales-dynamics?days=10` | Динамика продаж |

#### Торговые автоматы (`/api/vending-machines`)
| Метод | URL | Описание |
|-------|-----|----------|
| GET | `/api/vending-machines?page=1&pageSize=10&search=` | Список с пагинацией |
| GET | `/api/vending-machines/{id}` | Получить по ID |
| POST | `/api/vending-machines` | Создать |
| PUT | `/api/vending-machines/{id}` | Обновить |
| DELETE | `/api/vending-machines/{id}` | Удалить |
| POST | `/api/vending-machines/{id}/detach-modem` | Отвязать модем |

#### Компании (`/api/companies`)
| Метод | URL | Описание |
|-------|-----|----------|
| GET | `/api/companies` | Список компаний |
| GET | `/api/companies/{id}` | Получить по ID |
| POST | `/api/companies` | Создать |
| PUT | `/api/companies/{id}` | Обновить |
| DELETE | `/api/companies/{id}` | Удалить |
| POST | `/api/companies/seed` | Заполнить тестовыми данными |

### 6.2 Аутентификация

API использует JWT (JSON Web Token) для аутентификации.

**Заголовок запроса:**
```
Authorization: Bearer <token>
```

**Структура токена (Claims):**
- `sub` - ID пользователя
- `email` - Email
- `role` - Роль
- `full_name` - Полное имя
- `jti` - Уникальный ID токена
- `exp` - Время истечения (24 часа)

---

## 7. Desktop приложение (VendingMachineDesktop)

### 7.1 Архитектура MVVM

```
View (XAML)  ←──binding──→  ViewModel  ←──services──→  Model/API
    │                           │
    └── Commands ───────────────┘
```

### 7.2 Основные экраны

1. **LoginWindow** - Окно входа
   - Email и пароль
   - Валидация
   - Переход на MainWindow

2. **DashboardPage** - Главная страница
   - Gauge эффективности сети (LiveCharts)
   - Pie chart статуса сети
   - Column chart динамики продаж
   - Лента новостей

3. **MonitorPage** - Мониторинг ТА
   - DataGrid с 14+ колонками
   - 30+ фильтров
   - 26 вариантов сортировки
   - Экспорт в CSV

4. **VendingMachinesPage** - Управление ТА
   - Таблица и плитки
   - CRUD операции
   - Отвязка модема
   - Экспорт CSV

5. **CompaniesPage** - Управление компаниями
   - Таблица и плитки
   - CRUD операции
   - Экспорт CSV

### 7.3 Сервисы

**ApiService** - HTTP клиент:
- Base URL: `http://localhost:5000/api/`
- Методы: GetAsync, PostAsync, PutAsync, DeleteAsync
- Автоматическое добавление JWT токена

**AuthService** - Аутентификация:
- LoginAsync - вход с получением токена
- GetToken/SetToken - управление токеном
- GetCurrentUser - текущий пользователь

---

## 8. Web приложение (VendingMachineWeb)

### 8.1 Маршруты

| URL | Контроллер | Описание |
|-----|------------|----------|
| `/login` | AuthController | Страница входа |
| `/register` | AuthController | Регистрация |
| `/logout` | AuthController | Выход |
| `/` | DashboardController | Главная |
| `/vending-machines` | VendingMachineController | Список ТА |
| `/vending-machines/create` | VendingMachineController | Создание |
| `/vending-machines/{id}/edit` | VendingMachineController | Редактирование |
| `/vending-machines/export` | VendingMachineController | CSV экспорт |
| `/contracts` | ContractController | Договоры |

### 8.2 Особенности

- **Интеграция с API** через VendingApiService
- **JWT токен** хранится в session
- **Alpine.js** для интерактивности (модальные окна, фильтры)
- **Canvas** для подписи договоров
- **LocalStorage** для audit log

---

## 9. Python скрипты импорта

### 9.1 import_products.py

**Назначение:** Импорт товаров из `products.json` в БД

**Запуск:**
```bash
python import_products.py
```

**Входные данные:** `products.json` (50 товаров)

### 9.2 import_users.py

**Назначение:** Импорт пользователей из `users/*.json` в БД

**Запуск:**
```bash
python import_users.py
```

**Входные данные:** `users/user_*.json` (20 файлов)

### 9.3 Требования

```bash
pip install psycopg2-binary
```

---

## 10. Команды запуска

### 10.1 База данных PostgreSQL

```bash
# Создание базы данных
createdb -U postgres 13

# Или через psql
psql -U postgres
CREATE DATABASE "13";
```

### 10.2 API (VendingMachineAPI)

```bash
cd VendingMachineAPI

# Восстановление зависимостей
dotnet restore

# Применение миграций (если есть)
dotnet ef database update

# Запуск в режиме разработки
dotnet run

# Или запуск на конкретном порту
dotnet run --urls "http://localhost:5000"
```

**API будет доступен:** `http://localhost:5000`
**Swagger UI:** `http://localhost:5000/swagger`

### 10.3 Desktop (VendingMachineDesktop)

```bash
cd VendingMachineDesktop

# Восстановление зависимостей
dotnet restore

# Сборка
dotnet build

# Запуск
dotnet run
```

### 10.4 Web (VendingMachineWeb)

```bash
cd VendingMachineWeb

# Установка PHP зависимостей
composer install

# Копирование .env файла
cp .env.example .env

# Генерация ключа приложения
php artisan key:generate

# Создание БД SQLite
touch database/database.sqlite

# Миграции
php artisan migrate

# Запуск dev сервера
php artisan serve
```

**Web будет доступен:** `http://localhost:8000`

### 10.5 Импорт данных

```bash
# Импорт продуктов
python import_products.py

# Импорт пользователей
python import_users.py
```

---

## 11. Руководство по развёртыванию

### 11.1 Порядок развёртывания

1. **Установить PostgreSQL** (версия 14+)
2. **Создать базу данных** с именем `13`
3. **Запустить API** (VendingMachineAPI)
4. **Импортировать данные** (Python скрипты)
5. **Запустить клиенты** (Desktop и/или Web)

### 11.2 Настройка подключения к БД

**API (appsettings.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=13;Username=postgres;Password=root"
  }
}
```

**Python скрипты:**
```python
db_params = {
    'dbname': '13',
    'user': 'postgres',
    'password': 'root',
    'host': 'localhost',
    'port': '5432'
}
```

**Laravel (.env):**
```
VENDING_API_URL=http://localhost:5000
```

### 11.3 Тестовые учётные данные

После импорта данных и регистрации пользователя через API:

```
Email: admin@example.com
Password: (установленный при регистрации)
```

### 11.4 Проверка работоспособности

1. Открыть Swagger: `http://localhost:5000/swagger`
2. Зарегистрировать пользователя через POST `/api/auth/register`
3. Войти через POST `/api/auth/login`
4. Проверить GET `/api/dashboard`

---

## Примечания

### Безопасность

- В production необходимо:
  - Изменить JWT SecretKey
  - Использовать переменные окружения для паролей БД
  - Включить HTTPS
  - Настроить CORS для конкретных доменов

### Известные ограничения

- Desktop приложение работает только на Windows
- Web клиент требует запущенный API
- Python скрипты требуют прямого доступа к PostgreSQL

---

*Документация создана: Январь 2026*
*Версия проекта: 1.0*
