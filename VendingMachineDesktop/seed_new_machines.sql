DO $$
DECLARE
    vm RECORD;
    products_arr TEXT[] := ARRAY['Кофе Американо','Кофе Латте','Кофе Капучино','Горячий шоколад','Чай черный','Чай зеленый','Эспрессо','Мокка','Кофе с молоком','Какао'];
    prices NUMERIC[] := ARRAY[80,120,130,100,60,60,70,140,110,90];
    cats TEXT[] := ARRAY['Кофе','Кофе','Кофе','Горячие напитки','Чай','Чай','Кофе','Кофе','Кофе','Горячие напитки'];
    i INT;
    prod_id UUID;
    sale_ts TIMESTAMP;
    j INT;
    maint_date TIMESTAMP;
BEGIN
    FOR vm IN SELECT id, name FROM vending_machines WHERE serial_number LIKE '912340%' ORDER BY serial_number
    LOOP
        FOR i IN 1..6 LOOP
            prod_id := uuid_generate_v4();
            INSERT INTO products (id, name, category, price, quantity_available, min_stock, vending_machine_id, created_at, updated_at)
            VALUES (prod_id, products_arr[((i-1) % 10) + 1], cats[((i-1) % 10) + 1], prices[((i-1) % 10) + 1],
                    floor(random()*80+20)::int, floor(random()*10+5)::int, vm.id, NOW(), NOW());

            FOR j IN 1..floor(random()*150+50)::int LOOP
                sale_ts := '2024-01-01'::timestamp + (random() * 790)::int * interval '1 day' + (random() * 16 + 6)::int * interval '1 hour';
                INSERT INTO sales (id, product_id, vending_machine_id, quantity, total_price, payment_method, "timestamp")
                VALUES (uuid_generate_v4(), prod_id, vm.id, floor(random()*3+1)::int,
                        prices[((i-1) % 10) + 1] * floor(random()*3+1)::int,
                        (ARRAY['Cash','Card','QR'])[floor(random()*3+1)::int],
                        sale_ts);
            END LOOP;
        END LOOP;

        FOR i IN 1..floor(random()*10+5)::int LOOP
            maint_date := '2024-01-01'::timestamp + (random() * 790)::int * interval '1 day';
            INSERT INTO maintenance (id, vending_machine_id, date, work_description, issues_found, full_name, status, created_at, updated_at)
            VALUES (uuid_generate_v4(), vm.id, maint_date,
                    (ARRAY['Плановое ТО','Замена фильтра','Чистка системы','Ремонт монетоприемника','Замена нагревателя','Диагностика электроники','Обновление прошивки'])[floor(random()*7+1)::int],
                    CASE WHEN random() > 0.5 THEN (ARRAY['Износ уплотнителя','Засор трубки','Ошибка датчика','Утечка воды','Нет проблем'])[floor(random()*5+1)::int] ELSE NULL END,
                    (ARRAY['Козлов Д.И.','Петров М.А.','Сидоров А.Н.','Николаев К.П.'])[floor(random()*4+1)::int],
                    (ARRAY['Completed','Scheduled','InProgress'])[floor(random()*3+1)::int],
                    NOW(), NOW());
        END LOOP;
    END LOOP;
END $$;
