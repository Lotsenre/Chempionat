# Vending Machine Desktop Application (WPF)

## Что реализовано

### ✅ Базовая структура проекта
- **MVVM архитектура** с использованием CommunityToolkit.Mvvm
- **Dependency Injection** через Microsoft.Extensions.DependencyInjection
- **Material Design** и **MahApps.Metro** для современного UI
- **LiveCharts** для графиков (готово к использованию)

### ✅ Сервисы
- **ApiService** - работа с REST API на localhost:5000
- **AuthService** - управление JWT авторизацией
- **XAML Converters** - конвертеры для UI (BoolToVisibility, StatusToColor, и т.д.)

### ✅ Models
- User, VendingMachine, Company, Dashboard
- DTOs для работы с API (LoginRequest, LoginResponse)

### ✅ ViewModels (базовые)
- **LoginViewModel** - авторизация с валидацией
- **MainViewModel** - главное окно с навигацией
- **DashboardViewModel** - дашборд (подключен к API)
- **VendingMachinesViewModel** - управление ТА
- **CompaniesViewModel** - управление компаниями

### ✅ Views
- **LoginWindow** - окно входа с Material Design
- **MainWindow** - главное окно с боковым меню (HamburgerMenu style)

## Запуск приложения

### 1. Запустите API сервер
```bash
cd VendingMachineAPI
dotnet run
```
API должен быть доступен на `http://localhost:5000`

### 2. Запустите Desktop приложение
```bash
cd VendingMachineDesktop
dotnet run
```

### 3. Войдите в систему
Используйте email и пароль пользователя из вашей базы данных PostgreSQL.

## Что нужно доработать

### 📋 Dashboard (Главная страница)
Необходимо создать `Views/Pages/DashboardPage.xaml` со следующими блоками:

1. **Блок "Эффективность сети"** - CircularProgressBar с процентом работающих автоматов
2. **Блок "Состояние сети"** - PieChart с соотношением статусов (работает/не работает/на обслуживании)
3. **Блок "Сводка"** - данные по продажам, инкассации, обслуживанию
4. **Блок "Динамика продаж"** - график LiveCharts за 10 дней с переключателем (по сумме/по количеству)
5. **Блок "Новости"** - список новостей франчайзера

### 📋 Администрирование - Торговые автоматы
Создать `Views/Pages/VendingMachinesPage.xaml`:

- **DataGrid** с колонками: ID, Название, Модель, Компания, Модем, Адрес/Место, В работе с
- **Переключение режимов** отображения: таблица/плитка
- **Фильтр** по названию автомата
- **Пагинация** (кнопки < 1 2 3 >, выбор количества строк 10/25/50)
- **Кнопки действий**: Добавить, Редактировать, Удалить, Отвязать модем, Экспорт CSV
- **Выделение цветом** нечетных строк таблицы

Создать `Views/Dialogs/AddEditVendingMachineDialog.xaml`:
- Модальное окно для добавления/редактирования ТА
- Все поля согласно макету из PDF (Название ТА, Производитель, Модель, Адрес, Место, и т.д.)

### 📋 Администрирование - Компании
Создать `Views/Pages/CompaniesPage.xaml`:

- Аналогично VendingMachinesPage
- Колонки: Название, Вышестоящая, Адрес, Контакты, В работе с, Действия
- Фильтр, пагинация, экспорт

Создать `Views/Dialogs/AddEditCompanyDialog.xaml`

### 📋 Монитор ТА
Создать `Views/Pages/MonitorPage.xaml`:

- **Фильтры**:
  - По общему состоянию (Зеленый/Красный/Синий)
  - По типу подключения
  - По дополнительным статусам
- **Таблица** с колонками: №, ТП, Связь, Загрузка, Денежные средства, События, Оборудование, Информация, Доп.
- **Сортировка** по состоянию ТА
- **Итого автоматов** и **Денег в автоматах** (с учетом фильтров)

## Необходимые API endpoints

Убедитесь, что в вашем API сервере реализованы следующие endpoints:

### Dashboard
- `GET /api/dashboard` - все данные для главной страницы
- `GET /api/dashboard/sales-dynamics?days=10&filterBy=amount` - график продаж

### Vending Machines
- `GET /api/vendingmachines` - список всех ТА
- `GET /api/vendingmachines/{id}` - детали ТА
- `POST /api/vendingmachines` - добавить ТА
- `PUT /api/vendingmachines/{id}` - обновить ТА
- `DELETE /api/vendingmachines/{id}` - удалить ТА
- `GET /api/vendingmachines/export/csv` - экспорт в CSV

### Companies
- `GET /api/companies` - список всех компаний
- `GET /api/companies/{id}` - детали компании
- `POST /api/companies` - добавить компанию
- `PUT /api/companies/{id}` - обновить компанию
- `DELETE /api/companies/{id}` - удалить компанию
- `GET /api/companies/export/csv` - экспорт в CSV

### Monitor
- `GET /api/monitor` - список ТА с фильтрами
- `GET /api/monitor/{id}/status` - детальный статус ТА
- `GET /api/monitor/{id}/events` - события ТА

## Примеры использования

### Добавление новой страницы

1. Создайте Page в `Views/Pages/`:
```xaml
<Page x:Class="VendingMachineDesktop.Views.Pages.DashboardPage"
      xmlns:materialDesign="http://materialdesigninxaml.net/winfx/xaml/themes">
    <!-- Ваш контент -->
</Page>
```

2. Обновите `MainWindow.xaml.cs` для навигации:
```csharp
case 0:
    ContentFrame.Navigate(new DashboardPage());
    break;
```

### Использование LiveCharts

```csharp
// В ViewModel
public ISeries[] Series { get; set; } = new ISeries[]
{
    new LineSeries<double>
    {
        Values = new double[] { 200, 558, 458, 249, 457, 339, 587, 421, 389, 512 }
    }
};
```

```xaml
<!-- В XAML -->
<lvc:CartesianChart Series="{Binding Series}"/>
```

### Экспорт в CSV

```csharp
using CsvHelper;

// В ViewModel
[RelayCommand]
private void ExportToCsv()
{
    var saveDialog = new SaveFileDialog
    {
        Filter = "CSV files (*.csv)|*.csv",
        FileName = "vending_machines.csv"
    };

    if (saveDialog.ShowDialog() == true)
    {
        using var writer = new StreamWriter(saveDialog.FileName);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(VendingMachines);
    }
}
```

## Структура проекта

```
VendingMachineDesktop/
├── Models/
│   ├── DTOs/
│   │   ├── LoginRequest.cs
│   │   └── LoginResponse.cs
│   ├── User.cs
│   ├── VendingMachine.cs
│   ├── Company.cs
│   └── Dashboard.cs
├── ViewModels/
│   ├── LoginViewModel.cs
│   ├── MainViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── VendingMachinesViewModel.cs
│   └── CompaniesViewModel.cs
├── Views/
│   ├── LoginWindow.xaml
│   ├── MainWindow.xaml
│   ├── Pages/ (создайте здесь страницы)
│   └── Dialogs/ (создайте здесь диалоги)
├── Services/
│   ├── IApiService.cs
│   ├── ApiService.cs
│   ├── IAuthService.cs
│   └── AuthService.cs
├── Utilities/
│   └── Converters/
│       ├── BoolToVisibilityConverter.cs
│       ├── InverseBooleanConverter.cs
│       ├── StatusToColorConverter.cs
│       └── BoolToWidthConverter.cs
├── App.xaml
└── App.xaml.cs
```

## Технологии

- **.NET 8.0** - целевая платформа
- **WPF** - Windows Presentation Foundation
- **Material Design in XAML** - современный дизайн
- **MahApps.Metro** - дополнительные контролы
- **LiveCharts** - графики и диаграммы
- **CommunityToolkit.Mvvm** - MVVM фреймворк
- **System.Net.Http.Json** - работа с JSON API
- **CsvHelper** - экспорт в CSV

## Следующие шаги

1. Запустите API сервер
2. Запустите Desktop приложение
3. Войдите в систему
4. Создайте недостающие страницы согласно PDF макетам
5. Подключите LiveCharts для графиков
6. Реализуйте экспорт в CSV
7. Добавьте модальные диалоги для CRUD операций

Удачи в разработке! 🚀
