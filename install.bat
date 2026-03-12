@echo off
chcp 65001 >nul 2>&1
setlocal EnableDelayedExpansion

:: ============================================================
::  Chempionat - Автоматическая установка всех компонентов
::  VendingMachineDesktop | VendingMachineAPI | VendingMachineWeb
:: ============================================================

title Chempionat - Установка компонентов

set "SETUP_DIR=%USERPROFILE%\Desktop\ChempionatSetup"
set "PROJECT_DIR=%USERPROFILE%\Desktop\Chempionat"
set "LOG_FILE=%SETUP_DIR%\install_log.txt"
set "ERRORS=0"

:: --- Цвета и оформление ---
echo.
echo ================================================================
echo   Chempionat - Автоматическая установка компонентов
echo ================================================================
echo.
echo   Проект:     %PROJECT_DIR%
echo   Папка Setup: %SETUP_DIR%
echo.
echo ================================================================
echo.

:: ============================================================
::  ШАГ 0: Проверка прав администратора
:: ============================================================
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [ОШИБКА] Запустите этот файл от имени администратора!
    echo          ПКМ -^> "Запуск от имени администратора"
    echo.
    pause
    exit /b 1
)
echo [OK] Запущено от имени администратора
echo.

:: ============================================================
::  ШАГ 1: Создание пустой папки для установки
:: ============================================================
echo [ШАГ 1/8] Создание папки установки...
echo.

if exist "%SETUP_DIR%" (
    echo   Папка %SETUP_DIR% уже существует, очищаем...
    rd /s /q "%SETUP_DIR%" >nul 2>&1
)
mkdir "%SETUP_DIR%"
mkdir "%SETUP_DIR%\downloads"
mkdir "%SETUP_DIR%\logs"

echo [SETUP] Начало установки: %date% %time% > "%LOG_FILE%"
echo [OK] Папка создана: %SETUP_DIR%
echo.

:: ============================================================
::  ШАГ 2: Проверка наличия winget
:: ============================================================
echo [ШАГ 2/8] Проверка winget (Windows Package Manager)...
echo.

where winget >nul 2>&1
if %errorlevel% neq 0 (
    echo [ПРЕДУПРЕЖДЕНИЕ] winget не найден.
    echo   winget необходим для автоматической установки компонентов.
    echo   Обновите "App Installer" через Microsoft Store.
    echo   https://aka.ms/getwinget
    echo.
    echo   [winget] Не найден >> "%LOG_FILE%"
    set "HAS_WINGET=0"
) else (
    echo [OK] winget найден
    echo   [winget] Найден >> "%LOG_FILE%"
    set "HAS_WINGET=1"
)
echo.

:: ============================================================
::  ШАГ 3: Проверка и установка SDK компонентов
:: ============================================================
echo [ШАГ 3/8] Проверка и установка SDK компонентов...
echo ================================================================
echo.

:: --- 3.1 .NET 8 SDK ---
echo   [3.1] .NET 8 SDK ...
dotnet --list-sdks 2>nul | findstr /C:"8." >nul 2>&1
if %errorlevel% equ 0 (
    for /f "tokens=1" %%v in ('dotnet --version 2^>nul') do set "DOTNET_VER=%%v"
    echo         [OK] .NET SDK найден: !DOTNET_VER!
    echo   [.NET SDK] Найден: !DOTNET_VER! >> "%LOG_FILE%"
) else (
    echo         [НЕ НАЙДЕН] Устанавливаю .NET 8 SDK...
    echo   [.NET SDK] Не найден, устанавливаю... >> "%LOG_FILE%"
    if "!HAS_WINGET!"=="1" (
        winget install Microsoft.DotNet.SDK.8 --accept-package-agreements --accept-source-agreements --silent >> "%LOG_FILE%" 2>&1
        if !errorlevel! equ 0 (
            echo         [OK] .NET 8 SDK установлен
            echo   [.NET SDK] Установлен через winget >> "%LOG_FILE%"
        ) else (
            echo         [ОШИБКА] Не удалось установить .NET 8 SDK
            echo   [.NET SDK] ОШИБКА установки >> "%LOG_FILE%"
            set /a ERRORS+=1
        )
    ) else (
        echo         [ОШИБКА] winget не доступен. Установите .NET 8 SDK вручную:
        echo         https://dotnet.microsoft.com/download/dotnet/8.0
        set /a ERRORS+=1
    )
)
echo.

:: --- 3.2 Python ---
echo   [3.2] Python ...
where python >nul 2>&1
if %errorlevel% equ 0 (
    for /f "tokens=*" %%v in ('python --version 2^>nul') do set "PYTHON_VER=%%v"
    echo         [OK] !PYTHON_VER! найден
    echo   [Python] Найден: !PYTHON_VER! >> "%LOG_FILE%"
) else (
    echo         [НЕ НАЙДЕН] Устанавливаю Python...
    echo   [Python] Не найден, устанавливаю... >> "%LOG_FILE%"
    if "!HAS_WINGET!"=="1" (
        winget install Python.Python.3.12 --accept-package-agreements --accept-source-agreements --silent >> "%LOG_FILE%" 2>&1
        if !errorlevel! equ 0 (
            echo         [OK] Python установлен
            echo   [Python] Установлен через winget >> "%LOG_FILE%"
            :: Обновляем PATH для текущей сессии
            set "PATH=%PATH%;%LOCALAPPDATA%\Programs\Python\Python312;%LOCALAPPDATA%\Programs\Python\Python312\Scripts"
        ) else (
            echo         [ОШИБКА] Не удалось установить Python
            echo   [Python] ОШИБКА установки >> "%LOG_FILE%"
            set /a ERRORS+=1
        )
    ) else (
        echo         [ОШИБКА] winget не доступен. Установите Python вручную:
        echo         https://www.python.org/downloads/
        set /a ERRORS+=1
    )
)
echo.

:: --- 3.3 Node.js ---
echo   [3.3] Node.js ...
where node >nul 2>&1
if %errorlevel% equ 0 (
    for /f "tokens=*" %%v in ('node --version 2^>nul') do set "NODE_VER=%%v"
    echo         [OK] Node.js !NODE_VER! найден
    echo   [Node.js] Найден: !NODE_VER! >> "%LOG_FILE%"
) else (
    echo         [НЕ НАЙДЕН] Устанавливаю Node.js LTS...
    echo   [Node.js] Не найден, устанавливаю... >> "%LOG_FILE%"
    if "!HAS_WINGET!"=="1" (
        winget install OpenJS.NodeJS.LTS --accept-package-agreements --accept-source-agreements --silent >> "%LOG_FILE%" 2>&1
        if !errorlevel! equ 0 (
            echo         [OK] Node.js установлен
            echo   [Node.js] Установлен через winget >> "%LOG_FILE%"
        ) else (
            echo         [ОШИБКА] Не удалось установить Node.js
            echo   [Node.js] ОШИБКА установки >> "%LOG_FILE%"
            set /a ERRORS+=1
        )
    ) else (
        echo         [ОШИБКА] winget не доступен. Установите Node.js вручную:
        echo         https://nodejs.org/
        set /a ERRORS+=1
    )
)
echo.

:: --- 3.4 PHP ---
echo   [3.4] PHP ...
where php >nul 2>&1
if %errorlevel% equ 0 (
    for /f "tokens=*" %%v in ('php --version 2^>nul ^| findstr /N "^" ^| findstr "^1:"') do set "PHP_VER=%%v"
    echo         [OK] PHP найден
    echo   [PHP] Найден >> "%LOG_FILE%"
) else (
    echo         [НЕ НАЙДЕН] Устанавливаю PHP...
    echo   [PHP] Не найден, устанавливаю... >> "%LOG_FILE%"
    if "!HAS_WINGET!"=="1" (
        winget install PHP.PHP.8.3 --accept-package-agreements --accept-source-agreements --silent >> "%LOG_FILE%" 2>&1
        if !errorlevel! equ 0 (
            echo         [OK] PHP установлен
            echo   [PHP] Установлен через winget >> "%LOG_FILE%"
        ) else (
            echo         [ПРЕДУПРЕЖДЕНИЕ] winget не смог установить PHP.
            echo         Пробую альтернативный ID...
            winget install PHP.PHP --accept-package-agreements --accept-source-agreements --silent >> "%LOG_FILE%" 2>&1
            if !errorlevel! equ 0 (
                echo         [OK] PHP установлен
                echo   [PHP] Установлен через winget (альт.) >> "%LOG_FILE%"
            ) else (
                echo         [ОШИБКА] Установите PHP вручную: https://windows.php.net/download/
                echo   [PHP] ОШИБКА установки >> "%LOG_FILE%"
                set /a ERRORS+=1
            )
        )
    ) else (
        echo         [ОШИБКА] winget не доступен. Установите PHP вручную:
        echo         https://windows.php.net/download/
        set /a ERRORS+=1
    )
)
echo.

:: --- 3.5 Composer ---
echo   [3.5] Composer ...
where composer >nul 2>&1
if %errorlevel% equ 0 (
    echo         [OK] Composer найден
    echo   [Composer] Найден >> "%LOG_FILE%"
) else (
    echo         [НЕ НАЙДЕН] Устанавливаю Composer...
    echo   [Composer] Не найден, устанавливаю... >> "%LOG_FILE%"
    if "!HAS_WINGET!"=="1" (
        winget install Composer.Composer --accept-package-agreements --accept-source-agreements --silent >> "%LOG_FILE%" 2>&1
        if !errorlevel! equ 0 (
            echo         [OK] Composer установлен
            echo   [Composer] Установлен через winget >> "%LOG_FILE%"
        ) else (
            echo         [ОШИБКА] Не удалось установить Composer
            echo   [Composer] ОШИБКА установки >> "%LOG_FILE%"
            set /a ERRORS+=1
        )
    ) else (
        echo         [ОШИБКА] winget не доступен. Установите Composer вручную:
        echo         https://getcomposer.org/download/
        set /a ERRORS+=1
    )
)
echo.

:: --- 3.6 PostgreSQL ---
echo   [3.6] PostgreSQL ...
where psql >nul 2>&1
if %errorlevel% equ 0 (
    echo         [OK] PostgreSQL найден
    echo   [PostgreSQL] Найден >> "%LOG_FILE%"
) else (
    :: Проверим типичные пути установки PostgreSQL
    set "PG_FOUND=0"
    if exist "C:\Program Files\PostgreSQL" set "PG_FOUND=1"
    if exist "C:\Program Files (x86)\PostgreSQL" set "PG_FOUND=1"

    if "!PG_FOUND!"=="1" (
        echo         [OK] PostgreSQL обнаружен (не в PATH)
        echo         Рекомендуется добавить PostgreSQL\bin в PATH
        echo   [PostgreSQL] Найден (не в PATH) >> "%LOG_FILE%"
    ) else (
        echo         [НЕ НАЙДЕН] Устанавливаю PostgreSQL 16...
        echo   [PostgreSQL] Не найден, устанавливаю... >> "%LOG_FILE%"
        if "!HAS_WINGET!"=="1" (
            winget install PostgreSQL.PostgreSQL.16 --accept-package-agreements --accept-source-agreements --silent >> "%LOG_FILE%" 2>&1
            if !errorlevel! equ 0 (
                echo         [OK] PostgreSQL 16 установлен
                echo         Пароль по умолчанию: задан при установке
                echo   [PostgreSQL] Установлен через winget >> "%LOG_FILE%"
            ) else (
                echo         [ОШИБКА] Не удалось установить PostgreSQL
                echo         Скачайте вручную: https://www.postgresql.org/download/windows/
                echo   [PostgreSQL] ОШИБКА установки >> "%LOG_FILE%"
                set /a ERRORS+=1
            )
        ) else (
            echo         [ОШИБКА] winget не доступен. Установите PostgreSQL вручную:
            echo         https://www.postgresql.org/download/windows/
            set /a ERRORS+=1
        )
    )
)
echo.

:: Обновляем переменные среды после возможных установок
echo   Обновляю переменные среды...
call refreshenv >nul 2>&1

echo.
echo ================================================================
echo.

:: ============================================================
::  ШАГ 4: Python пакеты (psycopg2, bcrypt)
:: ============================================================
echo [ШАГ 4/8] Установка Python пакетов...
echo.

where python >nul 2>&1
if %errorlevel% equ 0 (
    echo   Устанавливаю psycopg2-binary...
    python -m pip install psycopg2-binary --quiet >> "%LOG_FILE%" 2>&1
    if !errorlevel! equ 0 (
        echo   [OK] psycopg2-binary установлен
        echo   [pip] psycopg2-binary установлен >> "%LOG_FILE%"
    ) else (
        echo   [ПРЕДУПРЕЖДЕНИЕ] psycopg2-binary не установился, пробую psycopg2...
        python -m pip install psycopg2 --quiet >> "%LOG_FILE%" 2>&1
        if !errorlevel! equ 0 (
            echo   [OK] psycopg2 установлен
            echo   [pip] psycopg2 установлен >> "%LOG_FILE%"
        ) else (
            echo   [ОШИБКА] Не удалось установить psycopg2
            echo   [pip] psycopg2 ОШИБКА >> "%LOG_FILE%"
            set /a ERRORS+=1
        )
    )

    echo   Устанавливаю bcrypt...
    python -m pip install bcrypt --quiet >> "%LOG_FILE%" 2>&1
    if !errorlevel! equ 0 (
        echo   [OK] bcrypt установлен
        echo   [pip] bcrypt установлен >> "%LOG_FILE%"
    ) else (
        echo   [ОШИБКА] Не удалось установить bcrypt
        echo   [pip] bcrypt ОШИБКА >> "%LOG_FILE%"
        set /a ERRORS+=1
    )
) else (
    echo   [ОШИБКА] Python не найден в PATH. Перезапустите скрипт после установки Python.
    echo   [pip] Python не найден, пропускаю >> "%LOG_FILE%"
    set /a ERRORS+=1
)
echo.

:: ============================================================
::  ШАГ 5: VendingMachineAPI - NuGet пакеты
:: ============================================================
echo [ШАГ 5/8] VendingMachineAPI - восстановление NuGet пакетов...
echo.

where dotnet >nul 2>&1
if %errorlevel% equ 0 (
    if exist "%PROJECT_DIR%\VendingMachineAPI\VendingMachineAPI.csproj" (
        echo   Выполняю dotnet restore...
        dotnet restore "%PROJECT_DIR%\VendingMachineAPI\VendingMachineAPI.csproj" >> "%LOG_FILE%" 2>&1
        if !errorlevel! equ 0 (
            echo   [OK] NuGet пакеты VendingMachineAPI восстановлены:
            echo        - Npgsql.EntityFrameworkCore.PostgreSQL 8.0.0
            echo        - Microsoft.AspNetCore.Authentication.JwtBearer 8.0.0
            echo        - BCrypt.Net-Next 4.0.3
            echo        - AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1
            echo        - Swashbuckle.AspNetCore 6.5.0
            echo        - Serilog.AspNetCore 8.0.0
            echo        - FluentValidation.AspNetCore 11.3.0
            echo        - Microsoft.EntityFrameworkCore.Design 8.0.0
            echo   [NuGet] VendingMachineAPI восстановлен >> "%LOG_FILE%"
        ) else (
            echo   [ОШИБКА] dotnet restore для VendingMachineAPI не удался
            echo   [NuGet] VendingMachineAPI ОШИБКА >> "%LOG_FILE%"
            set /a ERRORS+=1
        )
    ) else (
        echo   [ОШИБКА] Файл проекта VendingMachineAPI.csproj не найден
        echo   [NuGet] VendingMachineAPI.csproj не найден >> "%LOG_FILE%"
        set /a ERRORS+=1
    )
) else (
    echo   [ОШИБКА] dotnet CLI не найден. Перезапустите скрипт после установки .NET SDK.
    echo   [NuGet] dotnet не найден >> "%LOG_FILE%"
    set /a ERRORS+=1
)
echo.

:: ============================================================
::  ШАГ 6: VendingMachineDesktop - NuGet пакеты
:: ============================================================
echo [ШАГ 6/8] VendingMachineDesktop - восстановление NuGet пакетов...
echo.

where dotnet >nul 2>&1
if %errorlevel% equ 0 (
    if exist "%PROJECT_DIR%\VendingMachineDesktop\VendingMachineDesktop.csproj" (
        echo   Выполняю dotnet restore...
        dotnet restore "%PROJECT_DIR%\VendingMachineDesktop\VendingMachineDesktop.csproj" >> "%LOG_FILE%" 2>&1
        if !errorlevel! equ 0 (
            echo   [OK] NuGet пакеты VendingMachineDesktop восстановлены:
            echo        - CommunityToolkit.Mvvm 8.4.0
            echo        - CsvHelper 33.1.0
            echo        - LiveChartsCore.SkiaSharpView.WPF 2.0.0-rc2.2
            echo        - MahApps.Metro 2.4.11
            echo        - MaterialDesignThemes 5.3.0
            echo        - MaterialDesignColors 5.3.0
            echo        - Npgsql 8.0.5
            echo        - BCrypt.Net-Next 4.0.3
            echo        - Serilog.Sinks.File 7.0.0
            echo        - Microsoft.Extensions.Configuration.Json 8.0.0
            echo   [NuGet] VendingMachineDesktop восстановлен >> "%LOG_FILE%"
        ) else (
            echo   [ОШИБКА] dotnet restore для VendingMachineDesktop не удался
            echo   [NuGet] VendingMachineDesktop ОШИБКА >> "%LOG_FILE%"
            set /a ERRORS+=1
        )
    ) else (
        echo   [ОШИБКА] Файл проекта VendingMachineDesktop.csproj не найден
        echo   [NuGet] VendingMachineDesktop.csproj не найден >> "%LOG_FILE%"
        set /a ERRORS+=1
    )
) else (
    echo   [ОШИБКА] dotnet CLI не найден.
    set /a ERRORS+=1
)
echo.

:: ============================================================
::  ШАГ 7: VendingMachineWeb - Composer + NPM пакеты
:: ============================================================
echo [ШАГ 7/8] VendingMachineWeb - установка Composer и NPM пакетов...
echo.

if exist "%PROJECT_DIR%\VendingMachineWeb\composer.json" (
    :: --- Composer install ---
    where composer >nul 2>&1
    if %errorlevel% equ 0 (
        echo   Выполняю composer install...
        pushd "%PROJECT_DIR%\VendingMachineWeb"

        :: Копируем .env если нет
        if not exist ".env" (
            if exist ".env.example" (
                copy ".env.example" ".env" >nul 2>&1
                echo   [OK] .env скопирован из .env.example
            )
        )

        composer install --no-interaction >> "%LOG_FILE%" 2>&1
        if !errorlevel! equ 0 (
            echo   [OK] Composer пакеты установлены:
            echo        - laravel/framework ^12.0
            echo        - laravel/tinker ^2.10.1
            echo   [Composer] VendingMachineWeb установлен >> "%LOG_FILE%"

            :: Генерация ключа приложения
            where php >nul 2>&1
            if !errorlevel! equ 0 (
                php artisan key:generate --force >> "%LOG_FILE%" 2>&1
                echo   [OK] APP_KEY сгенерирован
            )
        ) else (
            echo   [ОШИБКА] composer install не удался
            echo   [Composer] VendingMachineWeb ОШИБКА >> "%LOG_FILE%"
            set /a ERRORS+=1
        )
        popd
    ) else (
        echo   [ОШИБКА] Composer не найден в PATH
        set /a ERRORS+=1
    )

    :: --- NPM install ---
    where npm >nul 2>&1
    if %errorlevel% equ 0 (
        echo.
        echo   Выполняю npm install...
        pushd "%PROJECT_DIR%\VendingMachineWeb"
        npm install >> "%LOG_FILE%" 2>&1
        if !errorlevel! equ 0 (
            echo   [OK] NPM пакеты установлены:
            echo        - tailwindcss ^4.0.0
            echo        - vite ^7.0.7
            echo        - axios ^1.11.0
            echo        - laravel-vite-plugin ^2.0.0
            echo   [NPM] VendingMachineWeb установлен >> "%LOG_FILE%"
        ) else (
            echo   [ОШИБКА] npm install не удался
            echo   [NPM] VendingMachineWeb ОШИБКА >> "%LOG_FILE%"
            set /a ERRORS+=1
        )
        popd
    ) else (
        echo   [ОШИБКА] npm не найден в PATH
        set /a ERRORS+=1
    )
) else (
    echo   [ПРЕДУПРЕЖДЕНИЕ] VendingMachineWeb\composer.json не найден
    echo   Пропускаю установку Laravel зависимостей.
    echo   [Web] composer.json не найден >> "%LOG_FILE%"
)
echo.

:: ============================================================
::  ШАГ 8: Сводка установки
:: ============================================================
echo [ШАГ 8/8] Формирование отчёта...
echo.
echo ================================================================
echo                     РЕЗУЛЬТАТ УСТАНОВКИ
echo ================================================================
echo.

:: Проверяем итоговое состояние
echo   Компонент                 Статус
echo   -------------------------------------------------

where dotnet >nul 2>&1
if %errorlevel% equ 0 (
    echo   .NET SDK                  [УСТАНОВЛЕН]
) else (
    echo   .NET SDK                  [НЕ НАЙДЕН]
)

where python >nul 2>&1
if %errorlevel% equ 0 (
    echo   Python                    [УСТАНОВЛЕН]
) else (
    echo   Python                    [НЕ НАЙДЕН]
)

where node >nul 2>&1
if %errorlevel% equ 0 (
    echo   Node.js                   [УСТАНОВЛЕН]
) else (
    echo   Node.js                   [НЕ НАЙДЕН]
)

where php >nul 2>&1
if %errorlevel% equ 0 (
    echo   PHP                       [УСТАНОВЛЕН]
) else (
    echo   PHP                       [НЕ НАЙДЕН]
)

where composer >nul 2>&1
if %errorlevel% equ 0 (
    echo   Composer                  [УСТАНОВЛЕН]
) else (
    echo   Composer                  [НЕ НАЙДЕН]
)

where psql >nul 2>&1
if %errorlevel% equ 0 (
    echo   PostgreSQL                [УСТАНОВЛЕН]
) else (
    if exist "C:\Program Files\PostgreSQL" (
        echo   PostgreSQL                [УСТАНОВЛЕН]
    ) else (
        echo   PostgreSQL                [НЕ НАЙДЕН]
    )
)

echo.
echo   -------------------------------------------------
echo   Python: psycopg2           pip install psycopg2-binary
echo   Python: bcrypt              pip install bcrypt
echo   -------------------------------------------------
echo   VendingMachineAPI          dotnet restore
echo   VendingMachineDesktop      dotnet restore
echo   VendingMachineWeb          composer install + npm install
echo   -------------------------------------------------
echo.

if %ERRORS% equ 0 (
    echo   [УСПЕХ] Все компоненты установлены без ошибок!
) else (
    echo   [ВНИМАНИЕ] Обнаружено ошибок: %ERRORS%
    echo   Проверьте лог-файл: %LOG_FILE%
    echo.
    echo   Если SDK были только что установлены, закройте это
    echo   окно и ПЕРЕЗАПУСТИТЕ скрипт - новые SDK появятся
    echo   в PATH только после перезапуска терминала.
)

echo.
echo   Лог установки: %LOG_FILE%
echo.
echo ================================================================
echo.

echo [SETUP] Завершено: %date% %time% | Ошибок: %ERRORS% >> "%LOG_FILE%"

pause
exit /b %ERRORS%
