@extends('layouts.app')

@section('title', 'Главная')

@section('content')
<div class="space-y-6">
    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <!-- Total Machines -->
        <div class="bg-white rounded-lg shadow p-6">
            <div class="flex items-center">
                <div class="p-3 rounded-full bg-blue-100 text-blue-600">
                    <i class="fas fa-cash-register text-2xl"></i>
                </div>
                <div class="ml-4">
                    <p class="text-sm text-gray-500">{{ __('Всего автоматов') }}</p>
                    <p class="text-2xl font-semibold text-gray-800">{{ $stats['total_machines'] }}</p>
                </div>
            </div>
        </div>

        <!-- Working Machines -->
        <div class="bg-white rounded-lg shadow p-6">
            <div class="flex items-center">
                <div class="p-3 rounded-full bg-green-100 text-green-600">
                    <i class="fas fa-check-circle text-2xl"></i>
                </div>
                <div class="ml-4">
                    <p class="text-sm text-gray-500">{{ __('Работает') }}</p>
                    <p class="text-2xl font-semibold text-gray-800">{{ $stats['working_machines'] }}</p>
                </div>
            </div>
        </div>

        <!-- Not Working -->
        <div class="bg-white rounded-lg shadow p-6">
            <div class="flex items-center">
                <div class="p-3 rounded-full bg-red-100 text-red-600">
                    <i class="fas fa-times-circle text-2xl"></i>
                </div>
                <div class="ml-4">
                    <p class="text-sm text-gray-500">{{ __('Не работает') }}</p>
                    <p class="text-2xl font-semibold text-gray-800">{{ $stats['not_working_machines'] }}</p>
                </div>
            </div>
        </div>

        <!-- Total Income -->
        <div class="bg-white rounded-lg shadow p-6">
            <div class="flex items-center">
                <div class="p-3 rounded-full bg-yellow-100 text-yellow-600">
                    <i class="fas fa-ruble-sign text-2xl"></i>
                </div>
                <div class="ml-4">
                    <p class="text-sm text-gray-500">{{ __('Общий доход') }}</p>
                    <p class="text-2xl font-semibold text-gray-800">{{ number_format($stats['total_income'], 2, ',', ' ') }} ₽</p>
                </div>
            </div>
        </div>
    </div>

    <!-- Quick Actions -->
    <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-lg font-semibold text-gray-800 mb-4">{{ __('Быстрые действия') }}</h2>
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
            <a href="{{ route('vending-machines.index') }}"
               class="flex flex-col items-center p-4 bg-blue-50 rounded-lg hover:bg-blue-100 transition">
                <i class="fas fa-cash-register text-3xl text-blue-600 mb-2"></i>
                <span class="text-sm text-gray-700">{{ __('Автоматы') }}</span>
            </a>
            <a href="{{ route('vending-machines.create') }}"
               class="flex flex-col items-center p-4 bg-green-50 rounded-lg hover:bg-green-100 transition">
                <i class="fas fa-plus-circle text-3xl text-green-600 mb-2"></i>
                <span class="text-sm text-gray-700">{{ __('Добавить') }}</span>
            </a>
            <a href="#"
               class="flex flex-col items-center p-4 bg-purple-50 rounded-lg hover:bg-purple-100 transition">
                <i class="fas fa-chart-line text-3xl text-purple-600 mb-2"></i>
                <span class="text-sm text-gray-700">{{ __('Мониторинг') }}</span>
            </a>
            <a href="#"
               class="flex flex-col items-center p-4 bg-yellow-50 rounded-lg hover:bg-yellow-100 transition">
                <i class="fas fa-file-contract text-3xl text-yellow-600 mb-2"></i>
                <span class="text-sm text-gray-700">{{ __('Договоры') }}</span>
            </a>
        </div>
    </div>

    <!-- Recent Activity -->
    <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-lg font-semibold text-gray-800 mb-4">{{ __('Информация') }}</h2>
        <div class="text-center py-8 text-gray-500">
            <i class="fas fa-info-circle text-4xl mb-2"></i>
            <p>{{ __('Добро пожаловать в систему управления вендинговыми автоматами') }}</p>
            <p class="text-sm mt-2">{{ __('Используйте меню слева для навигации') }}</p>
        </div>
    </div>
</div>
@endsection
