<?php

namespace Database\Seeders;

use App\Models\VendingMachine;
use App\Models\Modem;
use Illuminate\Database\Seeder;
use Illuminate\Support\Str;

class VendingMachineSeeder extends Seeder
{
    public function run(): void
    {
        $machines = [
            [
                'serial_number' => '903823',
                'name' => 'БЦ Монолит',
                'model' => 'Saeco Cristallo 400',
                'company' => 'ООО Торговые Автоматы',
                'location' => 'г. Москва, ул. Пресненская, д. 12',
                'place' => 'Фойе',
                'status' => 'Работает',
                'install_date' => '2023-05-12',
                'working_hours' => '9:00 - 18:00',
                'total_income' => 15389.38,
            ],
            [
                'serial_number' => '903828',
                'name' => 'ТЦ Мегамолл',
                'model' => 'Unicum Rosso',
                'company' => 'ООО ТРЦ Золотой Город',
                'location' => 'г. Москва, ул. Тверская, д. 7',
                'place' => '1 этаж',
                'status' => 'Работает',
                'install_date' => '2023-05-15',
                'working_hours' => '9:00 - 18:00',
                'total_income' => 19713.49,
            ],
            [
                'serial_number' => '903829',
                'name' => 'ДОСААФ',
                'model' => 'Bianchi BVM 972',
                'company' => 'ООО Региональный Союз',
                'location' => 'г. Подольск, ул. Ленина, д. 4',
                'place' => 'Холл',
                'status' => 'На обслуживании',
                'install_date' => '2024-09-11',
                'working_hours' => '9:00 - 18:00',
                'total_income' => 20416.26,
            ],
            [
                'serial_number' => '903826',
                'name' => 'Завод Тайфун',
                'model' => 'Necta Kikko Max',
                'company' => 'ООО Торговые Автоматы',
                'location' => 'г. Рязань, ул. Промышленная, д. 15',
                'place' => '2 этаж',
                'status' => 'Работает',
                'install_date' => '2023-06-01',
                'working_hours' => '9:00 - 18:00',
                'total_income' => 8447.97,
            ],
            [
                'serial_number' => '903834',
                'name' => 'Майка +2',
                'model' => 'Rheavendors Luce E5',
                'company' => 'ООО Торговые Автоматы',
                'location' => 'г. Тула, ул. Советская, д. 31',
                'place' => 'В зале',
                'status' => 'В ожидании',
                'install_date' => '2024-06-02',
                'working_hours' => '9:00 - 18:00',
                'total_income' => 5729.0,
            ],
            [
                'serial_number' => '903827',
                'name' => 'FAS Perla',
                'model' => 'FAS Perla',
                'company' => 'ООО Торговые Автоматы',
                'location' => 'г. Воронеж, ул. Кирова, д. 12',
                'place' => 'Фойе',
                'status' => 'Работает',
                'install_date' => '2023-05-25',
                'working_hours' => '9:00 - 18:00',
                'total_income' => 28231.5,
            ],
            [
                'serial_number' => '909822',
                'name' => 'Ранчо',
                'model' => 'Jofemar Coffeemar G250',
                'company' => 'ООО Торговые Автоматы',
                'location' => 'г. Самара, ул. Московская, д. 48',
                'place' => '1 этаж',
                'status' => 'Не работает',
                'install_date' => '2024-01-05',
                'working_hours' => '9:00 - 18:00',
                'total_income' => 31753.4,
            ],
            [
                'serial_number' => '903820',
                'name' => 'ТК +21 Век+',
                'model' => 'Necta Kikko ES6',
                'company' => 'ООО Торговые Автоматы',
                'location' => 'г. Казань, ул. Баумана, д. 19',
                'place' => '3 этаж',
                'status' => 'Работает',
                'install_date' => '2023-07-26',
                'working_hours' => '9:00 - 18:00',
                'total_income' => 20319.68,
            ],
            [
                'serial_number' => '903821',
                'name' => 'ТК +21 Века+',
                'model' => 'Unicum Food Box',
                'company' => 'ООО Торговые Автоматы',
                'location' => 'г. Казань, ул. Пушкина, д. 11',
                'place' => '1 этаж',
                'status' => 'Работает',
                'install_date' => '2023-08-15',
                'working_hours' => '9:00 - 18:00',
                'total_income' => 22529.01,
            ],
        ];

        $modemModels = ['Cinterion BGS2', 'Quectel M95', 'SIMCom SIM800L', 'Telit GL865'];

        foreach ($machines as $machineData) {
            $machineId = Str::uuid();

            $machine = VendingMachine::create(array_merge($machineData, [
                'id' => $machineId,
                'coordinates' => rand(-90, 90) . '.' . rand(100000, 999999) . ', ' . rand(-180, 180) . '.' . rand(100000, 999999),
                'timezone' => 'UTC+3',
            ]));

            // Add modem to some machines
            if (rand(0, 1)) {
                Modem::create([
                    'id' => Str::uuid(),
                    'modem_number' => '1824' . rand(10000, 99999),
                    'model' => $modemModels[array_rand($modemModels)],
                    'status' => 'Active',
                    'vending_machine_id' => $machineId,
                ]);
            }
        }
    }
}
