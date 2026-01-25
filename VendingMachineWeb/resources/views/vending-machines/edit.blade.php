@extends('layouts.app')

@section('title', 'Редактировать торговый автомат')

@section('content')
<div class="max-w-4xl mx-auto">
    <div class="bg-white rounded-lg shadow">
        <!-- Header -->
        <div class="px-6 py-4 border-b border-gray-200">
            <div class="flex items-center justify-between">
                <h1 class="text-xl font-semibold text-gray-800 flex items-center">
                    <i class="fas fa-edit text-blue-500 mr-3"></i>
                    {{ __('Редактировать торговый автомат') }}
                </h1>
                <a href="{{ route('vending-machines.index') }}" class="text-gray-500 hover:text-gray-700">
                    <i class="fas fa-times text-xl"></i>
                </a>
            </div>
        </div>

        <!-- Form -->
        <form action="{{ route('vending-machines.update', $vendingMachine->id) }}" method="POST" class="p-6">
            @csrf
            @method('PUT')

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <!-- Serial Number -->
                <div>
                    <label for="serial_number" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Серийный номер') }} <span class="text-red-500">*</span>
                    </label>
                    <input type="text" name="serial_number" id="serial_number"
                           value="{{ old('serial_number', $vendingMachine->serial_number) }}"
                           class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500 @error('serial_number') border-red-500 @enderror"
                           required>
                    @error('serial_number')
                        <p class="mt-1 text-sm text-red-500">{{ $message }}</p>
                    @enderror
                </div>

                <!-- Name -->
                <div>
                    <label for="name" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Название') }} <span class="text-red-500">*</span>
                    </label>
                    <input type="text" name="name" id="name" value="{{ old('name', $vendingMachine->name) }}"
                           class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500 @error('name') border-red-500 @enderror"
                           required>
                    @error('name')
                        <p class="mt-1 text-sm text-red-500">{{ $message }}</p>
                    @enderror
                </div>

                <!-- Model -->
                <div>
                    <label for="model" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Модель') }} <span class="text-red-500">*</span>
                    </label>
                    <select name="model" id="model"
                            class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500 @error('model') border-red-500 @enderror"
                            required>
                        <option value="">{{ __('Выберите модель') }}</option>
                        @foreach(['Saeco Cristallo 400', 'Unicum Rosso', 'Bianchi BVM 972', 'Necta Kikko Max', 'Rheavendors Luce E5', 'FAS Perla', 'Jofemar Coffeemar G250', 'Necta Kikko ES6', 'Unicum Food Box'] as $model)
                            <option value="{{ $model }}" {{ old('model', $vendingMachine->model) == $model ? 'selected' : '' }}>{{ $model }}</option>
                        @endforeach
                    </select>
                    @error('model')
                        <p class="mt-1 text-sm text-red-500">{{ $message }}</p>
                    @enderror
                </div>

                <!-- Status -->
                <div>
                    <label for="status" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Статус') }} <span class="text-red-500">*</span>
                    </label>
                    <select name="status" id="status"
                            class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500 @error('status') border-red-500 @enderror"
                            required>
                        @foreach(['Работает', 'На обслуживании', 'В ожидании', 'Не работает'] as $status)
                            <option value="{{ $status }}" {{ old('status', $vendingMachine->status) == $status ? 'selected' : '' }}>{{ __($status) }}</option>
                        @endforeach
                    </select>
                    @error('status')
                        <p class="mt-1 text-sm text-red-500">{{ $message }}</p>
                    @enderror
                </div>

                <!-- Company -->
                <div>
                    <label for="company" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Компания') }}
                    </label>
                    <input type="text" name="company" id="company" value="{{ old('company', $vendingMachine->company) }}"
                           class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500">
                </div>

                <!-- Install Date -->
                <div>
                    <label for="install_date" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Дата установки') }} <span class="text-red-500">*</span>
                    </label>
                    <input type="date" name="install_date" id="install_date"
                           value="{{ old('install_date', $vendingMachine->install_date) }}"
                           class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500 @error('install_date') border-red-500 @enderror"
                           required>
                    @error('install_date')
                        <p class="mt-1 text-sm text-red-500">{{ $message }}</p>
                    @enderror
                </div>

                <!-- Location -->
                <div class="md:col-span-2">
                    <label for="location" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Адрес') }} <span class="text-red-500">*</span>
                    </label>
                    <input type="text" name="location" id="location" value="{{ old('location', $vendingMachine->location) }}"
                           class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500 @error('location') border-red-500 @enderror"
                           required>
                    @error('location')
                        <p class="mt-1 text-sm text-red-500">{{ $message }}</p>
                    @enderror
                </div>

                <!-- Place -->
                <div class="md:col-span-2">
                    <label for="place" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Место установки') }}
                    </label>
                    <input type="text" name="place" id="place" value="{{ old('place', $vendingMachine->place) }}"
                           class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500">
                </div>

                <!-- Working Hours -->
                <div>
                    <label for="working_hours" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Часы работы') }}
                    </label>
                    <input type="text" name="working_hours" id="working_hours"
                           value="{{ old('working_hours', $vendingMachine->working_hours) }}"
                           class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500">
                </div>

                <!-- Notes -->
                <div class="md:col-span-2">
                    <label for="notes" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Примечания') }}
                    </label>
                    <textarea name="notes" id="notes" rows="3"
                              class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500">{{ old('notes', $vendingMachine->notes) }}</textarea>
                </div>
            </div>

            <!-- Actions -->
            <div class="mt-6 flex items-center justify-end space-x-3">
                <a href="{{ route('vending-machines.index') }}"
                   class="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50 transition">
                    {{ __('Отмена') }}
                </a>
                <button type="submit"
                        class="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 transition">
                    <i class="fas fa-save mr-2"></i>
                    {{ __('Сохранить') }}
                </button>
            </div>
        </form>
    </div>
</div>
@endsection
