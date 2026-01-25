@extends('layouts.app')

@section('title', 'Торговые автоматы')

@section('content')
<!-- Rental Notice Banner -->
<div x-data="rentalManager()" x-init="init()">
    <!-- First Visit Banner -->
    <div x-show="showRentalBanner" x-cloak
         class="mb-6 bg-gradient-to-r from-amber-50 to-orange-50 border border-amber-200 rounded-xl p-6 shadow-sm">
        <div class="flex items-start">
            <div class="flex-shrink-0">
                <div class="w-12 h-12 bg-amber-100 rounded-full flex items-center justify-center">
                    <i class="fas fa-exclamation-triangle text-amber-600 text-xl"></i>
                </div>
            </div>
            <div class="ml-4 flex-1">
                <h3 class="text-lg font-semibold text-amber-800">Необходимо оформить торговый автомат в аренду</h3>
                <p class="text-amber-700 mt-1">
                    Для начала работы с системой вам необходимо выбрать и забронировать торговые автоматы.
                    Выберите подходящие ТА из нашего каталога доступных аппаратов.
                </p>
                <div class="mt-4 flex items-center gap-3">
                    <button @click="openRentalModal()"
                            class="inline-flex items-center px-5 py-2.5 bg-amber-600 text-white rounded-lg font-medium hover:bg-amber-700 transition shadow-sm">
                        <i class="fas fa-shopping-cart mr-2"></i>
                        Выбрать ТА в аренду
                    </button>
                    <button @click="dismissBanner()"
                            class="text-amber-600 hover:text-amber-800 text-sm font-medium">
                        Напомнить позже
                    </button>
                </div>
            </div>
            <button @click="dismissBanner()" class="text-amber-400 hover:text-amber-600">
                <i class="fas fa-times"></i>
            </button>
        </div>
    </div>

    <!-- Rental Modal -->
    <div x-show="showModal" x-cloak
         class="fixed inset-0 z-50 overflow-y-auto"
         x-transition:enter="transition ease-out duration-300"
         x-transition:enter-start="opacity-0"
         x-transition:enter-end="opacity-100"
         x-transition:leave="transition ease-in duration-200"
         x-transition:leave-start="opacity-100"
         x-transition:leave-end="opacity-0">
        <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:p-0">
            <!-- Backdrop -->
            <div class="fixed inset-0 bg-gray-900 bg-opacity-50 transition-opacity" @click="closeModal()"></div>

            <!-- Modal Content -->
            <div class="relative bg-white rounded-2xl shadow-2xl max-w-5xl w-full mx-auto transform transition-all max-h-[90vh] flex flex-col"
                 x-transition:enter="transition ease-out duration-300"
                 x-transition:enter-start="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
                 x-transition:enter-end="opacity-100 translate-y-0 sm:scale-100"
                 @click.away="closeModal()">

                <!-- Modal Header -->
                <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between flex-shrink-0">
                    <div>
                        <h2 class="text-xl font-bold text-gray-800">Выбор торговых автоматов в аренду</h2>
                        <p class="text-sm text-gray-500 mt-1">Выберите один или несколько ТА для бронирования</p>
                    </div>
                    <button @click="closeModal()" class="text-gray-400 hover:text-gray-600 p-2">
                        <i class="fas fa-times text-xl"></i>
                    </button>
                </div>

                <!-- Filters -->
                <div class="px-6 py-4 bg-gray-50 border-b border-gray-200 flex-shrink-0">
                    <div class="flex flex-wrap items-center gap-4">
                        <!-- Model Filter -->
                        <div class="flex items-center gap-2">
                            <label class="text-sm text-gray-600">Модель:</label>
                            <select x-model="filterModel"
                                    class="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-blue-500 focus:border-blue-500">
                                <option value="">Все модели</option>
                                <template x-for="model in uniqueModels" :key="model">
                                    <option :value="model" x-text="model"></option>
                                </template>
                            </select>
                        </div>

                        <!-- Status Filter -->
                        <div class="flex items-center gap-2">
                            <label class="text-sm text-gray-600">Статус:</label>
                            <select x-model="filterStatus"
                                    class="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-blue-500 focus:border-blue-500">
                                <option value="">Все</option>
                                <option value="available">Доступен</option>
                                <option value="reserved">Забронирован</option>
                            </select>
                        </div>

                        <!-- Sort -->
                        <div class="flex items-center gap-2">
                            <label class="text-sm text-gray-600">Сортировка:</label>
                            <select x-model="sortBy"
                                    class="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-blue-500 focus:border-blue-500">
                                <option value="payback_asc">Срок окупаемости (по возрастанию)</option>
                                <option value="payback_desc">Срок окупаемости (по убыванию)</option>
                                <option value="price_asc">Цена (по возрастанию)</option>
                                <option value="price_desc">Цена (по убыванию)</option>
                            </select>
                        </div>

                        <!-- Selected Count -->
                        <div class="ml-auto">
                            <span class="text-sm text-gray-600">
                                Выбрано: <span class="font-bold text-blue-600" x-text="selectedMachines.length"></span>
                            </span>
                        </div>
                    </div>
                </div>

                <!-- Machine Cards -->
                <div class="flex-1 overflow-y-auto p-6">
                    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        <template x-for="machine in filteredMachines" :key="machine.id">
                            <div @click="toggleSelection(machine)"
                                 :class="{
                                     'ring-2 ring-blue-500 bg-blue-50': isSelected(machine.id),
                                     'hover:shadow-md': machine.status === 'available',
                                     'opacity-60 cursor-not-allowed': machine.status === 'reserved'
                                 }"
                                 class="bg-white border border-gray-200 rounded-xl p-4 cursor-pointer transition-all relative">

                                <!-- Selection Checkbox -->
                                <div class="absolute top-3 right-3">
                                    <div :class="isSelected(machine.id) ? 'bg-blue-500 border-blue-500' : 'border-gray-300'"
                                         class="w-6 h-6 rounded-full border-2 flex items-center justify-center transition-colors">
                                        <i x-show="isSelected(machine.id)" class="fas fa-check text-white text-xs"></i>
                                    </div>
                                </div>

                                <!-- Status Badge -->
                                <div class="mb-3">
                                    <span :class="machine.status === 'available' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'"
                                          class="px-2 py-1 rounded-full text-xs font-medium">
                                        <i :class="machine.status === 'available' ? 'fas fa-check-circle' : 'fas fa-clock'" class="mr-1"></i>
                                        <span x-text="machine.status === 'available' ? 'Доступен' : 'Забронирован'"></span>
                                    </span>
                                </div>

                                <!-- Machine Info -->
                                <h3 class="font-semibold text-gray-800 mb-1" x-text="machine.name"></h3>
                                <p class="text-sm text-gray-500 mb-3" x-text="machine.model"></p>

                                <!-- Details -->
                                <div class="space-y-2 text-sm">
                                    <div class="flex justify-between">
                                        <span class="text-gray-500">Аренда/мес:</span>
                                        <span class="font-semibold text-gray-800" x-text="formatPrice(machine.monthlyRent) + ' ₽'"></span>
                                    </div>
                                    <div class="flex justify-between">
                                        <span class="text-gray-500">Аренда/год:</span>
                                        <span class="font-semibold text-gray-800" x-text="formatPrice(machine.yearlyRent) + ' ₽'"></span>
                                    </div>
                                    <div class="flex justify-between">
                                        <span class="text-gray-500">Окупаемость:</span>
                                        <span class="font-semibold text-emerald-600" x-text="machine.paybackMonths + ' мес.'"></span>
                                    </div>
                                    <div class="flex items-start pt-2 border-t border-gray-100">
                                        <i class="fas fa-map-marker-alt text-gray-400 mt-0.5 mr-2"></i>
                                        <span class="text-gray-600 text-xs" x-text="machine.location"></span>
                                    </div>
                                </div>
                            </div>
                        </template>
                    </div>

                    <!-- Empty State -->
                    <div x-show="filteredMachines.length === 0" class="text-center py-12">
                        <i class="fas fa-search text-gray-300 text-5xl mb-4"></i>
                        <p class="text-gray-500">Нет автоматов, соответствующих фильтрам</p>
                    </div>
                </div>

                <!-- Modal Footer -->
                <div class="px-6 py-4 border-t border-gray-200 bg-gray-50 flex items-center justify-between flex-shrink-0 rounded-b-2xl">
                    <div class="text-sm text-gray-600">
                        <template x-if="selectedMachines.length > 0">
                            <span>
                                Итого: <span class="font-bold text-gray-800" x-text="formatPrice(totalMonthlyRent) + ' ₽/мес'"></span>
                                <span class="text-gray-400 mx-2">|</span>
                                <span class="font-bold text-gray-800" x-text="formatPrice(totalYearlyRent) + ' ₽/год'"></span>
                            </span>
                        </template>
                    </div>
                    <div class="flex items-center gap-3">
                        <button @click="closeModal()"
                                class="px-4 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition">
                            Отмена
                        </button>
                        <button @click="proceedToBooking()"
                                :disabled="selectedMachines.length === 0"
                                :class="selectedMachines.length === 0 ? 'bg-gray-300 cursor-not-allowed' : 'bg-green-600 hover:bg-green-700'"
                                class="px-6 py-2 text-white rounded-lg font-medium transition flex items-center">
                            <i class="fas fa-arrow-right mr-2"></i>
                            Перейти к бронированию
                            <span x-show="selectedMachines.length > 0" class="ml-2 bg-white/20 px-2 py-0.5 rounded-full text-sm" x-text="selectedMachines.length"></span>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Booking Form Modal -->
    <div x-show="showBookingModal" x-cloak
         class="fixed inset-0 z-50 overflow-y-auto">
        <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center">
            <div class="fixed inset-0 bg-gray-900 bg-opacity-50 transition-opacity" @click="closeBookingModal()"></div>

            <div class="relative bg-white rounded-2xl shadow-2xl max-w-2xl w-full mx-auto transform transition-all">
                <!-- Header -->
                <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
                    <div>
                        <h2 class="text-xl font-bold text-gray-800">Оформление бронирования</h2>
                        <p class="text-sm text-gray-500 mt-1">Заполните данные для бронирования <span x-text="selectedMachines.length"></span> ТА</p>
                    </div>
                    <button @click="closeBookingModal()" class="text-gray-400 hover:text-gray-600 p-2">
                        <i class="fas fa-times text-xl"></i>
                    </button>
                </div>

                <!-- Form -->
                <div class="p-6 space-y-6">
                    <!-- Selected Machines Summary -->
                    <div class="bg-blue-50 rounded-lg p-4">
                        <h4 class="font-medium text-blue-800 mb-2">Выбранные автоматы:</h4>
                        <div class="flex flex-wrap gap-2">
                            <template x-for="id in selectedMachines" :key="id">
                                <span class="bg-blue-100 text-blue-800 px-3 py-1 rounded-full text-sm"
                                      x-text="getMachineName(id)"></span>
                            </template>
                        </div>
                        <div class="mt-3 pt-3 border-t border-blue-200 text-sm text-blue-700">
                            Итого: <span class="font-bold" x-text="formatPrice(totalMonthlyRent) + ' ₽/мес'"></span>
                        </div>
                    </div>

                    <!-- Booking Dates -->
                    <div class="grid grid-cols-2 gap-4">
                        <div>
                            <label class="block text-sm font-medium text-gray-700 mb-1">Дата начала</label>
                            <input type="date" x-model="bookingForm.startDate"
                                   :min="todayDate"
                                   class="w-full border border-gray-300 rounded-lg px-4 py-2.5 focus:ring-2 focus:ring-blue-500 focus:border-blue-500">
                        </div>
                        <div>
                            <label class="block text-sm font-medium text-gray-700 mb-1">Дата окончания</label>
                            <input type="date" x-model="bookingForm.endDate"
                                   :min="bookingForm.startDate || todayDate"
                                   class="w-full border border-gray-300 rounded-lg px-4 py-2.5 focus:ring-2 focus:ring-blue-500 focus:border-blue-500">
                        </div>
                    </div>

                    <!-- Franchise Type -->
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-2">Способ ведения франчайзи</label>
                        <div class="space-y-2">
                            <label class="flex items-center p-3 border rounded-lg cursor-pointer transition"
                                   :class="bookingForm.franchiseType === 'self' ? 'border-blue-500 bg-blue-50' : 'border-gray-200 hover:bg-gray-50'">
                                <input type="radio" x-model="bookingForm.franchiseType" value="self" class="sr-only">
                                <div class="w-5 h-5 rounded-full border-2 mr-3 flex items-center justify-center"
                                     :class="bookingForm.franchiseType === 'self' ? 'border-blue-500' : 'border-gray-300'">
                                    <div x-show="bookingForm.franchiseType === 'self'" class="w-2.5 h-2.5 rounded-full bg-blue-500"></div>
                                </div>
                                <div>
                                    <span class="font-medium text-gray-800">Самостоятельное управление</span>
                                    <p class="text-sm text-gray-500">Вы сами управляете автоматами</p>
                                </div>
                            </label>
                            <label class="flex items-center p-3 border rounded-lg cursor-pointer transition"
                                   :class="bookingForm.franchiseType === 'managed' ? 'border-blue-500 bg-blue-50' : 'border-gray-200 hover:bg-gray-50'">
                                <input type="radio" x-model="bookingForm.franchiseType" value="managed" class="sr-only">
                                <div class="w-5 h-5 rounded-full border-2 mr-3 flex items-center justify-center"
                                     :class="bookingForm.franchiseType === 'managed' ? 'border-blue-500' : 'border-gray-300'">
                                    <div x-show="bookingForm.franchiseType === 'managed'" class="w-2.5 h-2.5 rounded-full bg-blue-500"></div>
                                </div>
                                <div>
                                    <span class="font-medium text-gray-800">С поддержкой франчайзера</span>
                                    <p class="text-sm text-gray-500">Франчайзер помогает с управлением (+10% к стоимости)</p>
                                </div>
                            </label>
                            <label class="flex items-center p-3 border rounded-lg cursor-pointer transition"
                                   :class="bookingForm.franchiseType === 'full' ? 'border-blue-500 bg-blue-50' : 'border-gray-200 hover:bg-gray-50'">
                                <input type="radio" x-model="bookingForm.franchiseType" value="full" class="sr-only">
                                <div class="w-5 h-5 rounded-full border-2 mr-3 flex items-center justify-center"
                                     :class="bookingForm.franchiseType === 'full' ? 'border-blue-500' : 'border-gray-300'">
                                    <div x-show="bookingForm.franchiseType === 'full'" class="w-2.5 h-2.5 rounded-full bg-blue-500"></div>
                                </div>
                                <div>
                                    <span class="font-medium text-gray-800">Полное управление франчайзером</span>
                                    <p class="text-sm text-gray-500">Франчайзер полностью управляет автоматами (+25% к стоимости)</p>
                                </div>
                            </label>
                        </div>
                    </div>

                    <!-- Insurance -->
                    <div>
                        <label class="flex items-center p-4 border border-gray-200 rounded-lg cursor-pointer hover:bg-gray-50 transition">
                            <input type="checkbox" x-model="bookingForm.insurance" class="w-5 h-5 text-blue-600 border-gray-300 rounded focus:ring-blue-500">
                            <div class="ml-3">
                                <span class="font-medium text-gray-800">Страхование оборудования</span>
                                <p class="text-sm text-gray-500">Защита от повреждений и кражи (+5% к стоимости, не обязательно)</p>
                            </div>
                        </label>
                    </div>

                    <!-- Total -->
                    <div class="bg-gray-50 rounded-lg p-4">
                        <div class="flex justify-between items-center text-lg">
                            <span class="font-medium text-gray-700">Итоговая стоимость:</span>
                            <span class="font-bold text-gray-900" x-text="formatPrice(calculateTotal()) + ' ₽/мес'"></span>
                        </div>
                    </div>
                </div>

                <!-- Footer -->
                <div class="px-6 py-4 border-t border-gray-200 bg-gray-50 flex items-center justify-between rounded-b-2xl">
                    <button @click="closeBookingModal()"
                            class="px-4 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition">
                        Назад
                    </button>
                    <button @click="submitBooking()"
                            :disabled="isSubmitting || !isBookingFormValid()"
                            :class="isSubmitting || !isBookingFormValid() ? 'bg-gray-300 cursor-not-allowed' : 'bg-green-600 hover:bg-green-700'"
                            class="px-6 py-2 text-white rounded-lg font-medium transition flex items-center">
                        <template x-if="isSubmitting">
                            <i class="fas fa-spinner fa-spin mr-2"></i>
                        </template>
                        <template x-if="!isSubmitting">
                            <i class="fas fa-paper-plane mr-2"></i>
                        </template>
                        <span x-text="isSubmitting ? 'Отправка...' : 'Отправить заявку'"></span>
                    </button>
                </div>
            </div>
        </div>
    </div>

    <!-- Franchisor Confirmation Modal (Emulation) -->
    <div x-show="showConfirmationModal" x-cloak
         class="fixed inset-0 z-50 overflow-y-auto">
        <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center">
            <div class="fixed inset-0 bg-gray-900 bg-opacity-50 transition-opacity"></div>

            <div class="relative bg-white rounded-2xl shadow-2xl max-w-md w-full mx-auto transform transition-all p-6">
                <!-- Loading State -->
                <div x-show="confirmationStep === 'loading'" class="text-center py-8">
                    <div class="w-16 h-16 border-4 border-blue-500 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
                    <h3 class="text-lg font-semibold text-gray-800">Ожидание подтверждения</h3>
                    <p class="text-gray-500 mt-2">Франчайзер рассматривает вашу заявку...</p>
                </div>

                <!-- Success State -->
                <div x-show="confirmationStep === 'success'" class="text-center py-4">
                    <div class="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <i class="fas fa-check text-green-600 text-4xl"></i>
                    </div>
                    <h3 class="text-xl font-bold text-gray-800">Бронирование подтверждено!</h3>
                    <p class="text-gray-500 mt-2">Франчайзер одобрил вашу заявку</p>

                    <div class="mt-6 p-4 bg-gray-50 rounded-lg text-left">
                        <p class="text-sm text-gray-600 mb-2">Номер договора:</p>
                        <p class="font-mono font-bold text-lg text-gray-800" x-text="generatedContractNumber"></p>
                    </div>

                    <div class="mt-6 flex gap-3">
                        <button @click="closeConfirmationModal()"
                                class="flex-1 px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition">
                            Закрыть
                        </button>
                        <button @click="goToContract()"
                                class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition flex items-center justify-center">
                            <i class="fas fa-file-signature mr-2"></i>
                            Подписать договор
                        </button>
                    </div>
                </div>

                <!-- Error State -->
                <div x-show="confirmationStep === 'error'" class="text-center py-4">
                    <div class="w-20 h-20 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <i class="fas fa-times text-red-600 text-4xl"></i>
                    </div>
                    <h3 class="text-xl font-bold text-gray-800">Ошибка бронирования</h3>
                    <p class="text-gray-500 mt-2" x-text="confirmationError"></p>

                    <button @click="closeConfirmationModal()"
                            class="mt-6 w-full px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition">
                        Закрыть
                    </button>
                </div>
            </div>
        </div>
    </div>

    <!-- Contract Signing Modal -->
    <div x-show="showContractModal" x-cloak
         class="fixed inset-0 z-50 overflow-y-auto">
        <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center">
            <div class="fixed inset-0 bg-gray-900 bg-opacity-50 transition-opacity" @click="closeContractModal()"></div>

            <div class="relative bg-white rounded-2xl shadow-2xl max-w-4xl w-full mx-auto transform transition-all max-h-[90vh] flex flex-col">
                <!-- Header -->
                <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between flex-shrink-0">
                    <div>
                        <h2 class="text-xl font-bold text-gray-800">Подписание договора</h2>
                        <p class="text-sm text-gray-500 mt-1">Договор № <span x-text="generatedContractNumber"></span></p>
                    </div>
                    <button @click="closeContractModal()" class="text-gray-400 hover:text-gray-600 p-2">
                        <i class="fas fa-times text-xl"></i>
                    </button>
                </div>

                <!-- Contract Content -->
                <div class="flex-1 overflow-hidden flex flex-col">
                    <!-- PDF Viewer (Simulated) -->
                    <div class="flex-1 overflow-y-auto p-6 bg-gray-100">
                        <div class="bg-white shadow-lg rounded-lg p-8 max-w-3xl mx-auto" id="contract-content">
                            <div class="text-center mb-8">
                                <h1 class="text-2xl font-bold text-gray-900">ДОГОВОР ФРАНЧАЙЗИНГА</h1>
                                <p class="text-gray-600 mt-2">№ <span x-text="generatedContractNumber"></span></p>
                                <p class="text-gray-500 text-sm" x-text="'от ' + formatDateRu(new Date())"></p>
                            </div>

                            <div class="space-y-4 text-sm text-gray-700 leading-relaxed">
                                <p><strong>1. ПРЕДМЕТ ДОГОВОРА</strong></p>
                                <p>1.1. Франчайзер предоставляет Франчайзи право на использование торговых автоматов, указанных в Приложении 1 к настоящему договору.</p>
                                <p>1.2. Франчайзи обязуется использовать предоставленное оборудование в соответствии с условиями настоящего договора.</p>

                                <p class="mt-6"><strong>2. СРОК ДЕЙСТВИЯ ДОГОВОРА</strong></p>
                                <p>2.1. Договор вступает в силу с <span class="font-medium" x-text="formatDateRu(new Date(bookingForm.startDate))"></span> и действует до <span class="font-medium" x-text="formatDateRu(new Date(bookingForm.endDate))"></span>.</p>

                                <p class="mt-6"><strong>3. СТОИМОСТЬ И ПОРЯДОК ОПЛАТЫ</strong></p>
                                <p>3.1. Ежемесячная арендная плата составляет <span class="font-bold" x-text="formatPrice(calculateTotal()) + ' рублей'"></span>.</p>
                                <p>3.2. Оплата производится не позднее 5 числа каждого месяца.</p>

                                <p class="mt-6"><strong>4. СПОСОБ ВЕДЕНИЯ</strong></p>
                                <p>4.1. Выбранный способ ведения франчайзи: <span class="font-medium" x-text="getFranchiseTypeName()"></span>.</p>

                                <template x-if="bookingForm.insurance">
                                    <div>
                                        <p class="mt-6"><strong>5. СТРАХОВАНИЕ</strong></p>
                                        <p>5.1. Оборудование застраховано от повреждений и кражи.</p>
                                    </div>
                                </template>

                                <p class="mt-6"><strong>6. РЕКВИЗИТЫ СТОРОН</strong></p>
                                <div class="grid grid-cols-2 gap-8 mt-4">
                                    <div>
                                        <p class="font-medium">Франчайзер:</p>
                                        <p>ООО "Джутсу Вендинг"</p>
                                        <p>ИНН: 7707123456</p>
                                        <p>г. Москва, ул. Примерная, д. 1</p>
                                    </div>
                                    <div>
                                        <p class="font-medium">Франчайзи:</p>
                                        <p x-text="'{{ session('user.name', 'Пользователь') }}'"></p>
                                        <p x-text="'{{ session('user.email', '') }}'"></p>
                                    </div>
                                </div>

                                <!-- Signature Area -->
                                <div class="mt-8 pt-6 border-t border-gray-200">
                                    <p class="font-medium mb-4">Подписи сторон:</p>
                                    <div class="grid grid-cols-2 gap-8">
                                        <div>
                                            <p class="text-gray-500 text-xs mb-2">Франчайзер:</p>
                                            <div class="h-20 border-b-2 border-gray-300 flex items-end pb-1">
                                                <img src="data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxNTAiIGhlaWdodD0iNTAiPjxwYXRoIGQ9Ik0xMCA0MEMzMCAyMCA1MCAzMCA3MCAyMFM5MCAxMCAxMTAgMzBTMTMwIDIwIDE0MCAzMCIgc3Ryb2tlPSIjMzMzIiBzdHJva2Utd2lkdGg9IjIiIGZpbGw9Im5vbmUiLz48L3N2Zz4=" alt="Подпись" class="h-12">
                                            </div>
                                            <p class="text-xs text-gray-500 mt-1">Директор Иванов И.И.</p>
                                        </div>
                                        <div>
                                            <p class="text-gray-500 text-xs mb-2">Франчайзи:</p>
                                            <div class="h-20 border-2 border-dashed border-gray-300 rounded-lg flex items-center justify-center"
                                                 :class="signatureData ? 'border-green-500 bg-green-50' : ''">
                                                <template x-if="!signatureData">
                                                    <span class="text-gray-400 text-sm">Место для подписи</span>
                                                </template>
                                                <template x-if="signatureData">
                                                    <img :src="signatureData" alt="Ваша подпись" class="h-16">
                                                </template>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Signature Pad -->
                    <div class="p-4 bg-white border-t border-gray-200">
                        <div class="flex items-start gap-4">
                            <div class="flex-1">
                                <label class="block text-sm font-medium text-gray-700 mb-2">Нарисуйте вашу подпись:</label>
                                <div class="border-2 border-gray-300 rounded-lg overflow-hidden bg-white relative">
                                    <canvas id="signaturePad" width="400" height="100" class="w-full cursor-crosshair"></canvas>
                                    <button @click="clearSignature()"
                                            class="absolute top-2 right-2 text-gray-400 hover:text-gray-600 bg-white rounded-full w-8 h-8 flex items-center justify-center shadow-sm">
                                        <i class="fas fa-eraser"></i>
                                    </button>
                                </div>
                            </div>
                            <div class="flex flex-col gap-2 pt-7">
                                <button @click="saveSignature()"
                                        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition text-sm">
                                    <i class="fas fa-save mr-1"></i> Сохранить
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Footer -->
                <div class="px-6 py-4 border-t border-gray-200 bg-gray-50 flex items-center justify-between rounded-b-2xl">
                    <div class="flex items-center text-sm text-gray-500">
                        <i class="fas fa-shield-alt text-green-500 mr-2"></i>
                        Документ будет зашифрован после подписания
                    </div>
                    <button @click="signContract()"
                            :disabled="!signatureData || isSigningContract"
                            :class="!signatureData || isSigningContract ? 'bg-gray-300 cursor-not-allowed' : 'bg-green-600 hover:bg-green-700'"
                            class="px-6 py-2 text-white rounded-lg font-medium transition flex items-center">
                        <template x-if="isSigningContract">
                            <i class="fas fa-spinner fa-spin mr-2"></i>
                        </template>
                        <template x-if="!isSigningContract">
                            <i class="fas fa-file-signature mr-2"></i>
                        </template>
                        <span x-text="isSigningContract ? 'Подписание...' : 'Подписать договор'"></span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="bg-white rounded-lg shadow" x-data="{ viewMode: localStorage.getItem('viewMode') || 'table' }" x-init="$watch('viewMode', val => localStorage.setItem('viewMode', val))">
    <!-- Header -->
    <div class="px-6 py-4 border-b border-gray-200">
        <div class="flex items-center justify-between">
            <div>
                <h1 class="text-2xl font-semibold text-gray-800 flex items-center">
                    <i class="fas fa-cash-register text-blue-500 mr-3"></i>
                    Торговые автоматы
                </h1>
                <p class="text-sm text-gray-500 mt-1">Всего найдено {{ $totalCount }} шт.</p>
            </div>
        </div>
    </div>

    <!-- Filters and Actions -->
    <div class="px-6 py-4 border-b border-gray-200 bg-gray-50">
        <div class="flex flex-wrap items-center justify-between gap-4">
            <!-- Left side: Per page and Search -->
            <div class="flex items-center gap-4">
                <!-- Per Page Selector -->
                <div class="flex items-center gap-2">
                    <label class="text-sm text-gray-600">Показать</label>
                    <form method="GET" action="{{ route('vending-machines.index') }}" id="perPageForm">
                        <input type="hidden" name="search" value="{{ request('search') }}">
                        <select name="per_page" onchange="document.getElementById('perPageForm').submit()"
                                class="border border-gray-300 rounded-md px-3 py-1.5 text-sm focus:ring-blue-500 focus:border-blue-500">
                            <option value="10" {{ request('per_page') == 10 ? 'selected' : '' }}>10</option>
                            <option value="30" {{ request('per_page', 30) == 30 ? 'selected' : '' }}>30</option>
                            <option value="50" {{ request('per_page') == 50 ? 'selected' : '' }}>50</option>
                            <option value="100" {{ request('per_page') == 100 ? 'selected' : '' }}>100</option>
                        </select>
                    </form>
                    <span class="text-sm text-gray-600">записей</span>
                </div>

                <!-- Search -->
                <form method="GET" action="{{ route('vending-machines.index') }}" class="flex items-center">
                    <input type="hidden" name="per_page" value="{{ request('per_page', 30) }}">
                    <div class="relative">
                        <input type="text" name="search" value="{{ request('search') }}"
                               placeholder="Поиск..."
                               class="border border-gray-300 rounded-md pl-10 pr-4 py-1.5 text-sm w-64 focus:ring-blue-500 focus:border-blue-500">
                        <i class="fas fa-search absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"></i>
                    </div>
                    <button type="submit" class="ml-2 px-4 py-1.5 bg-blue-500 text-white rounded-md text-sm hover:bg-blue-600">
                        <i class="fas fa-search"></i>
                    </button>
                    @if(request('search'))
                        <a href="{{ route('vending-machines.index', ['per_page' => request('per_page', 30)]) }}"
                           class="ml-2 px-4 py-1.5 bg-gray-200 text-gray-700 rounded-md text-sm hover:bg-gray-300 transition"
                           title="Сбросить поиск">
                            <i class="fas fa-times"></i>
                        </a>
                    @endif
                </form>
            </div>

            <!-- Right side: View Toggle and Action Buttons -->
            <div class="flex items-center gap-2">
                <!-- View Mode Toggle -->
                <div class="flex items-center bg-gray-100 rounded-lg p-1 border border-gray-200">
                    <button @click="viewMode = 'table'"
                            :class="viewMode === 'table' ? 'bg-white text-blue-600 shadow-sm' : 'text-gray-500 hover:text-gray-700'"
                            class="px-3 py-1.5 rounded-md text-sm font-medium transition flex items-center">
                        <i class="fas fa-table mr-1.5"></i>
                        Таблица
                    </button>
                    <button @click="viewMode = 'cards'"
                            :class="viewMode === 'cards' ? 'bg-white text-blue-600 shadow-sm' : 'text-gray-500 hover:text-gray-700'"
                            class="px-3 py-1.5 rounded-md text-sm font-medium transition flex items-center">
                        <i class="fas fa-th-large mr-1.5"></i>
                        Карточки
                    </button>
                </div>

                <button onclick="document.dispatchEvent(new CustomEvent('open-rental-modal'))"
                        class="inline-flex items-center px-4 py-2 bg-amber-500 text-white rounded-md text-sm font-medium hover:bg-amber-600 transition">
                    <i class="fas fa-shopping-cart mr-2"></i>
                    Арендовать ТА
                </button>
                <a href="{{ route('vending-machines.create') }}"
                   class="inline-flex items-center px-4 py-2 bg-green-500 text-white rounded-md text-sm font-medium hover:bg-green-600 transition">
                    <i class="fas fa-plus mr-2"></i>
                    Добавить
                </a>
                <a href="{{ route('vending-machines.export', request()->query()) }}"
                   class="inline-flex items-center px-4 py-2 bg-blue-500 text-white rounded-md text-sm font-medium hover:bg-blue-600 transition">
                    <i class="fas fa-file-export mr-2"></i>
                    Экспорт
                </a>
            </div>
        </div>
    </div>

    <!-- Table View -->
    <div x-show="viewMode === 'table'" class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
            <thead>
                <tr>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider bg-blue-50">ID</th>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider bg-gray-50">Название</th>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider bg-blue-50">Модель</th>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider bg-gray-50">Компания</th>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider bg-blue-50">Модем</th>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider bg-gray-50">Адрес/Место</th>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider bg-blue-50">В работе</th>
                    <th class="px-4 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider bg-gray-50">Действия</th>
                </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
                @forelse($machines as $machine)
                    @php
                        $statusIcon = match($machine->status) {
                            'Работает', 'Working' => '<i class="fas fa-check-circle text-green-500"></i>',
                            'На обслуживании', 'Обслуживается', 'В ожидании' => '<i class="fas fa-clock text-yellow-500"></i>',
                            'Не работает', 'NotWorking', 'Остановлен' => '<i class="fas fa-times-circle text-red-500"></i>',
                            default => '<i class="fas fa-question-circle text-gray-500"></i>'
                        };
                    @endphp
                    <tr class="hover:bg-gray-50 transition-colors">
                        <td class="px-4 py-3 whitespace-nowrap text-sm font-medium text-gray-900 bg-blue-50/50">
                            {{ $machine->serial_number }}
                        </td>
                        <td class="px-4 py-3 whitespace-nowrap text-sm text-gray-700">
                            <div class="flex items-center">
                                {!! $statusIcon !!}
                                <span class="ml-2">{{ $machine->name }}</span>
                            </div>
                        </td>
                        <td class="px-4 py-3 whitespace-nowrap text-sm text-gray-700 bg-blue-50/50">
                            {{ $machine->model }}
                        </td>
                        <td class="px-4 py-3 whitespace-nowrap text-sm text-gray-700">
                            {{ $machine->company ?? '-' }}
                        </td>
                        <td class="px-4 py-3 whitespace-nowrap text-sm text-gray-700 bg-blue-50/50">
                            @if($machine->modem)
                                <span class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-800">
                                    {{ $machine->modem->modem_number }}
                                </span>
                            @else
                                <span class="text-gray-400">-</span>
                            @endif
                        </td>
                        <td class="px-4 py-3 text-sm text-gray-700 max-w-xs">
                            <div class="truncate" title="{{ $machine->location }}{{ $machine->place ? ', ' . $machine->place : '' }}">
                                {{ $machine->location }}
                                @if($machine->place)
                                    <br><span class="text-xs text-gray-500">{{ $machine->place }}</span>
                                @endif
                            </div>
                        </td>
                        <td class="px-4 py-3 whitespace-nowrap text-sm text-gray-700 bg-blue-50/50">
                            {{ $machine->install_date ? $machine->install_date->format('d.m.Y') : '-' }}
                        </td>
                        <td class="px-4 py-3 whitespace-nowrap text-sm text-center">
                            <div class="flex items-center justify-center space-x-2">
                                <a href="{{ route('vending-machines.edit', $machine->id) }}"
                                   class="text-blue-600 hover:text-blue-800 p-1" title="Редактировать">
                                    <i class="fas fa-edit"></i>
                                </a>
                                @if($machine->modem)
                                    <form action="{{ route('vending-machines.detach-modem', $machine->id) }}" method="POST" class="inline"
                                          onsubmit="return confirm('Отвязать модем?')">
                                        @csrf
                                        <button type="submit" class="text-yellow-600 hover:text-yellow-800 p-1" title="Отвязать модем">
                                            <i class="fas fa-unlink"></i>
                                        </button>
                                    </form>
                                @endif
                                <form action="{{ route('vending-machines.destroy', $machine->id) }}" method="POST" class="inline"
                                      onsubmit="return confirm('Удалить торговый автомат?')">
                                    @csrf
                                    @method('DELETE')
                                    <button type="submit" class="text-red-600 hover:text-red-800 p-1" title="Удалить">
                                        <i class="fas fa-trash"></i>
                                    </button>
                                </form>
                            </div>
                        </td>
                    </tr>
                @empty
                    <tr>
                        <td colspan="8" class="px-4 py-8 text-center text-gray-500">
                            <i class="fas fa-inbox text-4xl mb-2"></i>
                            <p>Торговые автоматы не найдены</p>
                        </td>
                    </tr>
                @endforelse
            </tbody>
        </table>
    </div>

    <!-- Cards View -->
    <div x-show="viewMode === 'cards'" x-cloak class="p-6">
        @if($machines->count() > 0)
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
                @foreach($machines as $machine)
                    @php
                        $cardBorder = match($machine->status) {
                            'Работает', 'Working' => 'border-l-4 border-l-green-500',
                            'На обслуживании', 'Обслуживается', 'В ожидании' => 'border-l-4 border-l-yellow-500',
                            'Не работает', 'NotWorking', 'Остановлен' => 'border-l-4 border-l-red-500',
                            default => 'border-l-4 border-l-gray-300'
                        };
                        $statusBadge = match($machine->status) {
                            'Работает', 'Working' => 'bg-green-100 text-green-800',
                            'На обслуживании', 'Обслуживается', 'В ожидании' => 'bg-yellow-100 text-yellow-800',
                            'Не работает', 'NotWorking', 'Остановлен' => 'bg-red-100 text-red-800',
                            default => 'bg-gray-100 text-gray-800'
                        };
                    @endphp
                    <div class="bg-white rounded-lg shadow-sm border {{ $cardBorder }} hover:shadow-md transition-shadow">
                        <div class="p-4">
                            <!-- Header -->
                            <div class="flex items-start justify-between mb-3">
                                <div>
                                    <h3 class="font-semibold text-gray-800">{{ $machine->name }}</h3>
                                    <p class="text-xs text-gray-500">ID: {{ $machine->serial_number }}</p>
                                </div>
                                <span class="px-2 py-1 rounded-full text-xs font-medium {{ $statusBadge }}">
                                    {{ $machine->status }}
                                </span>
                            </div>

                            <!-- Info -->
                            <div class="space-y-2 text-sm">
                                <div class="flex items-center text-gray-600">
                                    <i class="fas fa-microchip w-5 text-gray-400"></i>
                                    <span>{{ $machine->model }}</span>
                                </div>
                                <div class="flex items-center text-gray-600">
                                    <i class="fas fa-building w-5 text-gray-400"></i>
                                    <span>{{ $machine->company ?? '-' }}</span>
                                </div>
                                <div class="flex items-start text-gray-600">
                                    <i class="fas fa-map-marker-alt w-5 text-gray-400 mt-0.5"></i>
                                    <span class="text-xs">{{ Str::limit($machine->location, 40) }}</span>
                                </div>
                                @if($machine->modem)
                                    <div class="flex items-center text-gray-600">
                                        <i class="fas fa-wifi w-5 text-gray-400"></i>
                                        <span class="text-xs bg-blue-100 text-blue-800 px-2 py-0.5 rounded">{{ $machine->modem->modem_number }}</span>
                                    </div>
                                @endif
                                <div class="flex items-center text-gray-600">
                                    <i class="fas fa-calendar w-5 text-gray-400"></i>
                                    <span>{{ $machine->install_date ? $machine->install_date->format('d.m.Y') : '-' }}</span>
                                </div>
                            </div>

                            <!-- Actions -->
                            <div class="flex items-center justify-end mt-4 pt-3 border-t border-gray-100 space-x-2">
                                <a href="{{ route('vending-machines.edit', $machine->id) }}"
                                   class="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition" title="Редактировать">
                                    <i class="fas fa-edit"></i>
                                </a>
                                @if($machine->modem)
                                    <form action="{{ route('vending-machines.detach-modem', $machine->id) }}" method="POST" class="inline"
                                          onsubmit="return confirm('Отвязать модем?')">
                                        @csrf
                                        <button type="submit" class="p-2 text-yellow-600 hover:bg-yellow-50 rounded-lg transition" title="Отвязать модем">
                                            <i class="fas fa-unlink"></i>
                                        </button>
                                    </form>
                                @endif
                                <form action="{{ route('vending-machines.destroy', $machine->id) }}" method="POST" class="inline"
                                      onsubmit="return confirm('Удалить торговый автомат?')">
                                    @csrf
                                    @method('DELETE')
                                    <button type="submit" class="p-2 text-red-600 hover:bg-red-50 rounded-lg transition" title="Удалить">
                                        <i class="fas fa-trash"></i>
                                    </button>
                                </form>
                            </div>
                        </div>
                    </div>
                @endforeach
            </div>
        @else
            <div class="text-center py-8 text-gray-500">
                <i class="fas fa-inbox text-4xl mb-2"></i>
                <p>Торговые автоматы не найдены</p>
            </div>
        @endif
    </div>

    <!-- Pagination -->
    @if($machines->hasPages())
        <div class="px-6 py-4 border-t border-gray-200 bg-gray-50">
            <div class="flex items-center justify-between">
                <div class="text-sm text-gray-600">
                    Записи с {{ $machines->firstItem() }} до {{ $machines->lastItem() }} из {{ $machines->total() }}
                </div>
                <div class="flex items-center space-x-1">
                    @if ($machines->onFirstPage())
                        <span class="px-3 py-1 text-gray-400 bg-gray-100 rounded cursor-not-allowed">
                            <i class="fas fa-chevron-left"></i>
                        </span>
                    @else
                        <a href="{{ $machines->previousPageUrl() }}" class="px-3 py-1 text-gray-700 bg-white border border-gray-300 rounded hover:bg-gray-50">
                            <i class="fas fa-chevron-left"></i>
                        </a>
                    @endif

                    @foreach ($machines->getUrlRange(max(1, $machines->currentPage() - 2), min($machines->lastPage(), $machines->currentPage() + 2)) as $page => $url)
                        @if ($page == $machines->currentPage())
                            <span class="px-3 py-1 text-white bg-blue-500 rounded">{{ $page }}</span>
                        @else
                            <a href="{{ $url }}" class="px-3 py-1 text-gray-700 bg-white border border-gray-300 rounded hover:bg-gray-50">{{ $page }}</a>
                        @endif
                    @endforeach

                    @if ($machines->hasMorePages())
                        <a href="{{ $machines->nextPageUrl() }}" class="px-3 py-1 text-gray-700 bg-white border border-gray-300 rounded hover:bg-gray-50">
                            <i class="fas fa-chevron-right"></i>
                        </a>
                    @else
                        <span class="px-3 py-1 text-gray-400 bg-gray-100 rounded cursor-not-allowed">
                            <i class="fas fa-chevron-right"></i>
                        </span>
                    @endif
                </div>
            </div>
        </div>
    @endif
</div>

@push('scripts')
<script>
function rentalManager() {
    return {
        // Modal states
        showRentalBanner: false,
        showModal: false,
        showBookingModal: false,
        showConfirmationModal: false,
        showContractModal: false,

        // Filters
        filterModel: '',
        filterStatus: '',
        sortBy: 'payback_asc',
        selectedMachines: [],

        // Booking form
        bookingForm: {
            startDate: '',
            endDate: '',
            franchiseType: 'self',
            insurance: false
        },

        // States
        isSubmitting: false,
        isSigningContract: false,
        confirmationStep: 'loading',
        confirmationError: '',
        generatedContractNumber: '',
        signatureData: null,
        signaturePad: null,

        // Audit log
        auditLog: [],

        // Demo data for available machines
        availableMachines: [
            { id: 1, name: 'Снековый автомат SM-100', model: 'SM-100', monthlyRent: 15000, yearlyRent: 162000, paybackMonths: 8, location: 'ТЦ "Мега", 1 этаж, у главного входа', status: 'available' },
            { id: 2, name: 'Кофейный автомат CF-200', model: 'CF-200', monthlyRent: 25000, yearlyRent: 270000, paybackMonths: 6, location: 'БЦ "Премиум", холл, 2 этаж', status: 'available' },
            { id: 3, name: 'Комбинированный автомат CB-300', model: 'CB-300', monthlyRent: 35000, yearlyRent: 378000, paybackMonths: 10, location: 'Аэропорт Шереметьево, терминал B', status: 'reserved' },
            { id: 4, name: 'Снековый автомат SM-100', model: 'SM-100', monthlyRent: 14000, yearlyRent: 151200, paybackMonths: 9, location: 'Университет МГТУ, главный корпус', status: 'available' },
            { id: 5, name: 'Кофейный автомат CF-200', model: 'CF-200', monthlyRent: 22000, yearlyRent: 237600, paybackMonths: 7, location: 'Офисный центр "Сити", лобби', status: 'available' },
            { id: 6, name: 'Автомат напитков DR-150', model: 'DR-150', monthlyRent: 18000, yearlyRent: 194400, paybackMonths: 5, location: 'Фитнес-клуб "Спорт Лайф"', status: 'available' },
            { id: 7, name: 'Комбинированный автомат CB-300', model: 'CB-300', monthlyRent: 32000, yearlyRent: 345600, paybackMonths: 11, location: 'ТРЦ "Европейский", фуд-корт', status: 'reserved' },
            { id: 8, name: 'Автомат напитков DR-150', model: 'DR-150', monthlyRent: 16500, yearlyRent: 178200, paybackMonths: 6, location: 'Станция метро "Киевская"', status: 'available' }
        ],

        get todayDate() {
            return new Date().toISOString().split('T')[0];
        },

        init() {
            const hasRented = localStorage.getItem('hasRentedMachines');
            const bannerDismissed = localStorage.getItem('rentalBannerDismissed');
            const dismissedAt = localStorage.getItem('rentalBannerDismissedAt');

            if (!hasRented) {
                if (!bannerDismissed) {
                    this.showRentalBanner = true;
                } else if (dismissedAt) {
                    const dismissedTime = new Date(dismissedAt);
                    const hoursSinceDismissed = (new Date() - dismissedTime) / (1000 * 60 * 60);
                    if (hoursSinceDismissed > 24) {
                        this.showRentalBanner = true;
                        localStorage.removeItem('rentalBannerDismissed');
                    }
                }
            }

            // Load audit log
            this.auditLog = JSON.parse(localStorage.getItem('auditLog') || '[]');

            // Set default dates
            const today = new Date();
            const nextMonth = new Date(today);
            nextMonth.setMonth(nextMonth.getMonth() + 1);
            this.bookingForm.startDate = today.toISOString().split('T')[0];
            this.bookingForm.endDate = nextMonth.toISOString().split('T')[0];

            document.addEventListener('open-rental-modal', () => this.openRentalModal());

            this.logAction('page_view', 'Просмотр страницы торговых автоматов');
        },

        // Logging
        logAction(action, description, data = {}) {
            const logEntry = {
                id: Date.now(),
                timestamp: new Date().toISOString(),
                action,
                description,
                data,
                user: '{{ session('user.email', 'anonymous') }}'
            };
            this.auditLog.unshift(logEntry);
            if (this.auditLog.length > 100) this.auditLog = this.auditLog.slice(0, 100);
            localStorage.setItem('auditLog', JSON.stringify(this.auditLog));
            console.log('[AUDIT]', logEntry);
        },

        get uniqueModels() {
            return [...new Set(this.availableMachines.map(m => m.model))];
        },

        get filteredMachines() {
            let result = [...this.availableMachines];
            if (this.filterModel) result = result.filter(m => m.model === this.filterModel);
            if (this.filterStatus) result = result.filter(m => m.status === this.filterStatus);

            switch (this.sortBy) {
                case 'payback_asc': result.sort((a, b) => a.paybackMonths - b.paybackMonths); break;
                case 'payback_desc': result.sort((a, b) => b.paybackMonths - a.paybackMonths); break;
                case 'price_asc': result.sort((a, b) => a.monthlyRent - b.monthlyRent); break;
                case 'price_desc': result.sort((a, b) => b.monthlyRent - a.monthlyRent); break;
            }
            return result;
        },

        get totalMonthlyRent() {
            return this.selectedMachines.reduce((sum, id) => {
                const machine = this.availableMachines.find(m => m.id === id);
                return sum + (machine ? machine.monthlyRent : 0);
            }, 0);
        },

        get totalYearlyRent() {
            return this.selectedMachines.reduce((sum, id) => {
                const machine = this.availableMachines.find(m => m.id === id);
                return sum + (machine ? machine.yearlyRent : 0);
            }, 0);
        },

        formatPrice(price) {
            return new Intl.NumberFormat('ru-RU').format(price);
        },

        formatDateRu(date) {
            if (!date || isNaN(date)) return '';
            return date.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' });
        },

        isSelected(id) {
            return this.selectedMachines.includes(id);
        },

        getMachineName(id) {
            const machine = this.availableMachines.find(m => m.id === id);
            return machine ? machine.name : '';
        },

        toggleSelection(machine) {
            if (machine.status === 'reserved') return;
            const index = this.selectedMachines.indexOf(machine.id);
            if (index === -1) {
                this.selectedMachines.push(machine.id);
                this.logAction('machine_selected', `Выбран ТА: ${machine.name}`, { machineId: machine.id });
            } else {
                this.selectedMachines.splice(index, 1);
                this.logAction('machine_deselected', `Снят выбор ТА: ${machine.name}`, { machineId: machine.id });
            }
        },

        calculateTotal() {
            let total = this.totalMonthlyRent;
            if (this.bookingForm.franchiseType === 'managed') total *= 1.1;
            if (this.bookingForm.franchiseType === 'full') total *= 1.25;
            if (this.bookingForm.insurance) total *= 1.05;
            return Math.round(total);
        },

        getFranchiseTypeName() {
            const types = { 'self': 'Самостоятельное управление', 'managed': 'С поддержкой франчайзера', 'full': 'Полное управление франчайзером' };
            return types[this.bookingForm.franchiseType] || '';
        },

        isBookingFormValid() {
            return this.bookingForm.startDate && this.bookingForm.endDate &&
                   this.bookingForm.franchiseType && this.selectedMachines.length > 0 &&
                   new Date(this.bookingForm.endDate) > new Date(this.bookingForm.startDate);
        },

        openRentalModal() {
            this.showModal = true;
            document.body.style.overflow = 'hidden';
            this.logAction('modal_opened', 'Открыто окно выбора ТА');
        },

        closeModal() {
            this.showModal = false;
            document.body.style.overflow = '';
        },

        dismissBanner() {
            this.showRentalBanner = false;
            localStorage.setItem('rentalBannerDismissed', 'true');
            localStorage.setItem('rentalBannerDismissedAt', new Date().toISOString());
            this.logAction('banner_dismissed', 'Баннер аренды скрыт');
        },

        proceedToBooking() {
            if (this.selectedMachines.length === 0) return;
            this.showModal = false;
            this.showBookingModal = true;
            this.logAction('booking_form_opened', 'Открыта форма бронирования', { selectedCount: this.selectedMachines.length });
        },

        closeBookingModal() {
            this.showBookingModal = false;
            this.showModal = true;
        },

        async submitBooking() {
            if (this.isSubmitting || !this.isBookingFormValid()) return;

            this.isSubmitting = true;
            this.logAction('booking_submitted', 'Отправлена заявка на бронирование', {
                machines: this.selectedMachines,
                form: this.bookingForm,
                total: this.calculateTotal()
            });

            this.showBookingModal = false;
            this.showConfirmationModal = true;
            this.confirmationStep = 'loading';

            // Simulate franchisor confirmation (2-4 seconds)
            await new Promise(resolve => setTimeout(resolve, 2000 + Math.random() * 2000));

            // 90% success rate simulation
            if (Math.random() > 0.1) {
                this.generatedContractNumber = 'FR-' + new Date().getFullYear() + '-' + String(Math.floor(Math.random() * 10000)).padStart(4, '0');
                this.confirmationStep = 'success';

                // Mark machines as reserved
                this.selectedMachines.forEach(id => {
                    const machine = this.availableMachines.find(m => m.id === id);
                    if (machine) machine.status = 'reserved';
                });

                // Save contract
                const contracts = JSON.parse(localStorage.getItem('contracts') || '[]');
                contracts.push({
                    number: this.generatedContractNumber,
                    signDate: null,
                    startDate: this.bookingForm.startDate,
                    endDate: this.bookingForm.endDate,
                    status: 'unsigned',
                    machines: this.selectedMachines.map(id => this.availableMachines.find(m => m.id === id)),
                    franchiseType: this.bookingForm.franchiseType,
                    insurance: this.bookingForm.insurance,
                    monthlyTotal: this.calculateTotal(),
                    createdAt: new Date().toISOString()
                });
                localStorage.setItem('contracts', JSON.stringify(contracts));

                this.logAction('booking_confirmed', 'Бронирование подтверждено франчайзером', { contractNumber: this.generatedContractNumber });
                if (typeof showToast === 'function') showToast('Бронирование успешно подтверждено!', 'success');
            } else {
                this.confirmationStep = 'error';
                this.confirmationError = 'Один или несколько ТА были забронированы другим пользователем';
                this.logAction('booking_failed', 'Ошибка бронирования', { error: this.confirmationError });
                if (typeof showToast === 'function') showToast('Ошибка бронирования', 'error');
            }

            this.isSubmitting = false;
        },

        closeConfirmationModal() {
            this.showConfirmationModal = false;
            this.showRentalBanner = false;
            localStorage.setItem('hasRentedMachines', 'true');
            this.selectedMachines = [];
        },

        goToContract() {
            this.showConfirmationModal = false;
            this.showContractModal = true;
            this.signatureData = null;
            this.logAction('contract_opened', 'Открыт договор для подписания', { contractNumber: this.generatedContractNumber });

            this.$nextTick(() => this.initSignaturePad());
        },

        closeContractModal() {
            this.showContractModal = false;
            this.selectedMachines = [];
        },

        initSignaturePad() {
            const canvas = document.getElementById('signaturePad');
            if (!canvas) return;

            const ctx = canvas.getContext('2d');
            let isDrawing = false;
            let lastX = 0, lastY = 0;

            ctx.strokeStyle = '#1f2937';
            ctx.lineWidth = 2;
            ctx.lineCap = 'round';
            ctx.lineJoin = 'round';

            const getPos = (e) => {
                const rect = canvas.getBoundingClientRect();
                const scaleX = canvas.width / rect.width;
                const scaleY = canvas.height / rect.height;
                if (e.touches) {
                    return { x: (e.touches[0].clientX - rect.left) * scaleX, y: (e.touches[0].clientY - rect.top) * scaleY };
                }
                return { x: (e.clientX - rect.left) * scaleX, y: (e.clientY - rect.top) * scaleY };
            };

            const startDrawing = (e) => { isDrawing = true; const pos = getPos(e); lastX = pos.x; lastY = pos.y; };
            const draw = (e) => {
                if (!isDrawing) return;
                e.preventDefault();
                const pos = getPos(e);
                ctx.beginPath();
                ctx.moveTo(lastX, lastY);
                ctx.lineTo(pos.x, pos.y);
                ctx.stroke();
                lastX = pos.x; lastY = pos.y;
            };
            const stopDrawing = () => { isDrawing = false; };

            canvas.addEventListener('mousedown', startDrawing);
            canvas.addEventListener('mousemove', draw);
            canvas.addEventListener('mouseup', stopDrawing);
            canvas.addEventListener('mouseout', stopDrawing);
            canvas.addEventListener('touchstart', startDrawing);
            canvas.addEventListener('touchmove', draw);
            canvas.addEventListener('touchend', stopDrawing);
        },

        clearSignature() {
            const canvas = document.getElementById('signaturePad');
            if (canvas) {
                const ctx = canvas.getContext('2d');
                ctx.clearRect(0, 0, canvas.width, canvas.height);
            }
            this.signatureData = null;
            this.logAction('signature_cleared', 'Подпись очищена');
        },

        saveSignature() {
            const canvas = document.getElementById('signaturePad');
            if (canvas) {
                this.signatureData = canvas.toDataURL('image/png');
                this.logAction('signature_saved', 'Подпись сохранена');
                if (typeof showToast === 'function') showToast('Подпись сохранена', 'info');
            }
        },

        async signContract() {
            if (!this.signatureData || this.isSigningContract) return;

            this.isSigningContract = true;
            this.logAction('contract_signing', 'Начато подписание договора', { contractNumber: this.generatedContractNumber });

            // Simulate signing process with encryption
            await new Promise(resolve => setTimeout(resolve, 1500));

            // Update contract
            const contracts = JSON.parse(localStorage.getItem('contracts') || '[]');
            const contract = contracts.find(c => c.number === this.generatedContractNumber);
            if (contract) {
                contract.status = 'signed';
                contract.signDate = new Date().toISOString();
                contract.signature = this.signatureData;
                // Simple "encryption" simulation - in real app use proper encryption
                contract.encryptedHash = btoa(JSON.stringify({ contractNumber: contract.number, signDate: contract.signDate, signature: this.signatureData.substring(0, 50) }));
                localStorage.setItem('contracts', JSON.stringify(contracts));
            }

            this.isSigningContract = false;
            this.logAction('contract_signed', 'Договор подписан и зашифрован', { contractNumber: this.generatedContractNumber });

            if (typeof showToast === 'function') showToast('Договор успешно подписан!', 'success');

            this.showContractModal = false;
            this.showRentalBanner = false;
            localStorage.setItem('hasRentedMachines', 'true');
            this.selectedMachines = [];

            // Redirect to contracts page after short delay
            setTimeout(() => {
                if (typeof showToast === 'function') showToast('Перейдите в раздел "Договоры" для просмотра', 'info');
            }, 1000);
        }
    };
}
</script>
@endpush
@endsection
