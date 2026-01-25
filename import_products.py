#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
===============================================================================
СКРИПТ ИМПОРТА ПРОДУКТОВ В БАЗУ ДАННЫХ
===============================================================================

Назначение:
    Импортирует данные о продуктах (товарах) из JSON файла в PostgreSQL базу данных.
    Используется для первоначальной загрузки каталога товаров торговых автоматов.

Источник данных:
    - Файл: products.json (в той же директории)
    - Формат: JSON массив объектов с полями:
      id, name, price, min_stock, vending_machine_id, description,
      quantity_available, sales_trend

Целевая таблица:
    - Таблица: products
    - База данных: PostgreSQL (параметры в db_params)

Особенности:
    - Использует UPSERT (INSERT ... ON CONFLICT DO UPDATE)
    - При повторном запуске обновляет существующие записи
    - Выводит подробный лог импорта с символами ✓/✗

Требования:
    - Python 3.x
    - Библиотека psycopg2 (pip install psycopg2-binary)
    - Доступ к PostgreSQL серверу

Запуск:
    python import_products.py

Автор: Система управления торговыми автоматами
===============================================================================
"""

import json          # Для парсинга JSON файлов
import psycopg2      # type: ignore # PostgreSQL драйвер для Python
from pathlib import Path  # Для работы с путями файлов (кроссплатформенно)
import sys           # Для sys.exit() при критических ошибках


def import_products():
    """
    Основная функция импорта продуктов.

    Алгоритм работы:
    1. Подключается к PostgreSQL базе данных
    2. Читает JSON файл с продуктами
    3. Для каждого продукта выполняет INSERT с ON CONFLICT
    4. Коммитит транзакцию при успехе
    5. Выводит статистику импорта

    Raises:
        SystemExit: При критических ошибках (БД недоступна, файл не найден)
    """

    # =========================================================================
    # ПАРАМЕТРЫ ПОДКЛЮЧЕНИЯ К БАЗЕ ДАННЫХ
    # =========================================================================
    # ВАЖНО: В production эти данные должны быть в переменных окружения!
    # Пример: os.environ.get('DB_NAME', 'default_db')
    db_params = {
        'dbname': '1133',           # Название базы данных (числовое имя - особенность проекта)
        'user': 'postgres',       # Пользователь PostgreSQL
        'password': 'root',       # Пароль (НЕ использовать в production!)
        'host': 'localhost',      # Адрес сервера БД
        'port': '5432'            # Стандартный порт PostgreSQL
    }

    try:
        # Подключение к базе данных
        print("Подключение к базе данных...")
        conn = psycopg2.connect(**db_params)
        cur = conn.cursor()
        print("✓ Подключение успешно")

        # Проверка наличия файла
        json_file = Path('products.json')
        if not json_file.exists():
            print(f"✗ Файл '{json_file}' не найден!")
            return

        # Чтение JSON файла
        print(f"\nЧтение файла {json_file}...")
        with open(json_file, 'r', encoding='utf-8') as f:
            products = json.load(f)

        if not isinstance(products, list):
            print("✗ Некорректный формат JSON (ожидается массив)")
            return

        print(f"✓ Найдено {len(products)} продуктов для импорта")

        # =====================================================================
        # SQL ЗАПРОС ДЛЯ UPSERT (INSERT или UPDATE)
        # =====================================================================
        # Используется PostgreSQL синтаксис ON CONFLICT для реализации UPSERT:
        # - Если записи с таким id нет → выполняется INSERT
        # - Если запись с таким id есть → выполняется UPDATE
        #
        # EXCLUDED - специальная таблица PostgreSQL, содержащая значения,
        # которые пытались вставить (те, что вызвали конфликт)
        #
        # Параметры %(name)s - именованные placeholder'ы для psycopg2,
        # которые будут заменены значениями из словаря product
        insert_query = """
            INSERT INTO products (
                id, name, price, min_stock, vending_machine_id,
                description, quantity_available, sales_trend
            ) VALUES (
                %(id)s, %(name)s, %(price)s, %(min_stock)s, %(vending_machine_id)s,
                %(description)s, %(quantity_available)s, %(sales_trend)s
            )
            ON CONFLICT (id) DO UPDATE SET
                name = EXCLUDED.name,
                price = EXCLUDED.price,
                min_stock = EXCLUDED.min_stock,
                vending_machine_id = EXCLUDED.vending_machine_id,
                description = EXCLUDED.description,
                quantity_available = EXCLUDED.quantity_available,
                sales_trend = EXCLUDED.sales_trend,
                updated_at = CURRENT_TIMESTAMP
        """

        # Импорт каждого продукта
        imported_count = 0
        errors_count = 0

        print("\nИмпорт продуктов:")
        print("-" * 60)

        for product in products:
            try:
                # Выполнение вставки
                cur.execute(insert_query, product)
                imported_count += 1
                print(f"✓ {product['name'][:50]:<50} {product['price']:>8.2f} ₽")

            except Exception as e:
                errors_count += 1
                print(f"✗ {product.get('name', 'Unknown')}: Ошибка - {str(e)}")

        # Подтверждение транзакции
        conn.commit()

    except psycopg2.Error as e:
        print(f"\n✗ Ошибка базы данных: {e}")
        sys.exit(1)

    except json.JSONDecodeError as e:
        print(f"\n✗ Ошибка чтения JSON: {e}")
        sys.exit(1)

    except Exception as e:
        print(f"\n✗ Неожиданная ошибка: {e}")
        sys.exit(1)

    finally:
        # Закрытие соединения
        if 'cur' in locals():
            cur.close()
        if 'conn' in locals():
            conn.close()
        print("\nСоединение закрыто")


if __name__ == '__main__':
    print("="*60)
    print("ИМПОРТ ДАННЫХ ПРОДУКТОВ")
    print("="*60)
    import_products()
