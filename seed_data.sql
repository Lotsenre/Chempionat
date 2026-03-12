-- =============================================
-- MEGA SEED: Тонна данных для всех таблиц
-- =============================================

-- Массив ID автоматов
DO $$
DECLARE
    vm_ids uuid[] := ARRAY[
        '70f22c9f-de0a-41a9-9a53-cabf42688d47',
        '0a9a91bf-0fcc-44ac-b3c2-88a33411413a',
        '89c05e76-6b7e-4d72-b97c-62ecb96b1370',
        '53e2ec2e-d407-4e9a-a1d9-5a9501e81ede',
        'e1d62e6a-5427-4dda-8209-bb584bfbc9e5',
        '166c8a3f-a211-4d25-b2d4-a5b0b5f067d5',
        '4daae871-71ac-44da-8b58-dcf1a2ae907a',
        'a977e322-192c-4567-9f5f-36f504bfc13d',
        'dc92a945-80cf-4c86-8927-97026c6e82da',
        '5ae208bb-1e73-40fa-b56d-2c0470a01ad9'
    ];
    comp_ids uuid[] := ARRAY[
        'bc7719f1-a46f-4fd0-8cc8-73e78e4db98a',
        '234a5482-ee39-422d-acb3-8317ddf13122',
        '56651003-1af0-406a-bc7a-730d38ed406b',
        '8ee7726a-7a31-486a-a1ec-4425e9352ebc',
        '9fd364ea-7525-4261-bcd4-87b53018b1c8'
    ];
    admin_id uuid := '53b90a8b-3a5b-4c1f-966e-f56174583758';

    i int;
    j int;
    vm_id uuid;
    comp_id uuid;
    prod_id uuid;
    user_id uuid;

    product_names text[] := ARRAY['Кофе Американо','Кофе Латте','Кофе Капучино','Эспрессо','Горячий шоколад','Чай чёрный','Чай зелёный','Какао','Мокачино','Раф кофе','Кофе с молоком','Двойной эспрессо','Флэт Уайт','Кофе Карамель','Кофе Ваниль','Вода газированная','Кола','Спрайт','Фанта','Сок яблочный','Сок апельсиновый','Энергетик','Молоко','Кефир','Снэк шоколадный','Чипсы','Орехи','Печенье','Круассан','Сэндвич'];
    categories text[] := ARRAY['Кофе','Кофе','Кофе','Кофе','Горячие напитки','Чай','Чай','Горячие напитки','Кофе','Кофе','Кофе','Кофе','Кофе','Кофе','Кофе','Вода','Газировка','Газировка','Газировка','Соки','Соки','Энергетики','Молочные','Молочные','Снэки','Снэки','Снэки','Выпечка','Выпечка','Еда'];
    prices decimal[] := ARRAY[89,129,139,79,99,59,59,89,149,159,109,99,169,139,139,49,79,79,79,89,89,129,69,59,99,89,119,79,109,149];

    user_names text[] := ARRAY['Иванов Петр Сергеевич','Смирнова Анна Дмитриевна','Козлов Максим Андреевич','Новикова Елена Игоревна','Морозов Дмитрий Олегович','Волкова Мария Павловна','Соколов Артём Владимирович','Лебедева Ольга Николаевна','Кузнецов Сергей Михайлович','Попова Наталья Александровна','Фёдоров Алексей Юрьевич','Егорова Татьяна Валерьевна','Орлов Виктор Сергеевич','Макарова Ирина Олеговна','Зайцев Денис Романович','Белова Светлана Петровна','Тихонов Роман Дмитриевич','Крылова Дарья Андреевна','Гусев Николай Викторович','Калинина Юлия Сергеевна'];
    roles text[] := ARRAY['Admin','Engineer','Engineer','Franchisee','Engineer','Franchisee','Engineer','Admin','Engineer','Franchisee','Engineer','Franchisee','Engineer','Franchisee','Engineer','Franchisee','Engineer','Franchisee','Engineer','Franchisee'];

    modem_models text[] := ARRAY['Cinterion PLS8-E','Quectel EC25-E','SIMCom SIM7600E','Huawei ME909s','Sierra MC7455','Telit LE910C4-EU','u-blox SARA-R5','Nordic nRF9160','Sequans Monarch','Thales Cinterion EXS82'];

    payment_methods text[] := ARRAY['Наличные','Карта','Apple Pay','Google Pay','СБП'];
    work_descriptions text[] := ARRAY['Плановое ТО: замена фильтров, чистка','Замена кофемолки, калибровка','Ремонт монетоприёмника','Обновление ПО терминала','Замена уплотнителей, чистка бойлера','Диагностика платёжной системы','Замена насоса подачи воды','Профилактика холодильного модуля','Замена датчика уровня воды','Полная чистка и дезинфекция','Ремонт купюроприёмника','Замена подсветки дисплея','Обновление прошивки контроллера','Замена термостата','Калибровка дозатора'];
    issues text[] := ARRAY['Износ уплотнителей','Засор фильтра','Сбой ПО','Окисление контактов','Не обнаружены','Повышенный шум компрессора','Утечка воды','Не обнаружены','Износ подшипника','Перегрев платы','Не обнаружены','Замятие купюр','Не обнаружены','Нестабильное напряжение','Не обнаружены'];
    statuses text[] := ARRAY['Completed','Completed','Completed','Pending','Completed','Completed','Pending','Completed','Completed','Completed','Pending','Completed','Completed','Completed','Pending'];

    news_titles text[] := ARRAY[
        'Обновление системы мониторинга v3.2',
        'Новый партнёр: сеть АЗС "ТопливоПлюс"',
        'Итоги квартала: рост выручки на 23%',
        'Запуск программы лояльности для клиентов',
        'Техническое обслуживание серверов 15.01',
        'Новая линейка кофейных автоматов Necta',
        'Партнёрство с CoffeeLike: 50 новых точек',
        'Внедрение системы предиктивного обслуживания',
        'Открытие сервисного центра в Воронеже',
        'Результаты аудита безопасности',
        'Переход на новую платёжную систему',
        'Расширение ассортимента: энергетики и снэки',
        'Награда "Лучший вендинг-оператор 2025"',
        'Зимнее меню: глинтвейн и какао',
        'Оптимизация логистики: сокращение простоев на 40%',
        'Интеграция с 1С:Бухгалтерия',
        'Новые тарифы на обслуживание с марта',
        'Вебинар: эффективное управление вендинг-сетью',
        'Запуск мобильного приложения для инженеров',
        'Итоги года: 150 автоматов в сети'
    ];
    news_contents text[] := ARRAY[
        'Выпущено крупное обновление системы мониторинга. Добавлены: real-time уведомления, улучшенные графики, экспорт в PDF.',
        'Подписан договор на установку 30 автоматов на АЗС сети "ТопливоПлюс" в Московской области.',
        'По итогам Q3 2025 общая выручка сети выросла на 23% по сравнению с аналогичным периодом прошлого года.',
        'Для постоянных клиентов запущена бонусная программа. Каждая 10-я покупка — бесплатно.',
        'В ночь с 15 на 16 января будет проведено плановое обслуживание серверов. Возможны кратковременные перебои.',
        'В каталог добавлена новая линейка итальянских кофейных автоматов Necta Korinto Prime.',
        'Достигнута договорённость с сетью CoffeeLike о размещении 50 автоматов в их заведениях.',
        'Запущена AI-система предиктивного обслуживания, которая предсказывает поломки за 48 часов.',
        'Открыт новый сервисный центр в Воронеже для обслуживания автоматов в ЦФО.',
        'Проведён внешний аудит информационной безопасности. Все системы соответствуют стандартам ISO 27001.',
        'С 1 февраля все автоматы переведены на новую платёжную систему с поддержкой СБП и NFC.',
        'Ассортимент расширен: добавлены энергетики Red Bull, Monster и снэки от KDV.',
        'Компания получила награду "Лучший вендинг-оператор 2025" на выставке VendExpo.',
        'Запущено сезонное зимнее меню: безалкогольный глинтвейн, какао с маршмеллоу, пряный чай.',
        'Благодаря оптимизации маршрутов обслуживания среднее время простоя автоматов сократилось на 40%.',
        'Завершена интеграция системы управления с 1С:Бухгалтерия для автоматического учёта.',
        'С 1 марта вступают в силу новые тарифы на техническое обслуживание автоматов.',
        'Приглашаем на онлайн-вебинар 20 марта: "Как увеличить прибыль вендинг-сети в 2 раза".',
        'Выпущено мобильное приложение для инженеров с функцией сканирования QR-кодов и отчётов.',
        'По итогам 2025 года сеть достигла отметки в 150 активных торговых автоматов.'
    ];

    rand_date timestamp;
    mgmt_types text[] := ARRAY['Full','Partial','Self'];
    contract_statuses text[] := ARRAY['Draft','Draft','Signed','Signed','Signed','Active','Active','Active','Active','Expired'];

BEGIN

-- =============================================
-- 1. ПОЛЬЗОВАТЕЛИ (20 шт с хешированными паролями)
-- =============================================
FOR i IN 1..20 LOOP
    user_id := gen_random_uuid();
    INSERT INTO users (id, full_name, email, phone, password_hash, role, is_active, is_manager, is_engineer, is_operator, created_at, updated_at, last_login_at)
    VALUES (
        user_id,
        user_names[i],
        'user' || i || '@vendingnet.ru',
        '+7-9' || (10 + (random()*89)::int)::text || '-' || (100 + (random()*899)::int)::text || '-' || (10 + (random()*89)::int)::text || '-' || (10 + (random()*89)::int)::text,
        '$2a$11$rZbKqLmzWo3pVxKJfQ7aXO8HJwBqR6VKS0L5nN5VhYJ5nVjMJOday',
        roles[i],
        true,
        roles[i] = 'Admin',
        roles[i] = 'Engineer',
        false,
        NOW() - (interval '1 day' * (random()*365)::int),
        NOW(),
        NOW() - (interval '1 hour' * (random()*720)::int)
    )
    ON CONFLICT (email) DO NOTHING;
END LOOP;

-- =============================================
-- 2. ПРОДУКТЫ (30 наименований x 10 автоматов = 300 записей)
-- =============================================
FOR i IN 1..10 LOOP
    FOR j IN 1..30 LOOP
        prod_id := gen_random_uuid();
        INSERT INTO products (id, name, description, price, min_stock, category, vending_machine_id, quantity_available, sales_trend, created_at, updated_at)
        VALUES (
            prod_id,
            product_names[j],
            'Товар: ' || product_names[j] || ' для автомата #' || i,
            prices[j] + (random()*20 - 10)::int,
            5 + (random()*15)::int,
            categories[j],
            vm_ids[i],
            (random()*100)::int,
            round((random()*40 - 10)::numeric, 1),
            NOW() - (interval '1 day' * (random()*180)::int),
            NOW()
        );
    END LOOP;
END LOOP;

-- =============================================
-- 3. ПРОДАЖИ (5000 записей за последние 365 дней)
-- =============================================
FOR i IN 1..5000 LOOP
    rand_date := NOW() - (interval '1 minute' * (random()*525600)::int);
    INSERT INTO sales (id, vending_machine_id, product_id, quantity, total_price, payment_method, "timestamp")
    VALUES (
        gen_random_uuid(),
        vm_ids[1 + (random()*9)::int],
        (SELECT id FROM products ORDER BY random() LIMIT 1),
        1 + (random()*4)::int,
        (50 + (random()*200)::int)::decimal,
        payment_methods[1 + (random()*4)::int],
        rand_date
    );
END LOOP;

-- =============================================
-- 4. ОБСЛУЖИВАНИЕ (200 записей за год)
-- =============================================
FOR i IN 1..200 LOOP
    rand_date := NOW() - (interval '1 day' * (random()*365)::int);
    INSERT INTO maintenance (id, vending_machine_id, date, work_description, issues_found, technician_id, full_name, status, created_at, updated_at)
    VALUES (
        gen_random_uuid(),
        vm_ids[1 + (random()*9)::int],
        rand_date,
        work_descriptions[1 + (random()*14)::int],
        issues[1 + (random()*14)::int],
        admin_id,
        user_names[1 + (random()*19)::int],
        statuses[1 + (random()*14)::int],
        rand_date,
        rand_date + interval '2 hours'
    );
END LOOP;

-- =============================================
-- 5. МОДЕМЫ (40 шт, часть привязана к автоматам)
-- =============================================
FOR i IN 1..40 LOOP
    INSERT INTO modems (id, modem_number, model, status, vending_machine_id, created_at, updated_at)
    VALUES (
        gen_random_uuid(),
        'MDM-' || lpad(i::text, 4, '0'),
        modem_models[1 + (random()*9)::int],
        CASE WHEN random() < 0.7 THEN 'Active' WHEN random() < 0.9 THEN 'Inactive' ELSE 'Maintenance' END,
        CASE WHEN i <= 10 THEN vm_ids[i] ELSE NULL END,
        NOW() - (interval '1 day' * (random()*300)::int),
        NOW()
    )
    ON CONFLICT DO NOTHING;
END LOOP;

-- =============================================
-- 6. НОВОСТИ (20 шт за последний год)
-- =============================================
FOR i IN 1..20 LOOP
    rand_date := NOW() - (interval '1 day' * (random()*365)::int);
    INSERT INTO news (id, title, content, published_at, author_id, is_active)
    VALUES (
        gen_random_uuid(),
        news_titles[i],
        news_contents[i],
        rand_date,
        admin_id,
        random() > 0.15
    );
END LOOP;

-- =============================================
-- 7. КОНТРАКТЫ (50 шт за 2 года)
-- =============================================
FOR i IN 1..50 LOOP
    rand_date := NOW() - (interval '1 day' * (random()*730)::int);
    INSERT INTO contracts (id, contract_number, company_id, vending_machine_id, start_date, end_date, monthly_rent, yearly_rent, payback_period_months, insurance_required, management_type, status, signed_at, created_at, updated_at)
    VALUES (
        gen_random_uuid(),
        'C-' || to_char(rand_date, 'YYYYMMDD') || '-' || upper(substr(md5(random()::text), 1, 4)),
        comp_ids[1 + (random()*4)::int],
        vm_ids[1 + (random()*9)::int],
        rand_date,
        rand_date + (interval '1 month' * (6 + (random()*30)::int)),
        (5000 + (random()*45000)::int)::decimal,
        (60000 + (random()*500000)::int)::decimal,
        6 + (random()*30)::int,
        random() > 0.4,
        mgmt_types[1 + (random()*2)::int],
        contract_statuses[1 + (random()*9)::int],
        CASE WHEN random() > 0.3 THEN rand_date + interval '3 days' ELSE NULL END,
        rand_date,
        NOW()
    );
END LOOP;

-- =============================================
-- 8. СОБЫТИЯ (100 шт)
-- =============================================
FOR i IN 1..100 LOOP
    rand_date := NOW() - (interval '1 hour' * (random()*8760)::int);
    INSERT INTO events (id, vending_machine_id, event_type, event_description, event_date_time, severity)
    VALUES (
        gen_random_uuid(),
        vm_ids[1 + (random()*9)::int],
        (ARRAY['Error','Warning','Info','Critical','Maintenance'])[1 + (random()*4)::int],
        (ARRAY['Низкий уровень воды','Замятие купюры','Перезагрузка системы','Ошибка платёжного терминала','Превышена температура','Дверь открыта','Нет связи с сервером','Заканчивается кофе','Контейнер отходов полон','Сбой датчика','Обновление ПО завершено','Кассета для монет заполнена','Ошибка дозатора','Техобслуживание выполнено','Замена ингредиентов'])[1 + (random()*14)::int],
        rand_date,
        (ARRAY['Info','Warning','Error','Critical'])[1 + (random()*3)::int]
    );
END LOOP;

RAISE NOTICE 'SEED COMPLETED: 20 users, 300 products, 5000 sales, 200 maintenance, 40 modems, 20 news, 50 contracts, 100 events';
END $$;
