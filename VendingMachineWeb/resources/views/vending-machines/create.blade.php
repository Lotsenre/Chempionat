@extends('layouts.app')

@section('title', 'Добавить торговый автомат')

@section('content')
<div class="max-w-4xl mx-auto">
    <div class="bg-white rounded-lg shadow">
        <!-- Header -->
        <div class="px-6 py-4 border-b border-gray-200">
            <div class="flex items-center justify-between">
                <h1 class="text-xl font-semibold text-gray-800 flex items-center">
                    <i class="fas fa-plus-circle text-green-500 mr-3"></i>
                    {{ __('Добавить торговый автомат') }}
                </h1>
                <a href="{{ route('vending-machines.index') }}" class="text-gray-500 hover:text-gray-700">
                    <i class="fas fa-times text-xl"></i>
                </a>
            </div>
        </div>

        <!-- Form -->
        <form action="{{ route('vending-machines.store') }}" method="POST" class="p-6">
            @csrf

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <!-- Serial Number -->
                <div>
                    <label for="serial_number" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Серийный номер') }} <span class="text-red-500">*</span>
                    </label>
                    <input type="text" name="serial_number" id="serial_number" value="{{ old('serial_number') }}"
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
                    <input type="text" name="name" id="name" value="{{ old('name') }}"
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
                        <option value="Saeco Cristallo 400" {{ old('model') == 'Saeco Cristallo 400' ? 'selected' : '' }}>Saeco Cristallo 400</option>
                        <option value="Unicum Rosso" {{ old('model') == 'Unicum Rosso' ? 'selected' : '' }}>Unicum Rosso</option>
                        <option value="Bianchi BVM 972" {{ old('model') == 'Bianchi BVM 972' ? 'selected' : '' }}>Bianchi BVM 972</option>
                        <option value="Necta Kikko Max" {{ old('model') == 'Necta Kikko Max' ? 'selected' : '' }}>Necta Kikko Max</option>
                        <option value="Rheavendors Luce E5" {{ old('model') == 'Rheavendors Luce E5' ? 'selected' : '' }}>Rheavendors Luce E5</option>
                        <option value="FAS Perla" {{ old('model') == 'FAS Perla' ? 'selected' : '' }}>FAS Perla</option>
                        <option value="Jofemar Coffeemar G250" {{ old('model') == 'Jofemar Coffeemar G250' ? 'selected' : '' }}>Jofemar Coffeemar G250</option>
                        <option value="Necta Kikko ES6" {{ old('model') == 'Necta Kikko ES6' ? 'selected' : '' }}>Necta Kikko ES6</option>
                        <option value="Unicum Food Box" {{ old('model') == 'Unicum Food Box' ? 'selected' : '' }}>Unicum Food Box</option>
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
                        <option value="Работает" {{ old('status') == 'Работает' ? 'selected' : '' }}>{{ __('Работает') }}</option>
                        <option value="На обслуживании" {{ old('status') == 'На обслуживании' ? 'selected' : '' }}>{{ __('На обслуживании') }}</option>
                        <option value="В ожидании" {{ old('status') == 'В ожидании' ? 'selected' : '' }}>{{ __('В ожидании') }}</option>
                        <option value="Не работает" {{ old('status') == 'Не работает' ? 'selected' : '' }}>{{ __('Не работает') }}</option>
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
                    <input type="text" name="company" id="company" value="{{ old('company') }}"
                           class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500">
                </div>

                <!-- Install Date -->
                <div>
                    <label for="install_date" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Дата установки') }} <span class="text-red-500">*</span>
                    </label>
                    <input type="date" name="install_date" id="install_date" value="{{ old('install_date', date('Y-m-d')) }}"
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
                    <input type="text" name="location" id="location" value="{{ old('location') }}"
                           class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500 @error('location') border-red-500 @enderror"
                           placeholder="{{ __('г. Москва, ул. Примерная, д. 1') }}"
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
                    <input type="text" name="place" id="place" value="{{ old('place') }}"
                           class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500"
                           placeholder="{{ __('1 этаж, фойе') }}">
                </div>

                <!-- Working Hours -->
                <div>
                    <label for="working_hours" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Часы работы') }}
                    </label>
                    <input type="text" name="working_hours" id="working_hours" value="{{ old('working_hours', '9:00 - 18:00') }}"
                           class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500">
                </div>

                <!-- Notes -->
                <div class="md:col-span-2">
                    <label for="notes" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Примечания') }}
                    </label>
                    <textarea name="notes" id="notes" rows="3"
                              class="w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500">{{ old('notes') }}</textarea>
                </div>
            </div>

            <!-- Actions -->
            <div class="mt-6 flex items-center justify-end space-x-3">
                <a href="{{ route('vending-machines.index') }}"
                   class="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50 transition">
                    {{ __('Отмена') }}
                </a>
                <button type="submit"
                        class="px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600 transition">
                    <i class="fas fa-save mr-2"></i>
                    {{ __('Сохранить') }}
                </button>
            </div>
        </form>
    </div>
</div>
@endsection
