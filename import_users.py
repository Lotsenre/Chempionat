import json          # Для парсинга JSON файлов 
import psycopg2
from pathlib import Path  # Для работы с путями и glob паттернами
import sys           # Для sys.exit() при критических ошибках


def import_users():
    db_params = {
        'dbname': 'A13',           # Название базы данных
        'user': 'postgres',       # Пользователь PostgreSQL
        'password': 'root',       # Пароль (изменить для production!)
        'host': 'localhost',      # Адрес сервера БД
        'port': '5432'            # Стандартный порт PostgreSQL
    }

    try:
        # Подключение к базе данных

        conn = psycopg2.connect(**db_params)
        cur = conn.cursor()

        # Получение списка JSON файлов
        users_dir = Path('users')
        if not users_dir.exists():
            return

        json_files = list(users_dir.glob('user_*.json'))
        if not json_files:
            return

        # SQL запрос для вставки данных
        insert_query = """
            INSERT INTO users (
                id, email, full_name, phone, role,
                is_manager, is_engineer, is_operator, image
            ) VALUES (
                %(id)s, %(email)s, %(full_name)s, %(phone)s, %(role)s,
                %(is_manager)s, %(is_engineer)s, %(is_operator)s, %(image)s
            )
            ON CONFLICT (id) DO UPDATE SET
                email = EXCLUDED.email,
                full_name = EXCLUDED.full_name,
                phone = EXCLUDED.phone,
                role = EXCLUDED.role,
                is_manager = EXCLUDED.is_manager,
                is_engineer = EXCLUDED.is_engineer,
                is_operator = EXCLUDED.is_operator,
                image = EXCLUDED.image,
                updated_at = CURRENT_TIMESTAMP
        """

        # Импорт каждого файла
        imported_count = 0
        errors_count = 0

        for json_file in sorted(json_files):
            try:
                with open(json_file, 'r', encoding='utf-8') as f:
                    data = json.load(f)

                # Выполнение вставки
                cur.execute(insert_query, data)
                imported_count += 1
                print(f"✓ {json_file.name}: {data['full_name']}")

            except Exception as e:
                errors_count += 1
                print(f"✗ {json_file.name}: Ошибка - {str(e)}")

        # Подтверждение транзакции
        conn.commit()


        # Проверка количества записей в таблице
        cur.execute("SELECT COUNT(*) FROM users")
        total = cur.fetchone()[0]
        print(f"\nВсего пользователей в базе данных: {total}")

    except psycopg2.Error as e:
        sys.exit(1)

    except Exception as e:
        sys.exit(1)

    finally:
        # Закрытие соединения
        if 'cur' in locals():
            cur.close()
        if 'conn' in locals():
            conn.close()


if __name__ == '__main__':
    import_users()
