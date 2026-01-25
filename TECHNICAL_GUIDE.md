# Техническое руководство: Информационная система для сети вендинговых аппаратов

## ⏱️ ВАЖНО: Каждый этап = 3 часа БЕЗ интернета!

---

## 📋 Содержание

1. [Разработка базы данных (3ч)](#1-разработка-базы-данных)
2. [Разработка API (3ч)](#2-разработка-api)
3. [Desktop приложение WPF (3ч)](#3-desktop-приложение-wpf)
4. [Web приложение Laravel (3ч)](#4-web-приложение-laravel)
5. [Mobile Wireframe (3ч)](#5-mobile-wireframe)
6. [Тестирование](#6-тестирование)
7. [Документация](#7-документация)

---

## 1. Разработка базы данных

### ⏱️ Время: 3 часа | 🎯 Цель: PostgreSQL БД в 3НФ с тестовыми данными

### Что делать

**Час 1: Создание структуры**
1. Создать БД в PostgreSQL
2. Создать 13 таблиц (см. ниже)
3. Установить все связи Foreign Key

**Час 2: Данные и триггеры**
4. Создать индексы
5. Написать триггеры
6. Написать функции

**Час 3: Тестовые данные**
7. Импортировать данные из "Импорт"
8. Сгенерировать тестовые данные
9. Проверить 3НФ

### SQL-скрипты для таблиц

> **ВАЖНО:** Структура таблиц адаптирована для прямого импорта данных через `\copy`.
> Названия колонок соответствуют названиям в файлах данных (snake_case).
> Все ID используют тип UUID для совместимости с импортируемыми данными.

```sql
-- Включаем расширение для UUID
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Таблица 1: Компании
CREATE TABLE companies (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    address VARCHAR(500) NOT NULL,
    contact_info VARCHAR(500),
    notes TEXT,
    parent_company_id UUID NULL,
    working_since DATE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_companies_parent FOREIGN KEY (parent_company_id)
        REFERENCES companies(id) ON DELETE SET NULL
);

CREATE INDEX ix_companies_parent ON companies(parent_company_id);

-- Таблица 2: Пользователи
-- Колонки соответствуют структуре файлов users/*.json
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    full_name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    phone VARCHAR(50),
    password_hash VARCHAR(255) DEFAULT '',
    role VARCHAR(50) NOT NULL,
    image TEXT,
    is_active BOOLEAN DEFAULT TRUE,
    company_id UUID NULL,
    franchisee_code VARCHAR(50) UNIQUE,
    is_manager BOOLEAN DEFAULT FALSE,
    is_engineer BOOLEAN DEFAULT FALSE,
    is_operator BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login_at TIMESTAMP,

    CONSTRAINT fk_users_companies FOREIGN KEY (company_id)
        REFERENCES companies(id) ON DELETE SET NULL
);

CREATE INDEX ix_users_email ON users(email);
CREATE INDEX ix_users_role ON users(role);

-- Таблица 3: Модемы
CREATE TABLE modems (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    modem_number VARCHAR(50) UNIQUE NOT NULL,
    model VARCHAR(100),
    status VARCHAR(50) DEFAULT 'Active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблица 4: Торговые автоматы
-- Колонки соответствуют структуре файла vending_machines.csv
CREATE TABLE vending_machines (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    serial_number VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    model VARCHAR(255) NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'Working',
    location VARCHAR(500) NOT NULL,
    place VARCHAR(500),
    coordinates VARCHAR(100),
    install_date TIMESTAMP NOT NULL,
    last_maintenance_date TIMESTAMP,
    working_hours VARCHAR(50),
    timezone VARCHAR(50),
    total_income DECIMAL(18,2) DEFAULT 0,
    user_id UUID,
    work_mode VARCHAR(100),
    rfid_cash_collection VARCHAR(50),
    rfid_loading VARCHAR(50),
    rfid_service VARCHAR(50),
    kit_online_id VARCHAR(50),
    company VARCHAR(255),
    payment_type VARCHAR(255),
    critical_threshold_template VARCHAR(100),
    service_priority VARCHAR(50),
    manager VARCHAR(255),
    notification_template VARCHAR(100),
    engineer VARCHAR(255),
    operator VARCHAR(255),
    technician VARCHAR(255),
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_vending_machines_users FOREIGN KEY (user_id)
        REFERENCES users(id) ON DELETE SET NULL
);

CREATE INDEX ix_vending_machines_status ON vending_machines(status);
CREATE INDEX ix_vending_machines_user_id ON vending_machines(user_id);

-- Таблица 5: Товары
-- Колонки соответствуют структуре файла products.json
CREATE TABLE products (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    price DECIMAL(10,2) NOT NULL,
    min_stock INT NOT NULL DEFAULT 10,
    category VARCHAR(100),
    vending_machine_id UUID,
    quantity_available INT DEFAULT 0,
    sales_trend DECIMAL(5,2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_products_vending_machines FOREIGN KEY (vending_machine_id)
        REFERENCES vending_machines(id) ON DELETE SET NULL
);

CREATE INDEX ix_products_vending_machine_id ON products(vending_machine_id);

-- Таблица 6: Запасы в автоматах
CREATE TABLE machine_inventory (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    vending_machine_id UUID NOT NULL,
    product_id UUID NOT NULL,
    current_stock INT NOT NULL DEFAULT 0,
    max_capacity INT NOT NULL,
    sales_popularity DECIMAL(5,2),
    last_restocked_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_machine_inventory_vending_machines FOREIGN KEY (vending_machine_id)
        REFERENCES vending_machines(id) ON DELETE CASCADE,
    CONSTRAINT fk_machine_inventory_products FOREIGN KEY (product_id)
        REFERENCES products(id) ON DELETE CASCADE,
    CONSTRAINT uq_machine_inventory UNIQUE (vending_machine_id, product_id)
);

-- Таблица 7: Продажи
-- Колонки соответствуют структуре файла sales.csv
CREATE TABLE sales (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    vending_machine_id UUID,
    product_id UUID NOT NULL,
    quantity INT NOT NULL,
    total_price DECIMAL(10,2) NOT NULL,
    payment_method VARCHAR(50) NOT NULL,
    timestamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_sales_vending_machines FOREIGN KEY (vending_machine_id)
        REFERENCES vending_machines(id) ON DELETE SET NULL,
    CONSTRAINT fk_sales_products FOREIGN KEY (product_id)
        REFERENCES products(id) ON DELETE SET NULL
);

CREATE INDEX ix_sales_timestamp ON sales(timestamp);
CREATE INDEX ix_sales_product_id ON sales(product_id);

-- Таблица 8: Обслуживание
-- Колонки соответствуют структуре файла maintenance.csv
CREATE TABLE maintenance (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    vending_machine_id UUID NOT NULL,
    date TIMESTAMP NOT NULL,
    work_description TEXT NOT NULL,
    issues_found TEXT,
    technician_id UUID,
    full_name VARCHAR(255),
    status VARCHAR(50) DEFAULT 'Completed',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_maintenance_vending_machines FOREIGN KEY (vending_machine_id)
        REFERENCES vending_machines(id) ON DELETE CASCADE
);

CREATE INDEX ix_maintenance_vending_machine_id ON maintenance(vending_machine_id);
CREATE INDEX ix_maintenance_date ON maintenance(date);

-- Таблица 9: Состояние автоматов
CREATE TABLE machine_status (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    vending_machine_id UUID NOT NULL UNIQUE,
    connection_status VARCHAR(50) DEFAULT 'Offline',
    last_online_at TIMESTAMP,
    coins_amount INT DEFAULT 0,
    bills_amount INT DEFAULT 0,
    coffee_level INT DEFAULT 0,
    sugar_level INT DEFAULT 0,
    milk_level INT DEFAULT 0,
    cups_level INT DEFAULT 0,
    lids_level INT DEFAULT 0,
    payment_system_status VARCHAR(50) DEFAULT 'OK',
    dispenser_status VARCHAR(50) DEFAULT 'OK',
    cooling_system_status VARCHAR(50) DEFAULT 'OK',
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_machine_status_vending_machines FOREIGN KEY (vending_machine_id)
        REFERENCES vending_machines(id) ON DELETE CASCADE
);

-- Таблица 10: События
CREATE TABLE events (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    vending_machine_id UUID NOT NULL,
    event_type VARCHAR(100) NOT NULL,
    event_description TEXT NOT NULL,
    event_date_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    severity VARCHAR(50) DEFAULT 'Info',

    CONSTRAINT fk_events_vending_machines FOREIGN KEY (vending_machine_id)
        REFERENCES vending_machines(id) ON DELETE CASCADE
);

CREATE INDEX ix_events_event_date_time ON events(event_date_time);
CREATE INDEX ix_events_vending_machine_id ON events(vending_machine_id);

-- Таблица 11: Новости
CREATE TABLE news (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    title VARCHAR(500) NOT NULL,
    content TEXT NOT NULL,
    published_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    author_id UUID NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,

    CONSTRAINT fk_news_users FOREIGN KEY (author_id)
        REFERENCES users(id) ON DELETE RESTRICT
);

CREATE INDEX ix_news_published_at ON news(published_at DESC);

-- Таблица 12: Договоры
CREATE TABLE contracts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    contract_number VARCHAR(50) UNIQUE NOT NULL,
    company_id UUID,
    vending_machine_id UUID NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    monthly_rent DECIMAL(10,2) NOT NULL,
    yearly_rent DECIMAL(10,2) NOT NULL,
    payback_period_months INT,
    insurance_required BOOLEAN DEFAULT FALSE,
    management_type VARCHAR(100),
    status VARCHAR(50) DEFAULT 'Draft',
    signed_at TIMESTAMP,
    signature_path VARCHAR(500),
    pdf_path VARCHAR(500),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_contracts_companies FOREIGN KEY (company_id)
        REFERENCES companies(id) ON DELETE SET NULL,
    CONSTRAINT fk_contracts_vending_machines FOREIGN KEY (vending_machine_id)
        REFERENCES vending_machines(id) ON DELETE RESTRICT
);

-- Таблица 13: Доступные для аренды
CREATE TABLE available_machines (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    vending_machine_id UUID NOT NULL,
    monthly_rent DECIMAL(10,2) NOT NULL,
    yearly_rent DECIMAL(10,2) NOT NULL,
    payback_period_months INT,
    placement_location VARCHAR(500),
    is_available BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_available_machines_vending_machines FOREIGN KEY (vending_machine_id)
        REFERENCES vending_machines(id) ON DELETE CASCADE
);
```

### Триггеры и функции

```sql
-- Триггер: обновление дохода при продаже
CREATE OR REPLACE FUNCTION update_total_revenue()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE vending_machines
    SET total_income = total_income + NEW.total_price,
        updated_at = CURRENT_TIMESTAMP
    WHERE id = NEW.vending_machine_id;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_update_revenue_after_sale
AFTER INSERT ON sales
FOR EACH ROW
EXECUTE FUNCTION update_total_revenue();

-- Функция: расчет эффективности сети
CREATE OR REPLACE FUNCTION calculate_network_efficiency()
RETURNS DECIMAL AS $$
DECLARE
    total_machines INT;
    working_machines INT;
    efficiency DECIMAL;
BEGIN
    SELECT COUNT(*) INTO total_machines FROM vending_machines;
    SELECT COUNT(*) INTO working_machines FROM vending_machines WHERE status = 'Working';

    IF total_machines > 0 THEN
        efficiency := (working_machines::DECIMAL / total_machines::DECIMAL) * 100;
    ELSE
        efficiency := 0;
    END IF;

    RETURN ROUND(efficiency, 2);
END;
$$ LANGUAGE plpgsql;
```

### Инструкции по импорту данных

#### Файлы для импорта

| Файл | Формат | Разделитель | Кодировка | Таблица |
|------|--------|-------------|-----------|---------|
| `vending_machines.csv` | CSV | `;` | WIN1251 | vending_machines |
| `sales.csv` | CSV | `;` | WIN1251 | sales |
| `maintenance.csv` | CSV | `;` | WIN1251 | maintenance |
| `products.json` | JSON | - | UTF-8 | products |
| `users/*.json` | JSON | - | UTF-8 | users |

#### Порядок импорта

**Важно:** Соблюдайте порядок из-за зависимостей (Foreign Keys)!

1. **vending_machines** (нет зависимостей от импортируемых данных)
2. **products** (зависит от vending_machines)
3. **sales** (зависит от products)
4. **maintenance** (зависит от vending_machines)

#### Импорт CSV файлов (прямой)

```sql
-- 1. Импорт vending_machines.csv
\copy vending_machines(serial_number, name, user_id, rfid_cash_collection, notes, location, work_mode, rfid_loading, model, kit_online_id, company, payment_type, critical_threshold_template, service_priority, manager, status, notification_template, working_hours, engineer, id, install_date, place, operator, technician, last_maintenance_date, rfid_service, coordinates, total_income, timezone) FROM 'C:/путь/vending_machines.csv' WITH (FORMAT csv, DELIMITER ';', HEADER true, ENCODING 'WIN1251');

-- 2. Импорт sales.csv
\copy sales(timestamp, product_id, total_price, quantity, payment_method) FROM 'C:\Users\Ernesto\Desktop\Chempionat\sales.csv' WITH (FORMAT csv, DELIMITER ';', HEADER true, ENCODING 'WIN1251');

-- 3. Импорт maintenance.csv
\copy maintenance(date, issues_found, vending_machine_id, full_name, work_description) FROM 'C:\Users\Ernesto\Desktop\Chempionat\maintenance.csv' WITH (FORMAT csv, DELIMITER ';', HEADER true, ENCODING 'WIN1251');
```

#### Импорт JSON файлов

**products.json:**
```sql
-- Создаём временную таблицу
CREATE TEMP TABLE temp_json (data JSONB);

-- Вставьте содержимое products.json (скопируйте весь JSON)
INSERT INTO temp_json VALUES ('[... содержимое products.json ...]');

-- Импорт напрямую в таблицу products
INSERT INTO products (id, name, price, min_stock, vending_machine_id, description, quantity_available, sales_trend)
SELECT
    (elem->>'id')::UUID,
    elem->>'name',
    (elem->>'price')::DECIMAL,
    (elem->>'min_stock')::INT,
    (elem->>'vending_machine_id')::UUID,
    elem->>'description',
    (elem->>'quantity_available')::INT,
    (elem->>'sales_trend')::DECIMAL
FROM temp_json, jsonb_array_elements(data) AS elem;

DROP TABLE temp_json;
```

**users/*.json (для каждого файла):**
```sql
-- Создаём временную таблицу
CREATE TEMP TABLE temp_user (data JSONB);

-- Вставьте содержимое одного user_xxx.json
INSERT INTO temp_user VALUES ('{... содержимое файла ...}');

-- Импорт в таблицу users
INSERT INTO users (id, email, full_name, phone, role, is_manager, is_engineer, is_operator, image)
SELECT
    (data->>'id')::UUID,
    data->>'email',
    data->>'full_name',
    data->>'phone',
    data->>'role',
    (data->>'is_manager')::BOOLEAN,
    (data->>'is_engineer')::BOOLEAN,
    (data->>'is_operator')::BOOLEAN,
    data->>'image'
FROM temp_user;

TRUNCATE temp_user;
-- Повторите для каждого файла пользователя...

DROP TABLE temp_user;
```

#### Обновление vending_machine_id в sales

После импорта sales нужно заполнить `vending_machine_id` через связь с products:

```sql
UPDATE sales s
SET vending_machine_id = p.vending_machine_id
FROM products p
WHERE s.product_id = p.id;
```

#### Проверка импорта

```sql
SELECT 'vending_machines' as table_name, COUNT(*) as count FROM vending_machines
UNION ALL SELECT 'products', COUNT(*) FROM products
UNION ALL SELECT 'sales', COUNT(*) FROM sales
UNION ALL SELECT 'users', COUNT(*) FROM users
UNION ALL SELECT 'maintenance', COUNT(*) FROM maintenance;
```

### Чек-лист

✅ 13 таблиц созданы
✅ Все Foreign Keys установлены
✅ Индексы добавлены
✅ Триггеры работают
✅ Функции работают
✅ Данные импортированы напрямую через \copy
✅ БД в 3НФ

---

## 2. Разработка API

### ⏱️ Время: 3 часа | 🎯 Цель: ASP.NET Core API с JWT

### Структура проекта

Создайте папки:
- Controllers/ - контроллеры
- Models/Entities/ - классы БД
- Models/DTOs/ - объекты передачи
- Data/ - DbContext
- Services/ - бизнес-логика
- Repositories/ - работа с БД
- Utilities/ - JWT, хеширование
- Validators/ - валидация

### Что реализовать

**Час 1: Настройка**
1. Создать проект ASP.NET Core Web API 8.0
2. Установить пакеты:
   - Npgsql.EntityFrameworkCore.PostgreSQL
   - Microsoft.AspNetCore.Authentication.JwtBearer
   - BCrypt.Net-Next
   - AutoMapper.Extensions.Microsoft.DependencyInjection
   - Swashbuckle.AspNetCore
   - Serilog.AspNetCore
   - FluentValidation.AspNetCore

3. Настроить Program.cs:
   - DbContext с PostgreSQL
   - JWT Authentication
   - CORS (AllowAll)
   - AutoMapper
   - Serilog
   - Swagger с JWT

4. Создать ApplicationDbContext
5. Создать Entity классы для всех таблиц

**Час 2: Controllers и Services**
6. Создать AuthController:
   - POST /api/auth/login (email, password → JWT token)
   - POST /api/auth/register-franchisee
   - POST /api/auth/generate-email-code
   - POST /api/auth/verify-email-code
   - POST /api/auth/verify-franchisee-code

7. Создать VendingMachinesController:
   - GET /api/vendingmachines (с пагинацией и фильтром)
   - GET /api/vendingmachines/{id}
   - POST /api/vendingmachines
   - PUT /api/vendingmachines/{id}
   - DELETE /api/vendingmachines/{id}
   - POST /api/vendingmachines/{id}/detach-modem
   - GET /api/vendingmachines/export/csv

8. Создать CompaniesController (аналогично)

**Час 3: Dashboard и Monitor**
9. Создать DashboardController:
   - GET /api/dashboard (всё для главной страницы)
   - GET /api/dashboard/network-efficiency
   - GET /api/dashboard/network-status
   - GET /api/dashboard/summary
   - GET /api/dashboard/sales-dynamics?days=10&filterBy=amount
   - GET /api/dashboard/news?count=5

10. Создать MonitorController:
    - GET /api/monitor (список ТА с фильтрами)
    - GET /api/monitor/{id}/status
    - GET /api/monitor/{id}/events
    - GET /api/monitor/export/excel

11. Создать ContractsController:
    - GET /api/contracts
    - GET /api/contracts/{id}
    - POST /api/contracts
    - POST /api/contracts/{id}/sign
    - GET /api/contracts/{id}/pdf
    - GET /api/contracts/available-machines

### Ключевые моменты

**JWT Token Generation:**
- Создать класс JwtTokenGenerator
- Метод GenerateToken(User user):
  - Claims: UserId, Email, Role
  - Expiration: 24 часа
  - SigningKey из конфигурации
  - Вернуть строку токена

**Password Hashing:**
- Использовать BCrypt.HashPassword(password)
- Проверять через BCrypt.Verify(password, hash)

**AutoMapper:**
- Создать профиль маппинга Entity ↔ DTO
- Использовать в Services

**FluentValidation:**
- Валидировать обязательные поля
- Проверять форматы (email, длина пароля)

**Middleware:**
- ErrorHandlingMiddleware для перехвата ошибок
- Логировать через Serilog
- Возвращать JSON с ошибкой

### Чек-лист

✅ Swagger UI работает
✅ JWT авторизация работает
✅ CRUD для ТА и Компаний работает
✅ Dashboard возвращает данные
✅ Monitor читает из MachineStatus
✅ Валидация работает
✅ Ошибки обрабатываются

---

## 3. Desktop приложение (WPF)

### ⏱️ Время: 3 часа | 🎯 Цель: WPF с Material Design и MVVM

### NuGet пакеты

Установить:
- CommunityToolkit.Mvvm
- MaterialDesignThemes + MaterialDesignColors
- MahApps.Metro
- LiveCharts2
- System.Net.Http.Json
- Newtonsoft.Json
- CsvHelper
- Microsoft.Extensions.DependencyInjection
- Serilog.Sinks.File

### Что реализовать

**Час 1: Структура и авторизация**
1. Создать структуру MVVM:
   - Models/
   - ViewModels/
   - Views/
   - Services/
   - Utilities/Converters/

2. Настроить DI в App.xaml.cs

3. Создать LoginWindow:
   - Material Design Card
   - Email + Password
   - Кнопка "Войти"
   - ProgressBar при загрузке

4. Создать LoginViewModel:
   - Свойства: Email, Password, IsBusy, ErrorMessage
   - Команда LoginCommand:
     - Валидация полей
     - POST к /api/auth/login
     - Сохранить токен
     - Открыть MainWindow

5. Создать ApiService:
   - HttpClient
   - Методы: GetAsync, PostAsync, PutAsync, DeleteAsync
   - Добавление токена в Authorization header
   - Обработка ошибок

**Час 2: Главное окно и страницы**
6. Создать MainWindow:
   - HamburgerMenu (MahApps.Metro)
   - Шапка с профилем
   - Frame для контента
   - Индикатор загрузки

7. Создать MainViewModel:
   - MenuItems (коллекция пунктов меню)
   - CurrentPage
   - TogglePaneCommand

8. Создать DashboardPage:
   - 5 блоков (3 карточки + график + новости)
   - Блок 1: Эффективность (CircularProgressBar)
   - Блок 2: Состояние (PieChart)
   - Блок 3: Сводка (список)
   - Блок 4: График продаж (CartesianChart)
   - Блок 5: Новости (ItemsControl)

9. Создать DashboardViewModel:
   - Загрузка данных с /api/dashboard
   - Обработка для графиков (LiveCharts2)
   - Переключение фильтра графика

**Час 3: Администрирование**
10. Создать VendingMachinesPage:
    - DataGrid с автоматами
    - Фильтр по названию
    - Пагинация (кнопки < 1 2 3 >)
    - ComboBox "Показать 10/25/50"
    - Кнопки: Добавить, Экспорт
    - Действия: Редактировать, Удалить, Отвязать модем

11. Создать VendingMachinesViewModel:
    - Загрузка с /api/vendingmachines
    - Пагинация
    - Фильтрация
    - CRUD операции
    - Экспорт в CSV (CsvHelper)

12. Создать AddEditVendingMachineDialog:
    - Модальное окно с полями
    - Валидация обязательных полей
    - Сохранение через POST/PUT

### Конвертеры XAML

Создать:
- BoolToVisibilityConverter
- StatusToColorConverter (Working→Green, NotWorking→Red)
- ConnectionStatusToIconConverter

### Чек-лист

✅ Авторизация работает
✅ Главная отображает все блоки
✅ График продаж переключается
✅ Меню раскрывается/скрывается
✅ Таблица ТА с пагинацией работает
✅ CRUD операции работают
✅ Экспорт CSV работает
✅ Удаление с подтверждением

---

## 4. Web приложение (Laravel)

### ⏱️ Время: 3 часа | 🎯 Цель: Laravel + Vue.js + Tailwind

### Установка

```bash
composer create-project laravel/laravel vending-web
cd vending-web

# Пакеты
composer require tymon/jwt-auth
composer require barryvdh/laravel-dompdf
composer require guzzlehttp/guzzle
composer require mewebstudio/captcha

# Frontend
npm install vue@next @vitejs/plugin-vue
npm install -D tailwindcss postcss autoprefixer
npm install axios signature_pad pdfjs-dist
```

### Что реализовать

**Час 1: Регистрация и авторизация**
1. Настроить .env (PostgreSQL!)
2. Настроить JWT (php artisan jwt:secret)
3. Настроить Vite для Vue + Tailwind

4. Создать RegisterController:
   - Шаг 1: Email + Password + CAPTCHA (3+ операции)
   - Шаг 2: Генерация email кода (показать в модалке)
   - Шаг 3: Проверка email кода
   - Шаг 4: Проверка кода франчайзи
   - Финал: Создание пользователя

5. Создать Vue компонент RegisterForm.vue:
   - Мультишаговая форма
   - Валидация на клиенте
   - Модальное окно для email кода
   - Axios для запросов

6. Создать LoginController:
   - Требует: email, password, franchisee_code
   - Генерация JWT
   - Редирект на dashboard

**Час 2: Dashboard и автоматы**
7. Создать layout app.blade.php:
   - Navbar с языками (RU/EN)
   - Sidebar
   - Основной контент

8. Создать VendingMachineController:
   - index() - список арендованных
   - available() - доступные для аренды
   - book() - бронирование

9. Создать страницу machines.blade.php:
   - Если нет автоматов: сообщение + кнопка
   - Модальное окно выбора автоматов (карточки)
   - Фильтры: модель, статус, сортировка
   - Форма бронирования (даты, страховка, способ ведения)
   - Эмуляция подтверждения в модалке

10. Создать Vue компонент AvailableMachines.vue:
    - Карточки автоматов
    - Чекбоксы для выбора
    - Фильтры и сортировка

11. Создать ApiService.php:
    - GET/POST/PUT/DELETE к .NET API
    - Добавление JWT токена
    - Обработка ошибок

**Час 3: Договоры**
12. Создать ContractController:
    - index() - список договоров
    - show() - просмотр PDF
    - sign() - подписание

13. Создать страницу contracts.blade.php:
    - Таблица договоров (номер, дата, статус)
    - Кнопка "Подписать" для неподписанных

14. Создать Vue компонент ContractViewer.vue:
    - PDF ридер (pdfjs-dist)
    - Canvas для подписи (signature_pad)
    - Кнопки: Очистить, Подписать
    - POST подписи в base64 к API

15. Создать PdfService.php:
    - generateContract() - генерация PDF договора
    - Использовать DomPDF
    - Шаблон в resources/views/pdf/contract.blade.php

16. Настроить мультиязычность:
    - Файлы ru.json, en.json
    - Переключатель в Navbar
    - Route для смены языка

### Blade шаблоны

**Tailwind CSS стили:**
- Кнопка primary: `bg-blue-500 hover:bg-blue-700 text-white py-2 px-4 rounded`
- Card: `bg-white shadow-md rounded-lg p-6`
- Input: `border rounded px-4 py-2 w-full`

### Чек-лист

✅ Регистрация с CAPTCHA работает
✅ Email код показывается в модалке
✅ Авторизация требует код франчайзи
✅ Языки переключаются (RU/EN)
✅ Список доступных автоматов загружается
✅ Бронирование создает договор
✅ Подтверждение эмулируется
✅ PDF договора открывается
✅ Подпись отправляется в API

---

## 5. Mobile Wireframe

### ⏱️ Время: 3 часа | 🎯 Цель: Wireframe для инженера

### Инструмент

Выбрать один: Figma / Adobe XD / Balsamiq / draw.io

### Экраны (14 основных)

**Час 1: Базовые экраны**
1. Вход (Login)
2. Главная (Dashboard) - список задач
3. Список задач (Tasks)
4. Детали задачи
5. Карта с маршрутом

**Час 2: Документирование**
6. Камера (фото/видео)
7. Создание заметки
8. Создание отчета (форма)
9. Электронная подпись клиента

**Час 3: Дополнительные**
10. Учет запчастей
11. Справка и руководства
12. Мониторинг состояния ТА
13. Мессенджер
14. Профиль

### Что включить

Для каждого экрана:
- Заголовок
- Основные элементы (кнопки, поля, списки)
- Bottom Navigation
- Аннотации (номера элементов)
- Описание функционала

### Требования

- Low-fidelity (низкая детализация)
- Размер: 375x812px (iPhone)
- Placeholder для текста и изображений
- Навигация стрелками между экранами
- Состояния: нормальное, загрузка, ошибка, пусто

### Результат

Создать 2 файла:
1. Исходник (.fig / .xd / .drawio)
2. PDF (один экран = одна страница)

### Чек-лист

✅ 14 экранов созданы
✅ Навигация показана
✅ Аннотации добавлены
✅ Файлы .fig и .pdf готовы
✅ Low-fidelity дизайн

---

## 6. Тестирование

### Тест-кейсы (Desktop - 10 шт)

Создать в Excel/Word по шаблону:
- ID, Название, Предусловия, Шаги, Ожидаемый результат, Приоритет

Примеры:
1. Авторизация с корректными данными
2. Добавление нового ТА
3. Фильтрация по названию
4. Экспорт в CSV
5. График продаж
6. Переключение фильтра графика
7. Удаление с подтверждением
8. Отмена удаления
9. Пагинация
10. Монитор ТА с фильтрами

### Selenium тесты (Web)

Установить Laravel Dusk:
```bash
composer require --dev laravel/dusk
php artisan dusk:install
```

Создать тесты:
- LoginTest - авторизация
- RegistrationTest - полный процесс
- MachinesListTest - список
- BookingTest - бронирование
- ContractSigningTest - подписание

---

## 7. Документация

### Use Case диаграмма

**Инструмент:** Enterprise Architect / StarUML / draw.io

**Что делать:**
1. Определить актеров (Administrator, Operator, Franchisee, Engineer)
2. Определить варианты использования для каждого
3. Создать диаграмму
4. Описать варианты использования (название, актер, предусловия, поток, постусловия)

**Файлы:** исходник + PDF

### IDEF0 диаграмма

**Инструмент:** RAMUS / Visio / draw.io

**Что делать:**
1. Контекстная диаграмма (A-0): одна функция "Управление сетью ТА"
   - Входы: Заявки, Данные
   - Выходы: Отчеты, Договоры
   - Управление: Правила
   - Механизмы: Персонал, ПО

2. Декомпозиция (A0): разбить на 5-6 функций
   - A1: Управление автоматами
   - A2: Управление компаниями
   - A3: Мониторинг
   - A4: Отчеты
   - A5: Договоры
   - A6: Обслуживание

**Файлы:** исходник + PDF

### Activity диаграмма

**Инструмент:** StarUML / draw.io / PlantUML

**Процесс:** "Бронирование торгового автомата"
- Swimlanes: Франчайзи, Система, Франчайзер
- Действия, решения, потоки

**Файлы:** исходник + PDF

### Postman коллекция

**Структура:**
```
Vending Machine API
├── Authentication (5 endpoints)
├── Vending Machines (7 endpoints)
├── Companies (6 endpoints)
├── Dashboard (6 endpoints)
├── Monitor (4 endpoints)
└── Contracts (6 endpoints)
```

**Для каждого:**
- Описание
- Параметры
- Примеры запросов/ответов
- Тесты (проверка status code, структуры)

**Переменные:**
- {{baseUrl}}
- {{token}}
- {{machineId}}
- {{companyId}}

**Экспорт:** JSON коллекции + JSON окружений

---

## Управление временем

### Каждый 3-часовой этап

**Час 1:** Настройка + основа
**Час 2:** Основной функционал
**Час 3:** Завершение + проверка

### Приоритеты

**Высокий (обязательно):**
- БД: все таблицы + данные
- API: Auth + CRUD + Dashboard
- Desktop: Авторизация + Главная + ТА
- Web: Регистрация + Автоматы + Бронирование
- Wireframe: 10-12 экранов

**Средний:**
- API: Monitor + Contracts
- Desktop: Монитор + Компании
- Web: Договоры

**Низкий:**
- Дополнительные фильтры
- Экспорт в разные форматы
- Уведомления

### Чек-лист перед сдачей

✅ БД: таблицы + связи + данные
✅ API: Swagger + JWT + CRUD
✅ Desktop: авторизация + главная + таблица ТА
✅ Web: регистрация + автоматы + бронирование
✅ Wireframe: экраны + PDF
✅ Документация: диаграммы + Postman

---

## Частые ошибки

❌ Усложнение (делай просто!)
❌ Функции не из PDF
❌ Фокус на красоту (функционал важнее)
❌ Попытки "идеально" (делай "работающим")
❌ Игнорирование PDF
❌ Нет тестовых данных
❌ Hardcode значений

---

**Успехов в реализации! 🚀**
