# Создание веб-приложения "Vending Machine" с нуля

Это пошаговое руководство описывает создание веб-приложения для управления торговыми автоматами на базе Laravel 12, Tailwind CSS и Alpine.js.

---

## Оглавление

1. [Требования к системе](#1-требования-к-системе)
2. [Установка необходимого ПО](#2-установка-необходимого-по)
3. [Создание проекта Laravel](#3-создание-проекта-laravel)
4. [Настройка окружения](#4-настройка-окружения)
5. [Настройка базы данных](#5-настройка-базы-данных)
6. [Создание структуры проекта](#6-создание-структуры-проекта)
7. [Создание моделей и миграций](#7-создание-моделей-и-миграций)
8. [Создание контроллеров](#8-создание-контроллеров)
9. [Настройка маршрутов](#9-настройка-маршрутов)
10. [Создание Middleware](#10-создание-middleware)
11. [Создание сервисов](#11-создание-сервисов)
12. [Настройка фронтенда](#12-настройка-фронтенда)
13. [Создание шаблонов Blade](#13-создание-шаблонов-blade)
14. [Запуск приложения](#14-запуск-приложения)
15. [Структура готового проекта](#15-структура-готового-проекта)

---

## 1. Требования к системе

### Минимальные требования:

| Компонент | Версия |
|-----------|--------|
| PHP | 8.2+ |
| Composer | 2.x |
| Node.js | 18+ |
| NPM | 9+ |
| SQLite / MySQL / PostgreSQL | любая |

### Необходимые расширения PHP:

- `php-mbstring`
- `php-xml`
- `php-curl`
- `php-sqlite3` (для SQLite)
- `php-pdo`
- `php-zip`

---

## 2. Установка необходимого ПО

### Windows

#### Установка PHP (через XAMPP или отдельно):

**Вариант 1: XAMPP (рекомендуется для начинающих)**
```bash
# Скачайте и установите XAMPP с https://www.apachefriends.org/
# PHP будет доступен по пути: C:\xampp\php\php.exe
# Добавьте C:\xampp\php в системную переменную PATH
```

**Вариант 2: Chocolatey**
```powershell
# Установите Chocolatey (если не установлен)
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

# Установите PHP
choco install php -y
```

#### Установка Composer:
```powershell
# Скачайте и запустите установщик с https://getcomposer.org/download/
# Или через Chocolatey:
choco install composer -y
```

#### Установка Node.js:
```powershell
# Скачайте с https://nodejs.org/
# Или через Chocolatey:
choco install nodejs-lts -y
```

### macOS

```bash
# Установка Homebrew (если не установлен)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Установка PHP
brew install php

# Установка Composer
brew install composer

# Установка Node.js
brew install node
```

### Linux (Ubuntu/Debian)

```bash
# Обновление пакетов
sudo apt update && sudo apt upgrade -y

# Установка PHP и расширений
sudo apt install php8.2 php8.2-cli php8.2-mbstring php8.2-xml php8.2-curl php8.2-sqlite3 php8.2-zip -y

# Установка Composer
curl -sS https://getcomposer.org/installer | php
sudo mv composer.phar /usr/local/bin/composer

# Установка Node.js (через NodeSource)
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt install nodejs -y
```

### Проверка установки:

```bash
php --version      # Должно показать PHP 8.2+
composer --version # Должно показать Composer 2.x
node --version     # Должно показать v18+
npm --version      # Должно показать 9+
```

---

## 3. Создание проекта Laravel

```bash
# Перейдите в директорию, где хотите создать проект
cd C:\Users\YourName\Projects

# Создайте новый Laravel проект
composer create-project laravel/laravel VendingMachineWeb

# Перейдите в директорию проекта
cd VendingMachineWeb
```

---

## 4. Настройка окружения

### Скопируйте файл .env.example в .env:

```bash
# Windows
copy .env.example .env

# macOS/Linux
cp .env.example .env
```

### Сгенерируйте ключ приложения:

```bash
php artisan key:generate
```

### Настройте файл `.env`:

```env
APP_NAME="Vending Machine"
APP_ENV=local
APP_DEBUG=true
APP_URL=http://localhost:8000

APP_LOCALE=ru
APP_FALLBACK_LOCALE=en

# База данных SQLite (простой вариант)
DB_CONNECTION=sqlite

# Сессии и кеш через базу данных
SESSION_DRIVER=database
SESSION_LIFETIME=120
CACHE_STORE=database
QUEUE_CONNECTION=database

# URL внешнего API (если используется)
VENDING_API_URL=http://localhost:5000
```

---

## 5. Настройка базы данных

### Для SQLite:

```bash
# Создайте файл базы данных
# Windows
type nul > database\database.sqlite

# macOS/Linux
touch database/database.sqlite
```

### Создайте таблицы для сессий, кеша и очередей:

```bash
php artisan session:table
php artisan cache:table
php artisan queue:table
```

---

## 6. Создание структуры проекта

### Создайте необходимые директории:

```bash
# Директория для сервисов
mkdir app\Services

# Директория для изображений
mkdir public\images
```

---

## 7. Создание моделей и миграций

### Создайте миграцию для торговых автоматов:

```bash
php artisan make:migration create_vending_machines_table
```

**Содержимое файла** `database/migrations/xxxx_xx_xx_create_vending_machines_table.php`:

```php
<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('vending_machines', function (Blueprint $table) {
            $table->uuid('id')->primary();
            $table->string('serial_number', 50)->unique();
            $table->string('name', 255);
            $table->string('model', 255);
            $table->string('status', 50)->default('Working');
            $table->string('location', 500);
            $table->string('place', 500)->nullable();
            $table->string('coordinates', 100)->nullable();
            $table->timestamp('install_date');
            $table->timestamp('last_maintenance_date')->nullable();
            $table->string('working_hours', 50)->nullable();
            $table->string('timezone', 50)->nullable();
            $table->decimal('total_income', 18, 2)->default(0);
            $table->uuid('user_id')->nullable();
            $table->string('work_mode', 100)->nullable();
            $table->string('rfid_cash_collection', 50)->nullable();
            $table->string('rfid_loading', 50)->nullable();
            $table->string('rfid_service', 50)->nullable();
            $table->string('kit_online_id', 50)->nullable();
            $table->string('company', 255)->nullable();
            $table->string('payment_type', 255)->nullable();
            $table->string('critical_threshold_template', 100)->nullable();
            $table->string('service_priority', 50)->nullable();
            $table->string('manager', 255)->nullable();
            $table->string('notification_template', 100)->nullable();
            $table->string('engineer', 255)->nullable();
            $table->string('operator', 255)->nullable();
            $table->string('technician', 255)->nullable();
            $table->text('notes')->nullable();
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('vending_machines');
    }
};
```

### Создайте миграцию для модемов:

```bash
php artisan make:migration create_modems_table
```

**Содержимое файла** `database/migrations/xxxx_xx_xx_create_modems_table.php`:

```php
<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('modems', function (Blueprint $table) {
            $table->uuid('id')->primary();
            $table->string('modem_number', 50)->unique();
            $table->string('model', 100)->nullable();
            $table->string('status', 50)->default('Active');
            $table->uuid('vending_machine_id')->nullable();
            $table->timestamps();

            $table->foreign('vending_machine_id')
                ->references('id')
                ->on('vending_machines')
                ->onDelete('set null');
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('modems');
    }
};
```

### Создайте модели:

**Файл** `app/Models/VendingMachine.php`:

```php
<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Concerns\HasUuids;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\HasOne;

class VendingMachine extends Model
{
    use HasUuids;

    protected $keyType = 'string';
    public $incrementing = false;

    protected $fillable = [
        'id',
        'serial_number',
        'name',
        'model',
        'status',
        'location',
        'place',
        'coordinates',
        'install_date',
        'last_maintenance_date',
        'working_hours',
        'timezone',
        'total_income',
        'user_id',
        'work_mode',
        'rfid_cash_collection',
        'rfid_loading',
        'rfid_service',
        'kit_online_id',
        'company',
        'payment_type',
        'critical_threshold_template',
        'service_priority',
        'manager',
        'notification_template',
        'engineer',
        'operator',
        'technician',
        'notes',
    ];

    protected $casts = [
        'install_date' => 'datetime',
        'last_maintenance_date' => 'datetime',
        'total_income' => 'decimal:2',
    ];

    public function modem(): HasOne
    {
        return $this->hasOne(Modem::class);
    }
}
```

**Файл** `app/Models/Modem.php`:

```php
<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Concerns\HasUuids;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class Modem extends Model
{
    use HasUuids;

    protected $keyType = 'string';
    public $incrementing = false;

    protected $fillable = [
        'id',
        'modem_number',
        'model',
        'status',
        'vending_machine_id',
    ];

    public function vendingMachine(): BelongsTo
    {
        return $this->belongsTo(VendingMachine::class);
    }
}
```

### Выполните миграции:

```bash
php artisan migrate
```

---

## 8. Создание контроллеров

### Создайте контроллеры:

```bash
php artisan make:controller AuthController
php artisan make:controller DashboardController
php artisan make:controller VendingMachineController
php artisan make:controller ContractController
```

**Файл** `app/Http/Controllers/AuthController.php`:

```php
<?php

namespace App\Http\Controllers;

use App\Services\VendingApiService;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Session;

class AuthController extends Controller
{
    protected VendingApiService $apiService;

    public function __construct(VendingApiService $apiService)
    {
        $this->apiService = $apiService;
    }

    public function showLogin()
    {
        return view('auth.login');
    }

    public function login(Request $request)
    {
        $credentials = $request->validate([
            'email' => 'required|email',
            'password' => 'required',
        ]);

        $response = $this->apiService->login($credentials['email'], $credentials['password']);

        if ($response['success']) {
            $userData = $response['data'];

            Session::put('user', [
                'id' => $userData['userId'] ?? null,
                'email' => $userData['email'] ?? $credentials['email'],
                'name' => $userData['fullName'] ?? 'User',
                'role' => $userData['role'] ?? 'User',
            ]);

            Session::put('logged_in', true);
            $request->session()->regenerate();

            return redirect()->intended('/vending-machines');
        }

        return back()->withErrors([
            'email' => 'Неверные учетные данные.',
        ]);
    }

    public function showRegister()
    {
        return view('auth.register');
    }

    public function register(Request $request)
    {
        $validated = $request->validate([
            'name' => 'required|string|max:255',
            'email' => 'required|email:rfc,dns',
            'password' => [
                'required',
                'confirmed',
                'min:8',
                'regex:/[0-9]/',
                'regex:/[@$!%*#?&]/',
            ],
        ], [
            'email.email' => 'Введите корректный email адрес.',
            'password.min' => 'Пароль должен содержать минимум 8 символов.',
            'password.regex' => 'Пароль должен содержать цифры и спецсимволы.',
            'password.confirmed' => 'Пароли не совпадают.',
        ]);

        $response = $this->apiService->register(
            $validated['email'],
            $validated['password'],
            $validated['name']
        );

        if ($response['success']) {
            return redirect()->route('login')
                ->with('success', 'Регистрация успешна!');
        }

        return back()->withErrors([
            'email' => $response['message'] ?? 'Ошибка регистрации',
        ])->withInput();
    }

    public function logout(Request $request)
    {
        $this->apiService->clearToken();
        Session::forget('user');
        Session::forget('logged_in');
        $request->session()->invalidate();
        $request->session()->regenerateToken();

        return redirect('/login');
    }
}
```

**Файл** `app/Http/Controllers/DashboardController.php`:

```php
<?php

namespace App\Http\Controllers;

use App\Services\VendingApiService;

class DashboardController extends Controller
{
    protected VendingApiService $apiService;

    public function __construct(VendingApiService $apiService)
    {
        $this->apiService = $apiService;
    }

    public function index()
    {
        $response = $this->apiService->getDashboard();

        $stats = [
            'total_machines' => 0,
            'working_machines' => 0,
            'not_working_machines' => 0,
            'total_income' => 0,
        ];

        if ($response['success'] && isset($response['data'])) {
            $data = $response['data'];
            $stats = [
                'total_machines' => $data['totalMachines'] ?? 0,
                'working_machines' => $data['workingMachines'] ?? 0,
                'not_working_machines' => $data['notWorkingMachines'] ?? 0,
                'total_income' => $data['totalIncome'] ?? 0,
            ];
        }

        return view('dashboard', compact('stats'));
    }
}
```

**Файл** `app/Http/Controllers/VendingMachineController.php`:

```php
<?php

namespace App\Http\Controllers;

use App\Services\VendingApiService;
use Illuminate\Http\Request;
use Illuminate\Pagination\LengthAwarePaginator;

class VendingMachineController extends Controller
{
    protected VendingApiService $apiService;

    public function __construct(VendingApiService $apiService)
    {
        $this->apiService = $apiService;
    }

    public function index(Request $request)
    {
        $perPage = $request->input('per_page', 10);
        $page = $request->input('page', 1);
        $search = $request->input('search');

        $response = $this->apiService->getVendingMachines($page, $perPage, $search);

        $machines = [];
        $total = 0;

        if ($response['success'] && isset($response['data'])) {
            $machines = collect($response['data'])->map(function ($item) {
                return $this->transformMachine($item);
            })->toArray();
            $total = (int) ($response['total'] ?? count($machines));
        }

        $paginator = new LengthAwarePaginator(
            $machines,
            $total,
            $perPage,
            $page,
            [
                'path' => $request->url(),
                'query' => $request->query(),
            ]
        );

        return view('vending-machines.index', [
            'machines' => $paginator,
            'search' => $search,
            'perPage' => $perPage,
        ]);
    }

    public function create()
    {
        return view('vending-machines.create');
    }

    public function store(Request $request)
    {
        $validated = $request->validate([
            'serial_number' => 'required|string|max:50',
            'name' => 'required|string|max:255',
            'model' => 'required|string|max:255',
            'location' => 'required|string|max:500',
            'status' => 'nullable|string|max:50',
        ]);

        $response = $this->apiService->createVendingMachine($validated);

        if ($response['success']) {
            return redirect()->route('vending-machines.index')
                ->with('success', 'Торговый автомат создан!');
        }

        return back()->withErrors(['error' => $response['message']])->withInput();
    }

    public function edit(string $id)
    {
        $response = $this->apiService->getVendingMachine($id);

        if (!$response['success']) {
            return redirect()->route('vending-machines.index')
                ->with('error', 'Автомат не найден');
        }

        $machine = $this->transformMachine($response['data']);

        return view('vending-machines.edit', compact('machine'));
    }

    public function update(Request $request, string $id)
    {
        $validated = $request->validate([
            'name' => 'required|string|max:255',
            'model' => 'required|string|max:255',
            'location' => 'required|string|max:500',
            'status' => 'nullable|string|max:50',
        ]);

        $response = $this->apiService->updateVendingMachine($id, $validated);

        if ($response['success']) {
            return redirect()->route('vending-machines.index')
                ->with('success', 'Автомат обновлен!');
        }

        return back()->withErrors(['error' => $response['message']])->withInput();
    }

    public function destroy(string $id)
    {
        $response = $this->apiService->deleteVendingMachine($id);

        if ($response['success']) {
            return redirect()->route('vending-machines.index')
                ->with('success', 'Автомат удален!');
        }

        return back()->with('error', $response['message']);
    }

    public function export()
    {
        $response = $this->apiService->getVendingMachines(1, 1000);

        $callback = function() use ($response) {
            $file = fopen('php://output', 'w');
            fprintf($file, chr(0xEF).chr(0xBB).chr(0xBF));

            fputcsv($file, ['ID', 'Название', 'Модель', 'Статус', 'Локация'], ';');

            if ($response['success'] && isset($response['data'])) {
                foreach ($response['data'] as $machine) {
                    fputcsv($file, [
                        $machine['id'] ?? '',
                        $machine['name'] ?? '',
                        $machine['model'] ?? '',
                        $machine['status'] ?? '',
                        $machine['location'] ?? '',
                    ], ';');
                }
            }

            fclose($file);
        };

        $filename = 'vending_machines_' . date('Y-m-d') . '.csv';

        return response()->stream($callback, 200, [
            'Content-Type' => 'text/csv; charset=UTF-8',
            'Content-Disposition' => "attachment; filename=\"$filename\"",
        ]);
    }

    protected function transformMachine(array $data): array
    {
        return [
            'id' => $data['id'] ?? null,
            'serial_number' => $data['serialNumber'] ?? $data['serial_number'] ?? '',
            'name' => $data['name'] ?? '',
            'model' => $data['model'] ?? '',
            'status' => $data['status'] ?? 'Unknown',
            'location' => $data['location'] ?? '',
            'place' => $data['place'] ?? '',
            'coordinates' => $data['coordinates'] ?? '',
            'install_date' => isset($data['installDate'])
                ? new \DateTime($data['installDate'])
                : null,
            'total_income' => $data['totalIncome'] ?? 0,
            'kit_online_id' => $data['kitOnlineId'] ?? null,
            'modem' => $data['modem'] ?? null,
        ];
    }
}
```

**Файл** `app/Http/Controllers/ContractController.php`:

```php
<?php

namespace App\Http\Controllers;

class ContractController extends Controller
{
    public function index()
    {
        return view('contracts.index');
    }
}
```

---

## 9. Настройка маршрутов

**Файл** `routes/web.php`:

```php
<?php

use App\Http\Controllers\VendingMachineController;
use App\Http\Controllers\AuthController;
use App\Http\Controllers\DashboardController;
use App\Http\Controllers\ContractController;
use Illuminate\Support\Facades\Route;

// Публичные маршруты авторизации
Route::get('/login', [AuthController::class, 'showLogin'])->name('login');
Route::post('/login', [AuthController::class, 'login'])->name('login.submit');
Route::get('/register', [AuthController::class, 'showRegister'])->name('register');
Route::post('/register', [AuthController::class, 'register'])->name('register.submit');
Route::post('/logout', [AuthController::class, 'logout'])->name('logout');

// Защищенные маршруты
Route::middleware('api.auth')->group(function () {
    // Главная страница
    Route::get('/', [DashboardController::class, 'index'])->name('dashboard');

    // Торговые автоматы
    Route::get('/vending-machines', [VendingMachineController::class, 'index'])
        ->name('vending-machines.index');
    Route::get('/vending-machines/export', [VendingMachineController::class, 'export'])
        ->name('vending-machines.export');
    Route::get('/vending-machines/create', [VendingMachineController::class, 'create'])
        ->name('vending-machines.create');
    Route::post('/vending-machines', [VendingMachineController::class, 'store'])
        ->name('vending-machines.store');
    Route::get('/vending-machines/{id}/edit', [VendingMachineController::class, 'edit'])
        ->name('vending-machines.edit');
    Route::put('/vending-machines/{id}', [VendingMachineController::class, 'update'])
        ->name('vending-machines.update');
    Route::delete('/vending-machines/{id}', [VendingMachineController::class, 'destroy'])
        ->name('vending-machines.destroy');

    // Договоры
    Route::get('/contracts', [ContractController::class, 'index'])->name('contracts.index');
});

// Переключатель языка
Route::get('/lang/{locale}', function ($locale) {
    if (in_array($locale, ['ru', 'en'])) {
        session(['locale' => $locale]);
    }
    return back();
})->name('lang.switch');
```

---

## 10. Создание Middleware

### Создайте middleware для авторизации:

```bash
php artisan make:middleware ApiAuth
php artisan make:middleware SetLocale
```

**Файл** `app/Http/Middleware/ApiAuth.php`:

```php
<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Session;
use Symfony\Component\HttpFoundation\Response;

class ApiAuth
{
    public function handle(Request $request, Closure $next): Response
    {
        if (!Session::get('logged_in') || !Session::get('api_token')) {
            return redirect()->route('login')
                ->with('error', 'Необходима авторизация');
        }

        return $next($request);
    }
}
```

**Файл** `app/Http/Middleware/SetLocale.php`:

```php
<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\App;
use Illuminate\Support\Facades\Session;
use Symfony\Component\HttpFoundation\Response;

class SetLocale
{
    public function handle(Request $request, Closure $next): Response
    {
        $locale = Session::get('locale', config('app.locale'));

        if (in_array($locale, ['ru', 'en'])) {
            App::setLocale($locale);
        }

        return $next($request);
    }
}
```

### Зарегистрируйте middleware:

**Файл** `bootstrap/app.php`:

```php
<?php

use Illuminate\Foundation\Application;
use Illuminate\Foundation\Configuration\Exceptions;
use Illuminate\Foundation\Configuration\Middleware;

return Application::configure(basePath: dirname(__DIR__))
    ->withRouting(
        web: __DIR__.'/../routes/web.php',
        commands: __DIR__.'/../routes/console.php',
        health: '/up',
    )
    ->withMiddleware(function (Middleware $middleware) {
        $middleware->alias([
            'api.auth' => \App\Http\Middleware\ApiAuth::class,
        ]);

        $middleware->web(append: [
            \App\Http\Middleware\SetLocale::class,
        ]);
    })
    ->withExceptions(function (Exceptions $exceptions) {
        //
    })->create();
```

---

## 11. Создание сервисов

**Файл** `app/Services/VendingApiService.php`:

```php
<?php

namespace App\Services;

use Illuminate\Support\Facades\Http;
use Illuminate\Support\Facades\Session;

class VendingApiService
{
    protected string $baseUrl;

    public function __construct()
    {
        $this->baseUrl = config('services.vending_api.url', 'http://localhost:5000');
    }

    protected function getToken(): ?string
    {
        return Session::get('api_token');
    }

    public function setToken(string $token): void
    {
        Session::put('api_token', $token);
    }

    public function clearToken(): void
    {
        Session::forget('api_token');
    }

    public function register(string $email, string $password, string $fullName): array
    {
        $response = Http::withOptions([
            'verify' => false,
        ])->post("{$this->baseUrl}/api/auth/register", [
            'email' => $email,
            'password' => $password,
            'fullName' => $fullName,
        ]);

        if ($response->successful()) {
            return ['success' => true, 'data' => $response->json()];
        }

        return [
            'success' => false,
            'message' => $response->json('message') ?? 'Registration failed',
            'status' => $response->status(),
        ];
    }

    public function login(string $email, string $password): array
    {
        $response = Http::withOptions([
            'verify' => false,
        ])->post("{$this->baseUrl}/api/auth/login", [
            'email' => $email,
            'password' => $password,
        ]);

        if ($response->successful()) {
            $data = $response->json();
            if (isset($data['token'])) {
                $this->setToken($data['token']);
            }
            return ['success' => true, 'data' => $data];
        }

        return [
            'success' => false,
            'message' => $response->json('message') ?? 'Login failed',
            'status' => $response->status(),
        ];
    }

    public function get(string $endpoint, array $query = []): array
    {
        $token = $this->getToken();

        $response = Http::withOptions([
            'verify' => false,
        ])->withHeaders([
            'Authorization' => "Bearer {$token}",
        ])->get("{$this->baseUrl}{$endpoint}", $query);

        return $this->handleResponse($response);
    }

    public function post(string $endpoint, array $data = []): array
    {
        $token = $this->getToken();

        $response = Http::withOptions([
            'verify' => false,
        ])->withHeaders([
            'Authorization' => "Bearer {$token}",
        ])->post("{$this->baseUrl}{$endpoint}", $data);

        return $this->handleResponse($response);
    }

    public function put(string $endpoint, array $data = []): array
    {
        $token = $this->getToken();

        $response = Http::withOptions([
            'verify' => false,
        ])->withHeaders([
            'Authorization' => "Bearer {$token}",
        ])->put("{$this->baseUrl}{$endpoint}", $data);

        return $this->handleResponse($response);
    }

    public function delete(string $endpoint): array
    {
        $token = $this->getToken();

        $response = Http::withOptions([
            'verify' => false,
        ])->withHeaders([
            'Authorization' => "Bearer {$token}",
        ])->delete("{$this->baseUrl}{$endpoint}");

        return $this->handleResponse($response);
    }

    protected function handleResponse($response): array
    {
        if ($response->successful()) {
            return [
                'success' => true,
                'data' => $response->json(),
                'total' => $response->header('X-Total-Count'),
            ];
        }

        return [
            'success' => false,
            'message' => $response->json('message') ?? 'Request failed',
            'status' => $response->status(),
        ];
    }

    public function getVendingMachines(int $page = 1, int $pageSize = 10, ?string $search = null): array
    {
        $query = ['page' => $page, 'pageSize' => $pageSize];
        if ($search) {
            $query['search'] = $search;
        }
        return $this->get('/api/vendingmachines', $query);
    }

    public function getVendingMachine(string $id): array
    {
        return $this->get("/api/vendingmachines/{$id}");
    }

    public function createVendingMachine(array $data): array
    {
        return $this->post('/api/vendingmachines', $data);
    }

    public function updateVendingMachine(string $id, array $data): array
    {
        return $this->put("/api/vendingmachines/{$id}", $data);
    }

    public function deleteVendingMachine(string $id): array
    {
        return $this->delete("/api/vendingmachines/{$id}");
    }

    public function getDashboard(): array
    {
        return $this->get('/api/dashboard');
    }
}
```

### Добавьте конфигурацию API:

**Файл** `config/services.php` (добавьте в конец массива):

```php
'vending_api' => [
    'url' => env('VENDING_API_URL', 'http://localhost:5000'),
],
```

---

## 12. Настройка фронтенда

### Установите NPM зависимости:

```bash
npm install
```

### Обновите `package.json`:

```json
{
    "private": true,
    "type": "module",
    "scripts": {
        "build": "vite build",
        "dev": "vite"
    },
    "devDependencies": {
        "@tailwindcss/vite": "^4.0.0",
        "axios": "^1.11.0",
        "concurrently": "^9.0.1",
        "laravel-vite-plugin": "^2.0.0",
        "tailwindcss": "^4.0.0",
        "vite": "^7.0.7"
    }
}
```

### Установите зависимости:

```bash
npm install
```

### Настройте Vite:

**Файл** `vite.config.js`:

```javascript
import { defineConfig } from 'vite';
import laravel from 'laravel-vite-plugin';
import tailwindcss from '@tailwindcss/vite';

export default defineConfig({
    plugins: [
        laravel({
            input: ['resources/css/app.css', 'resources/js/app.js'],
            refresh: true,
        }),
        tailwindcss(),
    ],
    server: {
        watch: {
            ignored: ['**/storage/framework/views/**'],
        },
    },
});
```

### Настройте CSS:

**Файл** `resources/css/app.css`:

```css
@import 'tailwindcss';

@source '../../vendor/laravel/framework/src/Illuminate/Pagination/resources/views/*.blade.php';
@source '../../storage/framework/views/*.php';
@source '../**/*.blade.php';
@source '../**/*.js';

@theme {
    --font-sans: 'Inter', ui-sans-serif, system-ui, sans-serif;
}
```

### Настройте JavaScript:

**Файл** `resources/js/app.js`:

```javascript
import './bootstrap';
```

**Файл** `resources/js/bootstrap.js`:

```javascript
import axios from 'axios';
window.axios = axios;

window.axios.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
```

---

## 13. Создание шаблонов Blade

### Создайте структуру папок:

```bash
mkdir resources\views\layouts
mkdir resources\views\auth
mkdir resources\views\vending-machines
mkdir resources\views\contracts
```

### Основной layout:

**Файл** `resources/views/layouts/app.blade.php`:

```html
<!DOCTYPE html>
<html lang="{{ app()->getLocale() }}">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="csrf-token" content="{{ csrf_token() }}">
    <title>@yield('title', 'Главная') - Vending Machine</title>

    <!-- Tailwind CSS CDN -->
    <script src="https://cdn.tailwindcss.com"></script>

    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">

    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">

    <script>
        tailwind.config = {
            theme: {
                extend: {
                    fontFamily: {
                        sans: ['Inter', 'sans-serif'],
                    },
                    colors: {
                        primary: {
                            500: '#3b82f6',
                            600: '#2563eb',
                            700: '#1d4ed8',
                        },
                        sidebar: {
                            dark: '#1e293b',
                            darker: '#0f172a',
                        }
                    }
                }
            }
        }
    </script>

    <style>
        [x-cloak] { display: none !important; }
    </style>

    <!-- Alpine.js -->
    <script defer src="https://cdn.jsdelivr.net/npm/alpinejs@3.x.x/dist/cdn.min.js"></script>
</head>
<body class="bg-gray-100 min-h-screen font-sans">
    <div class="flex h-screen overflow-hidden">
        <!-- Sidebar -->
        <aside class="w-64 bg-gradient-to-b from-sidebar-dark to-sidebar-darker flex-shrink-0">
            <div class="h-20 flex items-center justify-center px-4 border-b border-white/10">
                <span class="text-2xl font-bold text-white">Vending</span>
            </div>

            <nav class="py-4 px-3">
                <a href="{{ route('dashboard') }}"
                   class="flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-white/5 rounded-lg mb-1">
                    <i class="fas fa-home mr-3"></i>
                    <span>Главная</span>
                </a>

                <a href="{{ route('vending-machines.index') }}"
                   class="flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-white/5 rounded-lg mb-1">
                    <i class="fas fa-cash-register mr-3"></i>
                    <span>Торговые автоматы</span>
                </a>

                <a href="{{ route('contracts.index') }}"
                   class="flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-white/5 rounded-lg mb-1">
                    <i class="fas fa-file-contract mr-3"></i>
                    <span>Договоры</span>
                </a>

                <form method="POST" action="{{ route('logout') }}" class="mt-4">
                    @csrf
                    <button type="submit" class="w-full flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-red-500/10 rounded-lg">
                        <i class="fas fa-sign-out-alt mr-3"></i>
                        <span>Выход</span>
                    </button>
                </form>
            </nav>
        </aside>

        <!-- Main Content -->
        <div class="flex-1 flex flex-col overflow-hidden">
            <header class="h-16 bg-white shadow-sm flex items-center justify-between px-6">
                <h1 class="text-xl font-semibold text-gray-800">@yield('title', 'Главная')</h1>
                <span class="text-gray-600">{{ session('user.name', 'User') }}</span>
            </header>

            <main class="flex-1 overflow-y-auto p-6 bg-gray-50">
                @yield('content')
            </main>
        </div>
    </div>

    @stack('scripts')
</body>
</html>
```

### Страница входа:

**Файл** `resources/views/auth/login.blade.php`:

```html
<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Вход - Vending Machine</title>
    <script src="https://cdn.tailwindcss.com"></script>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600&display=swap" rel="stylesheet">
</head>
<body class="bg-gray-100 min-h-screen flex items-center justify-center font-['Inter']">
    <div class="w-full max-w-md">
        <div class="bg-white rounded-xl shadow-lg p-8">
            <h2 class="text-2xl font-bold text-center text-gray-800 mb-8">Вход в систему</h2>

            @if($errors->any())
                <div class="bg-red-50 text-red-600 p-4 rounded-lg mb-6">
                    {{ $errors->first() }}
                </div>
            @endif

            @if(session('success'))
                <div class="bg-green-50 text-green-600 p-4 rounded-lg mb-6">
                    {{ session('success') }}
                </div>
            @endif

            <form method="POST" action="{{ route('login.submit') }}">
                @csrf

                <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Email</label>
                    <input type="email" name="email" value="{{ old('email') }}" required
                           class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500">
                </div>

                <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Пароль</label>
                    <input type="password" name="password" required
                           class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500">
                </div>

                <button type="submit"
                        class="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-4 rounded-lg transition">
                    Войти
                </button>
            </form>

            <p class="mt-6 text-center text-gray-600">
                Нет аккаунта?
                <a href="{{ route('register') }}" class="text-blue-600 hover:underline">Зарегистрироваться</a>
            </p>
        </div>
    </div>
</body>
</html>
```

### Страница регистрации:

**Файл** `resources/views/auth/register.blade.php`:

```html
<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Регистрация - Vending Machine</title>
    <script src="https://cdn.tailwindcss.com"></script>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600&display=swap" rel="stylesheet">
</head>
<body class="bg-gray-100 min-h-screen flex items-center justify-center font-['Inter']">
    <div class="w-full max-w-md">
        <div class="bg-white rounded-xl shadow-lg p-8">
            <h2 class="text-2xl font-bold text-center text-gray-800 mb-8">Регистрация</h2>

            @if($errors->any())
                <div class="bg-red-50 text-red-600 p-4 rounded-lg mb-6">
                    {{ $errors->first() }}
                </div>
            @endif

            <form method="POST" action="{{ route('register.submit') }}">
                @csrf

                <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Имя</label>
                    <input type="text" name="name" value="{{ old('name') }}" required
                           class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                </div>

                <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Email</label>
                    <input type="email" name="email" value="{{ old('email') }}" required
                           class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                </div>

                <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Пароль</label>
                    <input type="password" name="password" required
                           class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                    <p class="text-xs text-gray-500 mt-1">Минимум 8 символов, цифры и спецсимволы</p>
                </div>

                <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Подтверждение пароля</label>
                    <input type="password" name="password_confirmation" required
                           class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                </div>

                <button type="submit"
                        class="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-4 rounded-lg transition">
                    Зарегистрироваться
                </button>
            </form>

            <p class="mt-6 text-center text-gray-600">
                Уже есть аккаунт?
                <a href="{{ route('login') }}" class="text-blue-600 hover:underline">Войти</a>
            </p>
        </div>
    </div>
</body>
</html>
```

### Главная страница (Dashboard):

**Файл** `resources/views/dashboard.blade.php`:

```html
@extends('layouts.app')

@section('title', 'Главная')

@section('content')
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <!-- Всего автоматов -->
        <div class="bg-white rounded-xl shadow-sm p-6">
            <div class="flex items-center">
                <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
                    <i class="fas fa-cash-register text-blue-600 text-xl"></i>
                </div>
                <div class="ml-4">
                    <p class="text-sm text-gray-500">Всего автоматов</p>
                    <p class="text-2xl font-bold text-gray-800">{{ $stats['total_machines'] }}</p>
                </div>
            </div>
        </div>

        <!-- Работающие -->
        <div class="bg-white rounded-xl shadow-sm p-6">
            <div class="flex items-center">
                <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
                    <i class="fas fa-check-circle text-green-600 text-xl"></i>
                </div>
                <div class="ml-4">
                    <p class="text-sm text-gray-500">Работающие</p>
                    <p class="text-2xl font-bold text-gray-800">{{ $stats['working_machines'] }}</p>
                </div>
            </div>
        </div>

        <!-- Не работающие -->
        <div class="bg-white rounded-xl shadow-sm p-6">
            <div class="flex items-center">
                <div class="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center">
                    <i class="fas fa-times-circle text-red-600 text-xl"></i>
                </div>
                <div class="ml-4">
                    <p class="text-sm text-gray-500">Не работающие</p>
                    <p class="text-2xl font-bold text-gray-800">{{ $stats['not_working_machines'] }}</p>
                </div>
            </div>
        </div>

        <!-- Общий доход -->
        <div class="bg-white rounded-xl shadow-sm p-6">
            <div class="flex items-center">
                <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
                    <i class="fas fa-ruble-sign text-purple-600 text-xl"></i>
                </div>
                <div class="ml-4">
                    <p class="text-sm text-gray-500">Общий доход</p>
                    <p class="text-2xl font-bold text-gray-800">{{ number_format($stats['total_income'], 0, ',', ' ') }} ₽</p>
                </div>
            </div>
        </div>
    </div>
@endsection
```

### Список торговых автоматов:

**Файл** `resources/views/vending-machines/index.blade.php`:

```html
@extends('layouts.app')

@section('title', 'Торговые автоматы')

@section('content')
    <div class="bg-white rounded-xl shadow-sm">
        <!-- Header -->
        <div class="p-6 border-b border-gray-100 flex justify-between items-center">
            <h2 class="text-lg font-semibold text-gray-800">Список автоматов</h2>
            <div class="flex gap-4">
                <form method="GET" action="{{ route('vending-machines.index') }}" class="flex gap-2">
                    <input type="text" name="search" value="{{ $search }}" placeholder="Поиск..."
                           class="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                    <button type="submit" class="px-4 py-2 bg-gray-100 hover:bg-gray-200 rounded-lg">
                        <i class="fas fa-search"></i>
                    </button>
                </form>
                <a href="{{ route('vending-machines.create') }}"
                   class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                    <i class="fas fa-plus mr-2"></i>Добавить
                </a>
                <a href="{{ route('vending-machines.export') }}"
                   class="px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg">
                    <i class="fas fa-download mr-2"></i>Экспорт
                </a>
            </div>
        </div>

        <!-- Table -->
        <div class="overflow-x-auto">
            <table class="w-full">
                <thead class="bg-gray-50">
                    <tr>
                        <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Название</th>
                        <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Модель</th>
                        <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Статус</th>
                        <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Локация</th>
                        <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Действия</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-gray-100">
                    @forelse($machines as $machine)
                        <tr class="hover:bg-gray-50">
                            <td class="px-6 py-4 text-sm text-gray-900">{{ $machine['name'] }}</td>
                            <td class="px-6 py-4 text-sm text-gray-600">{{ $machine['model'] }}</td>
                            <td class="px-6 py-4">
                                <span class="px-2 py-1 text-xs rounded-full
                                    {{ $machine['status'] === 'Working' ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700' }}">
                                    {{ $machine['status'] }}
                                </span>
                            </td>
                            <td class="px-6 py-4 text-sm text-gray-600">{{ $machine['location'] }}</td>
                            <td class="px-6 py-4">
                                <div class="flex gap-2">
                                    <a href="{{ route('vending-machines.edit', $machine['id']) }}"
                                       class="text-blue-600 hover:text-blue-800">
                                        <i class="fas fa-edit"></i>
                                    </a>
                                    <form method="POST" action="{{ route('vending-machines.destroy', $machine['id']) }}"
                                          onsubmit="return confirm('Удалить автомат?')">
                                        @csrf
                                        @method('DELETE')
                                        <button type="submit" class="text-red-600 hover:text-red-800">
                                            <i class="fas fa-trash"></i>
                                        </button>
                                    </form>
                                </div>
                            </td>
                        </tr>
                    @empty
                        <tr>
                            <td colspan="5" class="px-6 py-8 text-center text-gray-500">
                                Автоматы не найдены
                            </td>
                        </tr>
                    @endforelse
                </tbody>
            </table>
        </div>

        <!-- Pagination -->
        <div class="p-6 border-t border-gray-100">
            {{ $machines->links() }}
        </div>
    </div>
@endsection
```

### Создание дополнительных шаблонов:

**Файл** `resources/views/vending-machines/create.blade.php`:

```html
@extends('layouts.app')

@section('title', 'Добавить автомат')

@section('content')
    <div class="max-w-2xl mx-auto">
        <div class="bg-white rounded-xl shadow-sm p-6">
            <h2 class="text-lg font-semibold text-gray-800 mb-6">Новый торговый автомат</h2>

            <form method="POST" action="{{ route('vending-machines.store') }}">
                @csrf

                <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Серийный номер</label>
                    <input type="text" name="serial_number" value="{{ old('serial_number') }}" required
                           class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                </div>

                <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Название</label>
                    <input type="text" name="name" value="{{ old('name') }}" required
                           class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                </div>

                <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Модель</label>
                    <input type="text" name="model" value="{{ old('model') }}" required
                           class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                </div>

                <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Локация</label>
                    <input type="text" name="location" value="{{ old('location') }}" required
                           class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                </div>

                <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Статус</label>
                    <select name="status" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                        <option value="Working">Работает</option>
                        <option value="NotWorking">Не работает</option>
                        <option value="Maintenance">На обслуживании</option>
                    </select>
                </div>

                <div class="flex gap-4">
                    <button type="submit" class="px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                        Создать
                    </button>
                    <a href="{{ route('vending-machines.index') }}" class="px-6 py-2 bg-gray-200 hover:bg-gray-300 text-gray-700 rounded-lg">
                        Отмена
                    </a>
                </div>
            </form>
        </div>
    </div>
@endsection
```

**Файл** `resources/views/vending-machines/edit.blade.php`:

```html
@extends('layouts.app')

@section('title', 'Редактировать автомат')

@section('content')
    <div class="max-w-2xl mx-auto">
        <div class="bg-white rounded-xl shadow-sm p-6">
            <h2 class="text-lg font-semibold text-gray-800 mb-6">Редактирование автомата</h2>

            <form method="POST" action="{{ route('vending-machines.update', $machine['id']) }}">
                @csrf
                @method('PUT')

                <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Название</label>
                    <input type="text" name="name" value="{{ old('name', $machine['name']) }}" required
                           class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                </div>

                <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Модель</label>
                    <input type="text" name="model" value="{{ old('model', $machine['model']) }}" required
                           class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                </div>

                <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Локация</label>
                    <input type="text" name="location" value="{{ old('location', $machine['location']) }}" required
                           class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                </div>

                <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Статус</label>
                    <select name="status" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                        <option value="Working" {{ $machine['status'] === 'Working' ? 'selected' : '' }}>Работает</option>
                        <option value="NotWorking" {{ $machine['status'] === 'NotWorking' ? 'selected' : '' }}>Не работает</option>
                        <option value="Maintenance" {{ $machine['status'] === 'Maintenance' ? 'selected' : '' }}>На обслуживании</option>
                    </select>
                </div>

                <div class="flex gap-4">
                    <button type="submit" class="px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                        Сохранить
                    </button>
                    <a href="{{ route('vending-machines.index') }}" class="px-6 py-2 bg-gray-200 hover:bg-gray-300 text-gray-700 rounded-lg">
                        Отмена
                    </a>
                </div>
            </form>
        </div>
    </div>
@endsection
```

**Файл** `resources/views/contracts/index.blade.php`:

```html
@extends('layouts.app')

@section('title', 'Договоры')

@section('content')
    <div class="bg-white rounded-xl shadow-sm p-6">
        <h2 class="text-lg font-semibold text-gray-800 mb-4">Договоры</h2>
        <p class="text-gray-500">Страница в разработке</p>
    </div>
@endsection
```

---

## 14. Запуск приложения

### Соберите фронтенд:

```bash
npm run build
```

### Запустите сервер разработки:

```bash
# Вариант 1: Только Laravel сервер
php artisan serve

# Вариант 2: Laravel + Vite (для hot-reload)
# В первом терминале:
php artisan serve

# Во втором терминале:
npm run dev
```

### Откройте приложение:

Перейдите по адресу: http://localhost:8000

---

## 15. Структура готового проекта

```
VendingMachineWeb/
├── app/
│   ├── Http/
│   │   ├── Controllers/
│   │   │   ├── AuthController.php
│   │   │   ├── ContractController.php
│   │   │   ├── DashboardController.php
│   │   │   └── VendingMachineController.php
│   │   └── Middleware/
│   │       ├── ApiAuth.php
│   │       └── SetLocale.php
│   ├── Models/
│   │   ├── Modem.php
│   │   ├── User.php
│   │   └── VendingMachine.php
│   └── Services/
│       └── VendingApiService.php
├── bootstrap/
│   └── app.php
├── config/
│   └── services.php
├── database/
│   ├── migrations/
│   │   ├── create_vending_machines_table.php
│   │   └── create_modems_table.php
│   └── database.sqlite
├── resources/
│   ├── css/
│   │   └── app.css
│   ├── js/
│   │   ├── app.js
│   │   └── bootstrap.js
│   └── views/
│       ├── auth/
│       │   ├── login.blade.php
│       │   └── register.blade.php
│       ├── contracts/
│       │   └── index.blade.php
│       ├── layouts/
│       │   └── app.blade.php
│       ├── vending-machines/
│       │   ├── create.blade.php
│       │   ├── edit.blade.php
│       │   └── index.blade.php
│       └── dashboard.blade.php
├── routes/
│   └── web.php
├── .env
├── composer.json
├── package.json
└── vite.config.js
```

---

## Дополнительные команды

### Очистка кешей:

```bash
php artisan cache:clear
php artisan config:clear
php artisan view:clear
php artisan route:clear
```

### Запуск тестов:

```bash
php artisan test
```

### Запуск миграций заново:

```bash
php artisan migrate:fresh
```

---

## Технологический стек

| Технология | Версия | Назначение |
|------------|--------|------------|
| Laravel | 12.x | Backend framework |
| PHP | 8.2+ | Серверный язык |
| Tailwind CSS | 4.x | CSS framework |
| Alpine.js | 3.x | JavaScript framework |
| Vite | 7.x | Сборщик фронтенда |
| SQLite | - | База данных (dev) |
| Font Awesome | 6.x | Иконки |

---

## Полезные ссылки

- [Laravel Documentation](https://laravel.com/docs)
- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
- [Alpine.js Documentation](https://alpinejs.dev/docs)
- [Vite Documentation](https://vite.dev/guide)
- [Composer](https://getcomposer.org/)
- [Node.js](https://nodejs.org/)
