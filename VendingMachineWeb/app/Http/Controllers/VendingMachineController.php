<?php

namespace App\Http\Controllers;

use App\Services\VendingApiService;
use Illuminate\Http\Request;
use Illuminate\Support\Collection;

class VendingMachineController extends Controller
{
    protected VendingApiService $apiService;

    public function __construct(VendingApiService $apiService)
    {
        $this->apiService = $apiService;
    }

    public function index(Request $request)
    {
        $perPage = $request->get('per_page', 30);
        $page = $request->get('page', 1);
        $search = $request->get('search');

        $response = $this->apiService->getVendingMachines($page, $perPage, $search);

        if (!$response['success']) {
            return redirect()->route('login')
                ->with('error', 'Необходима авторизация');
        }

        $machinesData = $response['data'] ?? [];
        $totalCount = (int) ($response['total'] ?? count($machinesData));

        // Transform API data to match view expectations
        $machines = collect($machinesData)->map(function ($machine) {
            return (object) [
                'id' => $machine['id'],
                'serial_number' => $machine['serialNumber'] ?? '',
                'name' => $machine['name'] ?? '',
                'model' => $machine['model'] ?? '',
                'status' => $machine['status'] ?? 'Unknown',
                'location' => $machine['location'] ?? '',
                'place' => $machine['place'] ?? null,
                'company' => $machine['company'] ?? null,
                'install_date' => isset($machine['installDate']) ? new \DateTime($machine['installDate']) : null,
                'working_hours' => $machine['workingHours'] ?? null,
                'notes' => $machine['notes'] ?? null,
                'modem' => $machine['kitOnlineId'] ? (object) ['modem_number' => $machine['kitOnlineId']] : null,
                'kit_online_id' => $machine['kitOnlineId'] ?? null,
            ];
        });

        // Create a custom paginator
        $machines = new \Illuminate\Pagination\LengthAwarePaginator(
            $machines,
            $totalCount,
            $perPage,
            $page,
            ['path' => request()->url(), 'query' => request()->query()]
        );

        return view('vending-machines.index', compact('machines', 'totalCount'));
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
            'place' => 'nullable|string|max:500',
            'company' => 'nullable|string|max:255',
            'install_date' => 'required|date',
            'working_hours' => 'nullable|string|max:50',
            'notes' => 'nullable|string',
        ]);

        $apiData = [
            'serialNumber' => $validated['serial_number'],
            'name' => $validated['name'],
            'model' => $validated['model'],
            'location' => $validated['location'],
            'place' => $validated['place'] ?? null,
            'company' => $validated['company'] ?? null,
            'installDate' => $validated['install_date'] . 'T00:00:00Z',
            'workingHours' => $validated['working_hours'] ?? null,
            'notes' => $validated['notes'] ?? null,
        ];

        $response = $this->apiService->createVendingMachine($apiData);

        if (!$response['success']) {
            return redirect()->back()
                ->withInput()
                ->with('error', $response['message'] ?? 'Ошибка при создании торгового автомата');
        }

        return redirect()->route('vending-machines.index')
            ->with('success', 'Торговый автомат успешно добавлен');
    }

    public function edit(string $id)
    {
        $response = $this->apiService->getVendingMachine($id);

        if (!$response['success']) {
            return redirect()->route('vending-machines.index')
                ->with('error', 'Торговый автомат не найден');
        }

        $machine = $response['data'];
        $vendingMachine = (object) [
            'id' => $machine['id'],
            'serial_number' => $machine['serialNumber'] ?? '',
            'name' => $machine['name'] ?? '',
            'model' => $machine['model'] ?? '',
            'status' => $machine['status'] ?? 'Working',
            'location' => $machine['location'] ?? '',
            'place' => $machine['place'] ?? null,
            'company' => $machine['company'] ?? null,
            'install_date' => isset($machine['installDate']) ? (new \DateTime($machine['installDate']))->format('Y-m-d') : null,
            'working_hours' => $machine['workingHours'] ?? null,
            'notes' => $machine['notes'] ?? null,
        ];

        return view('vending-machines.edit', compact('vendingMachine'));
    }

    public function update(Request $request, string $id)
    {
        $validated = $request->validate([
            'serial_number' => 'required|string|max:50',
            'name' => 'required|string|max:255',
            'model' => 'required|string|max:255',
            'status' => 'required|string|max:50',
            'location' => 'required|string|max:500',
            'place' => 'nullable|string|max:500',
            'company' => 'nullable|string|max:255',
            'install_date' => 'required|date',
            'working_hours' => 'nullable|string|max:50',
            'notes' => 'nullable|string',
        ]);

        $apiData = [
            'serialNumber' => $validated['serial_number'],
            'name' => $validated['name'],
            'model' => $validated['model'],
            'status' => $validated['status'],
            'location' => $validated['location'],
            'place' => $validated['place'] ?? null,
            'company' => $validated['company'] ?? null,
            'workingHours' => $validated['working_hours'] ?? null,
            'notes' => $validated['notes'] ?? null,
        ];

        $response = $this->apiService->updateVendingMachine($id, $apiData);

        if (!$response['success']) {
            return redirect()->back()
                ->withInput()
                ->with('error', $response['message'] ?? 'Ошибка при обновлении торгового автомата');
        }

        return redirect()->route('vending-machines.index')
            ->with('success', 'Торговый автомат успешно обновлен');
    }

    public function destroy(string $id)
    {
        $response = $this->apiService->deleteVendingMachine($id);

        if (!$response['success']) {
            return redirect()->route('vending-machines.index')
                ->with('error', $response['message'] ?? 'Ошибка при удалении торгового автомата');
        }

        return redirect()->route('vending-machines.index')
            ->with('success', 'Торговый автомат успешно удален');
    }

    public function detachModem(string $id)
    {
        $response = $this->apiService->detachModem($id);

        if (!$response['success']) {
            return redirect()->route('vending-machines.index')
                ->with('error', $response['message'] ?? 'Ошибка при отвязке модема');
        }

        return redirect()->route('vending-machines.index')
            ->with('success', 'Модем успешно отвязан');
    }

    public function export(Request $request)
    {
        $search = $request->get('search');

        // Fetch all machines for export (up to 1000)
        $response = $this->apiService->getVendingMachines(1, 1000, $search);

        if (!$response['success']) {
            return redirect()->route('vending-machines.index')
                ->with('error', 'Ошибка при экспорте данных');
        }

        $machinesData = $response['data'] ?? [];

        $filename = 'vending_machines_' . date('Y-m-d_H-i-s') . '.csv';

        $headers = [
            'Content-Type' => 'text/csv; charset=utf-8',
            'Content-Disposition' => 'attachment; filename="' . $filename . '"',
        ];

        $callback = function () use ($machinesData) {
            $file = fopen('php://output', 'w');
            fprintf($file, chr(0xEF) . chr(0xBB) . chr(0xBF)); // BOM for UTF-8

            fputcsv($file, [
                'ID',
                'Название',
                'Модель',
                'Компания',
                'Модем',
                'Адрес/Место',
                'Статус',
                'В работе'
            ], ';');

            foreach ($machinesData as $machine) {
                $installDate = isset($machine['installDate'])
                    ? (new \DateTime($machine['installDate']))->format('d.m.Y')
                    : '';

                fputcsv($file, [
                    $machine['serialNumber'] ?? '',
                    $machine['name'] ?? '',
                    $machine['model'] ?? '',
                    $machine['company'] ?? '',
                    $machine['kitOnlineId'] ?? '',
                    ($machine['location'] ?? '') . (isset($machine['place']) ? ', ' . $machine['place'] : ''),
                    $machine['status'] ?? '',
                    $installDate,
                ], ';');
            }

            fclose($file);
        };

        return response()->stream($callback, 200, $headers);
    }
}
