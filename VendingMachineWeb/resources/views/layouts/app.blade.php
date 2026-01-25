<!DOCTYPE html>
<html lang="{{ app()->getLocale() }}">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="csrf-token" content="{{ csrf_token() }}">
    <title>@yield('title', 'Главная') - Джутсу Вендинг</title>

    <!-- Tailwind CSS CDN -->
    <script src="https://cdn.tailwindcss.com"></script>

    <!-- Font Awesome for icons -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">

    <!-- Google Fonts -->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">

    <script>
        tailwind.config = {
            theme: {
                extend: {
                    fontFamily: {
                        sans: ['Inter', 'sans-serif'],
                    },
                    colors: {
                        primary: {
                            50: '#eff6ff',
                            100: '#dbeafe',
                            200: '#bfdbfe',
                            300: '#93c5fd',
                            400: '#60a5fa',
                            500: '#3b82f6',
                            600: '#2563eb',
                            700: '#1d4ed8',
                            800: '#1e40af',
                            900: '#1e3a8a',
                        },
                        sidebar: {
                            dark: '#1e293b',
                            darker: '#0f172a',
                            light: '#334155',
                        }
                    }
                }
            }
        }
    </script>

    <style>
        [x-cloak] { display: none !important; }

        /* Custom scrollbar for sidebar */
        .sidebar-scroll::-webkit-scrollbar {
            width: 4px;
        }
        .sidebar-scroll::-webkit-scrollbar-track {
            background: transparent;
        }
        .sidebar-scroll::-webkit-scrollbar-thumb {
            background: rgba(255, 255, 255, 0.2);
            border-radius: 4px;
        }
        .sidebar-scroll::-webkit-scrollbar-thumb:hover {
            background: rgba(255, 255, 255, 0.3);
        }

        /* Sidebar link hover effect */
        .sidebar-link {
            position: relative;
            transition: all 0.2s ease;
        }
        .sidebar-link::before {
            content: '';
            position: absolute;
            left: 0;
            top: 0;
            height: 100%;
            width: 3px;
            background: transparent;
            border-radius: 0 4px 4px 0;
            transition: all 0.2s ease;
        }
        .sidebar-link:hover::before,
        .sidebar-link.active::before {
            background: #3b82f6;
        }
        .sidebar-link.active {
            background: rgba(59, 130, 246, 0.15);
        }

        /* Gradient text for logo */
        .gradient-text {
            background: linear-gradient(135deg, #3b82f6 0%, #8b5cf6 100%);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
        }
    </style>

    <!-- Notification System Script -->
    <script>
        // Toast Notification Function (available globally)
        function showToast(message, type = 'info', duration = 5000) {
            const container = document.getElementById('toast-container');
            if (!container) return;

            const icons = {
                success: 'fa-check-circle',
                error: 'fa-times-circle',
                warning: 'fa-exclamation-triangle',
                info: 'fa-info-circle'
            };

            const colors = {
                success: 'bg-green-500',
                error: 'bg-red-500',
                warning: 'bg-yellow-500',
                info: 'bg-blue-500'
            };

            const toast = document.createElement('div');
            toast.className = `flex items-center p-4 rounded-lg shadow-lg text-white ${colors[type] || colors.info} transform translate-x-full transition-transform duration-300`;
            toast.innerHTML = `
                <i class="fas ${icons[type] || icons.info} mr-3 text-lg"></i>
                <span class="flex-1">${message}</span>
                <button onclick="this.parentElement.remove()" class="ml-3 hover:opacity-75">
                    <i class="fas fa-times"></i>
                </button>
            `;

            container.appendChild(toast);

            // Trigger animation
            setTimeout(() => toast.classList.remove('translate-x-full'), 10);

            // Auto remove
            setTimeout(() => {
                toast.classList.add('translate-x-full');
                setTimeout(() => toast.remove(), 300);
            }, duration);

            // Save to notification center
            window.dispatchEvent(new CustomEvent('new-notification', {
                detail: { type, message }
            }));
        }

        // Helper function to format time
        function formatNotificationTime(date) {
            const now = new Date();
            const diff = now - date;
            const minutes = Math.floor(diff / 60000);
            const hours = Math.floor(diff / 3600000);
            const days = Math.floor(diff / 86400000);

            if (minutes < 1) return 'Только что';
            if (minutes < 60) return minutes + ' мин. назад';
            if (hours < 24) return hours + ' ч. назад';
            if (days < 7) return days + ' дн. назад';

            return date.toLocaleDateString('ru-RU');
        }
    </script>

    <!-- Alpine.js for interactivity -->
    <script defer src="https://cdn.jsdelivr.net/npm/alpinejs@3.x.x/dist/cdn.min.js"></script>
</head>
<body class="bg-gray-100 min-h-screen font-sans">
    <div class="flex h-screen overflow-hidden">
        <!-- Sidebar -->
        <aside class="w-64 bg-gradient-to-b from-sidebar-dark to-sidebar-darker flex-shrink-0 flex flex-col shadow-xl">
            <!-- Logo Section -->
            <div class="h-20 flex items-center justify-center px-4 border-b border-white/10 bg-white/5">
                <a href="{{ route('dashboard') }}" class="block">
                    <img src="{{ asset('images/logo.png') }}" alt="Джутсу Вендинг" class="h-14 w-auto">
                </a>
            </div>

            <!-- Navigation -->
            <nav class="flex-1 overflow-y-auto sidebar-scroll py-4 px-3">
                <!-- Main Section -->
                <div class="mb-6">
                    <p class="px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3">Главное</p>

                    <a href="{{ route('dashboard') }}"
                       class="sidebar-link flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-white/5 rounded-lg mb-1 {{ request()->routeIs('dashboard') ? 'active text-white' : '' }}">
                        <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-blue-500/20 to-blue-600/20 flex items-center justify-center mr-3">
                            <i class="fas fa-home text-blue-400"></i>
                        </div>
                        <span class="font-medium">{{ __('Главная') }}</span>
                    </a>

                    <a href="{{ route('vending-machines.index') }}"
                       class="sidebar-link flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-white/5 rounded-lg mb-1 {{ request()->routeIs('vending-machines.*') ? 'active text-white' : '' }}">
                        <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-emerald-500/20 to-emerald-600/20 flex items-center justify-center mr-3">
                            <i class="fas fa-cash-register text-emerald-400"></i>
                        </div>
                        <span class="font-medium">{{ __('Торговые автоматы') }}</span>
                        <span class="ml-auto bg-emerald-500/20 text-emerald-400 text-xs font-semibold px-2 py-0.5 rounded-full">9</span>
                    </a>
                </div>

                <!-- Management Section -->
                <div class="mb-6">
                    <p class="px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3">Управление</p>

                    <a href="#" class="sidebar-link flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-white/5 rounded-lg mb-1">
                        <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-violet-500/20 to-violet-600/20 flex items-center justify-center mr-3">
                            <i class="fas fa-building text-violet-400"></i>
                        </div>
                        <span class="font-medium">{{ __('Компании') }}</span>
                    </a>

                    <a href="#" class="sidebar-link flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-white/5 rounded-lg mb-1">
                        <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-amber-500/20 to-amber-600/20 flex items-center justify-center mr-3">
                            <i class="fas fa-chart-line text-amber-400"></i>
                        </div>
                        <span class="font-medium">{{ __('Мониторинг') }}</span>
                    </a>

                    <a href="{{ route('contracts.index') }}"
                       class="sidebar-link flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-white/5 rounded-lg mb-1 {{ request()->routeIs('contracts.*') ? 'active text-white' : '' }}">
                        <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-rose-500/20 to-rose-600/20 flex items-center justify-center mr-3">
                            <i class="fas fa-file-contract text-rose-400"></i>
                        </div>
                        <span class="font-medium">{{ __('Договоры') }}</span>
                    </a>

                    <a href="#" class="sidebar-link flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-white/5 rounded-lg mb-1">
                        <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-cyan-500/20 to-cyan-600/20 flex items-center justify-center mr-3">
                            <i class="fas fa-box text-cyan-400"></i>
                        </div>
                        <span class="font-medium">{{ __('Товары') }}</span>
                    </a>
                </div>

                <!-- Reports Section -->
                <div class="mb-6">
                    <p class="px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3">Отчёты</p>

                    <a href="#" class="sidebar-link flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-white/5 rounded-lg mb-1">
                        <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-indigo-500/20 to-indigo-600/20 flex items-center justify-center mr-3">
                            <i class="fas fa-chart-pie text-indigo-400"></i>
                        </div>
                        <span class="font-medium">{{ __('Продажи') }}</span>
                    </a>

                    <a href="#" class="sidebar-link flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-white/5 rounded-lg mb-1">
                        <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-teal-500/20 to-teal-600/20 flex items-center justify-center mr-3">
                            <i class="fas fa-wrench text-teal-400"></i>
                        </div>
                        <span class="font-medium">{{ __('Обслуживание') }}</span>
                    </a>
                </div>

                <!-- System Section -->
                <div class="mb-6">
                    <p class="px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3">Система</p>

                    <a href="#" class="sidebar-link flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-white/5 rounded-lg mb-1">
                        <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-gray-500/20 to-gray-600/20 flex items-center justify-center mr-3">
                            <i class="fas fa-cog text-gray-400"></i>
                        </div>
                        <span class="font-medium">{{ __('Настройки') }}</span>
                    </a>

                    <form method="POST" action="{{ route('logout') }}">
                        @csrf
                        <button type="submit" class="w-full sidebar-link flex items-center px-4 py-3 text-gray-300 hover:text-white hover:bg-red-500/10 rounded-lg">
                            <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-red-500/20 to-red-600/20 flex items-center justify-center mr-3">
                                <i class="fas fa-sign-out-alt text-red-400"></i>
                            </div>
                            <span class="font-medium">{{ __('Выход') }}</span>
                        </button>
                    </form>
                </div>
            </nav>
        </aside>

        <!-- Main Content -->
        <div class="flex-1 flex flex-col overflow-hidden">
            <!-- Top Header -->
            <header class="h-16 bg-white shadow-sm flex items-center justify-between px-6 flex-shrink-0">
                <div class="flex items-center space-x-4">
                    <!-- Breadcrumb -->
                    <nav class="flex items-center space-x-2 text-sm">
                        <a href="{{ route('dashboard') }}" class="text-gray-500 hover:text-gray-700">
                            <i class="fas fa-home"></i>
                        </a>
                        <i class="fas fa-chevron-right text-gray-300 text-xs"></i>
                        <span class="text-gray-700 font-medium">@yield('title', 'Главная')</span>
                    </nav>
                </div>

                <div class="flex items-center space-x-4">
                    <!-- Notification Center -->
                    <div class="relative" x-data="{
                        open: false,
                        notifications: [],
                        unreadCount: 0,
                        init() {
                            this.loadNotifications();
                            window.addEventListener('new-notification', (e) => this.addNotification(e.detail));
                        },
                        loadNotifications() {
                            try {
                                const stored = localStorage.getItem('notifications');
                                let items = stored ? JSON.parse(stored) : [];
                                // Удаляем дубликаты по ID
                                const seen = new Set();
                                this.notifications = items.filter(n => {
                                    if (seen.has(n.id)) return false;
                                    seen.add(n.id);
                                    return true;
                                });
                                this.unreadCount = this.notifications.filter(n => !n.read).length;
                                // Сохраняем очищенные данные
                                if (items.length !== this.notifications.length) {
                                    this.saveNotifications();
                                }
                            } catch (e) {
                                this.notifications = [];
                                this.unreadCount = 0;
                            }
                        },
                        saveNotifications() {
                            localStorage.setItem('notifications', JSON.stringify(this.notifications));
                            this.unreadCount = this.notifications.filter(n => !n.read).length;
                        },
                        addNotification(data) {
                            const id = Date.now().toString() + Math.random().toString(36).substr(2, 9);
                            this.notifications.unshift({
                                id: id,
                                type: data.type || 'info',
                                message: data.message,
                                time: formatNotificationTime(new Date()),
                                read: false
                            });
                            if (this.notifications.length > 50) this.notifications = this.notifications.slice(0, 50);
                            this.saveNotifications();
                        },
                        markAsRead(id) {
                            const n = this.notifications.find(x => x.id === id);
                            if (n) { n.read = true; this.saveNotifications(); }
                        },
                        removeNotification(id) {
                            this.notifications = this.notifications.filter(n => n.id !== id);
                            this.saveNotifications();
                        },
                        clearAll() {
                            this.notifications = [];
                            this.saveNotifications();
                        }
                    }">
                        <button @click="open = !open" class="relative p-2 text-gray-500 hover:text-gray-700 hover:bg-gray-100 rounded-lg transition">
                            <i class="fas fa-bell text-xl"></i>
                            <span x-show="unreadCount > 0" x-text="unreadCount > 9 ? '9+' : unreadCount"
                                  class="absolute -top-1 -right-1 min-w-[18px] h-[18px] bg-red-500 text-white text-xs font-bold rounded-full flex items-center justify-center px-1"></span>
                        </button>

                        <!-- Notifications Dropdown -->
                        <div x-show="open" @click.away="open = false" x-cloak
                             x-transition:enter="transition ease-out duration-100"
                             x-transition:enter-start="transform opacity-0 scale-95"
                             x-transition:enter-end="transform opacity-100 scale-100"
                             class="absolute right-0 mt-2 w-80 bg-white rounded-xl shadow-lg z-50 border border-gray-100 overflow-hidden">

                            <!-- Header -->
                            <div class="px-4 py-3 bg-gray-50 border-b border-gray-100 flex items-center justify-between">
                                <h3 class="font-semibold text-gray-800">Уведомления</h3>
                                <button @click="clearAll()" x-show="notifications.length > 0"
                                        class="text-xs text-gray-500 hover:text-red-600 transition">
                                    Очистить все
                                </button>
                            </div>

                            <!-- Notifications List -->
                            <div class="max-h-80 overflow-y-auto">
                                <template x-if="notifications.length === 0">
                                    <div class="px-4 py-8 text-center text-gray-500">
                                        <i class="fas fa-bell-slash text-3xl mb-2"></i>
                                        <p class="text-sm">Нет уведомлений</p>
                                    </div>
                                </template>

                                <template x-for="(notification, index) in notifications" :key="index">
                                    <div class="px-4 py-3 border-b border-gray-50 hover:bg-gray-50 transition cursor-pointer"
                                         :class="{ 'bg-blue-50/50': !notification.read }"
                                         @click="markAsRead(notification.id)">
                                        <div class="flex items-start space-x-3">
                                            <div class="flex-shrink-0 mt-0.5">
                                                <div :class="{
                                                    'bg-green-100 text-green-600': notification.type === 'success',
                                                    'bg-red-100 text-red-600': notification.type === 'error',
                                                    'bg-blue-100 text-blue-600': notification.type === 'info',
                                                    'bg-yellow-100 text-yellow-600': notification.type === 'warning'
                                                }" class="w-8 h-8 rounded-full flex items-center justify-center">
                                                    <i :class="{
                                                        'fas fa-check': notification.type === 'success',
                                                        'fas fa-times': notification.type === 'error',
                                                        'fas fa-info': notification.type === 'info',
                                                        'fas fa-exclamation': notification.type === 'warning'
                                                    }"></i>
                                                </div>
                                            </div>
                                            <div class="flex-1 min-w-0">
                                                <p class="text-sm text-gray-800" x-text="notification.message"></p>
                                                <p class="text-xs text-gray-400 mt-1" x-text="notification.time"></p>
                                            </div>
                                            <button @click.stop="removeNotification(notification.id)"
                                                    class="text-gray-400 hover:text-red-500 transition">
                                                <i class="fas fa-times text-xs"></i>
                                            </button>
                                        </div>
                                    </div>
                                </template>
                            </div>
                        </div>
                    </div>

                    <!-- User Menu -->
                    <div class="relative" x-data="{ open: false }">
                        <button @click="open = !open" class="flex items-center space-x-2 p-2 hover:bg-gray-100 rounded-lg transition">
                            <div class="w-8 h-8 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center text-white text-sm font-semibold">
                                {{ mb_substr(session('user.name', 'U'), 0, 1) }}
                            </div>
                            <span class="text-sm text-gray-700 hidden sm:block">{{ session('user.name', 'User') }}</span>
                            <i class="fas fa-chevron-down text-gray-400 text-xs"></i>
                        </button>

                        <div x-show="open" @click.away="open = false" x-cloak
                             x-transition:enter="transition ease-out duration-100"
                             x-transition:enter-start="transform opacity-0 scale-95"
                             x-transition:enter-end="transform opacity-100 scale-100"
                             x-transition:leave="transition ease-in duration-75"
                             x-transition:leave-start="transform opacity-100 scale-100"
                             x-transition:leave-end="transform opacity-0 scale-95"
                             class="absolute right-0 mt-2 w-48 bg-white rounded-xl shadow-lg py-2 z-50 border border-gray-100">
                            <a href="#" class="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-50">
                                <i class="fas fa-user w-5 text-gray-400"></i>
                                {{ __('Профиль') }}
                            </a>
                            <a href="#" class="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-50">
                                <i class="fas fa-cog w-5 text-gray-400"></i>
                                {{ __('Настройки') }}
                            </a>
                            <hr class="my-2 border-gray-100">
                            <form method="POST" action="{{ route('logout') }}">
                                @csrf
                                <button type="submit" class="flex items-center w-full px-4 py-2 text-sm text-red-600 hover:bg-red-50">
                                    <i class="fas fa-sign-out-alt w-5"></i>
                                    {{ __('Выйти') }}
                                </button>
                            </form>
                        </div>
                    </div>
                </div>
            </header>

            <!-- Main Content Area -->
            <main class="flex-1 overflow-y-auto p-6 bg-gray-50">
                @yield('content')
            </main>
        </div>
    </div>

    <!-- Toast Container -->
    <div id="toast-container" class="fixed top-4 right-4 z-50 space-y-2"></div>

    <!-- Flash Messages Script -->
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            // Debug: проверка что скрипт загружен
            console.log('Notification system loaded');
            console.log('localStorage notifications:', localStorage.getItem('notifications'));

            @if(session('success'))
                showToast("{{ session('success') }}", 'success');
            @endif

            @if(session('error'))
                showToast("{{ session('error') }}", 'error');
            @endif

            // Тестовое уведомление для проверки (удалить после отладки)
            // showToast("Тестовое уведомление", 'info');
        });
    </script>

    @stack('scripts')
</body>
</html>
