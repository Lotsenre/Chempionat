# 🚀 Vending Machine API

ASP.NET Core 8.0 Web API для системы управления торговыми автоматами.

---

## ✅ Что уже реализовано

### 📦 Структура проекта
```
VendingMachineAPI/
├── Controllers/          # API контроллеры
│   ├── AuthController.cs           (Login)
│   ├── VendingMachinesController.cs (CRUD)
│   └── DashboardController.cs      (Статистика)
├── Models/
│   ├── Entities/        # 11 Entity моделей (User, VendingMachine, Product и др.)
│   └── DTOs/            # Data Transfer Objects
├── Data/
│   └── ApplicationDbContext.cs
├── Utilities/
│   ├── JwtTokenGenerator.cs
│   ├── JwtSettings.cs
│   └── PasswordHasher.cs
├── Program.cs           # Конфигурация приложения
├── appsettings.json     # Настройки (БД, JWT, Serilog)
└── VendingMachineAPI.csproj
```

### 🎯 Реализованные endpoints

#### Authentication
- `POST /api/auth/login` - Авторизация с JWT

#### Vending Machines
- `GET /api/vendingmachines` - Список автоматов (пагинация, поиск)
- `GET /api/vendingmachines/{id}` - Получить автомат по ID
- `POST /api/vendingmachines` - Создать автомат
- `PUT /api/vendingmachines/{id}` - Обновить автомат
- `DELETE /api/vendingmachines/{id}` - Удалить автомат

#### Dashboard
- `GET /api/dashboard` - Общая статистика
- `GET /api/dashboard/network-efficiency` - Эффективность сети
- `GET /api/dashboard/sales-dynamics` - Динамика продаж

---

## 🛠️ Установка и запуск

### 1. Проверка .NET SDK

```bash
dotnet --version
# Должно быть 8.0 или выше
```

### 2. Восстановление пакетов

```bash
cd VendingMachineAPI
dotnet restore
```

### 3. Проверка настроек БД

Откройте `appsettings.json` и убедитесь, что строка подключения верна:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=13;Username=postgres;Password=root"
  }
}
```

### 4. Запуск приложения

```bash
dotnet run
```

API запустится на:
- HTTPS: https://localhost:7000
- HTTP: http://localhost:5000

**Swagger UI** доступен по адресу: https://localhost:7000

---

## 📖 Как использовать API

### 1. Авторизация

```bash
curl -X POST https://localhost:7000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "leonid_53@example.com",
    "password": "test123"
  }'
```

**Ответ:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "01f0b7d3-9b64-49d0-b63e-9190d736d091",
  "email": "leonid_53@example.com",
  "fullName": "Тимофеева Наталья Николаевна",
  "role": "Персонал"
}
```

**ВАЖНО:** Сохраните `token` для дальнейших запросов!

### 2. Получение списка автоматов

```bash
curl -X GET "https://localhost:7000/api/vendingmachines?page=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### 3. Получение статистики Dashboard

```bash
curl -X GET https://localhost:7000/api/dashboard \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

---

## 🔧 Тестирование через Swagger

1. Откройте https://localhost:7000
2. Нажмите **Authorize** (зеленая кнопка справа)
3. Выполните `/api/auth/login` для получения токена
4. Скопируйте токен из ответа
5. Нажмите **Authorize** снова и вставьте: `Bearer YOUR_TOKEN`
6. Теперь можете тестировать все endpoints!

---

## 📊 Примеры запросов

### Создание торгового автомата

```json
POST /api/vendingmachines
Authorization: Bearer YOUR_TOKEN

{
  "serialNumber": "VM-2024-001",
  "name": "Кофейный автомат #1",
  "model": "CoffeeMaster 3000",
  "location": "Офис, 1 этаж",
  "installDate": "2024-01-15T00:00:00Z"
}
```

### Обновление автомата

```json
PUT /api/vendingmachines/{id}
Authorization: Bearer YOUR_TOKEN

{
  "name": "Кофейный автомат #1 (обновлен)",
  "status": "Maintenance",
  "notes": "Требуется обслуживание"
}
```

---

## 🔐 Безопасность

- **JWT Authentication** - срок действия токена: 24 часа
- **BCrypt** - хеширование паролей
- **HTTPS** - обязательно в production
- **CORS** - настроено AllowAll (измените для production!)

---

## 📝 Логирование

Логи сохраняются в папке `Logs/`:
- Формат: `log-YYYYMMDD.txt`
- Ротация: каждый день

Просмотр логов:
```bash
cat Logs/log-20240115.txt
```

---

## 🚧 Что нужно добавить (опционально)

### Дополнительные Controllers:
- `CompaniesController` - управление компаниями
- `MonitorController` - мониторинг состояния автоматов
- `ContractsController` - работа с договорами

### Дополнительные фичи:
- FluentValidation (валидация входных данных)
- AutoMapper (маппинг Entity <-> DTO)
- Middleware для обработки ошибок
- Unit тесты

---

## ⚙️ Настройка JWT

Для production измените `SecretKey` в `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "ВАША_СЕКРЕТНАЯ_СТРОКА_МИНИМУМ_32_СИМВОЛА",
    "Issuer": "VendingMachineAPI",
    "Audience": "VendingMachineClient",
    "ExpirationHours": 24
  }
}
```

---

## 🐛 Решение проблем

### Ошибка подключения к БД
```
Npgsql.PostgresException: password authentication failed
```
**Решение:** Проверьте параметры в `appsettings.json`

### Ошибка SSL
```
The SSL connection could not be established
```
**Решение:** Добавьте в строку подключения: `;SSL Mode=Disable`

### Swagger не открывается
**Решение:** Убедитесь, что запустили в Development режиме:
```bash
dotnet run --environment Development
```

---

## 📞 API Reference

| Endpoint | Method | Auth | Описание |
|----------|--------|------|----------|
| `/api/auth/login` | POST | ❌ | Авторизация |
| `/api/vendingmachines` | GET | ✅ | Список автоматов |
| `/api/vendingmachines/{id}` | GET | ✅ | Автомат по ID |
| `/api/vendingmachines` | POST | ✅ | Создать автомат |
| `/api/vendingmachines/{id}` | PUT | ✅ | Обновить автомат |
| `/api/vendingmachines/{id}` | DELETE | ✅ | Удалить автомат |
| `/api/dashboard` | GET | ✅ | Общая статистика |
| `/api/dashboard/network-efficiency` | GET | ✅ | Эффективность сети |
| `/api/dashboard/sales-dynamics` | GET | ✅ | Динамика продаж |

---

**Готово к использованию! 🎉**

Для расширения функциональности см. `TECHNICAL_GUIDE.md`
