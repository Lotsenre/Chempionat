<!DOCTYPE html>
<html lang="{{ app()->getLocale() }}">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>{{ __('Вход') }} - Вендинг Система</title>
    <script src="https://cdn.tailwindcss.com"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
</head>
<body class="bg-gradient-to-br from-blue-500 to-purple-600 min-h-screen flex items-center justify-center p-4">
    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-md p-8">
        <!-- Logo -->
        <div class="text-center mb-8">
            <img src="{{ asset('images/logo.png') }}" alt="Джутсу Вендинг" class="h-20 mx-auto mb-4">
            <p class="text-gray-500 text-sm">{{ __('Войдите в свой аккаунт') }}</p>
        </div>

        <!-- Errors -->
        @if($errors->any())
            <div class="mb-4 p-4 bg-red-100 border border-red-400 text-red-700 rounded-lg text-sm">
                @foreach($errors->all() as $error)
                    <p>{{ $error }}</p>
                @endforeach
            </div>
        @endif

        <!-- Form -->
        <form method="POST" action="{{ route('login.submit') }}" class="space-y-6">
            @csrf

            <!-- Email -->
            <div>
                <label for="email" class="block text-sm font-medium text-gray-700 mb-1">
                    {{ __('Email') }}
                </label>
                <div class="relative">
                    <input type="email" name="email" id="email" value="{{ old('email') }}"
                           class="w-full border border-gray-300 rounded-lg pl-10 pr-4 py-3 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition"
                           placeholder="admin@example.com" required autofocus>
                    <i class="fas fa-envelope absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"></i>
                </div>
            </div>

            <!-- Password -->
            <div>
                <label for="password" class="block text-sm font-medium text-gray-700 mb-1">
                    {{ __('Пароль') }}
                </label>
                <div class="relative">
                    <input type="password" name="password" id="password"
                           class="w-full border border-gray-300 rounded-lg pl-10 pr-4 py-3 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition"
                           placeholder="••••••••" required>
                    <i class="fas fa-lock absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"></i>
                </div>
            </div>

            <!-- Remember Me -->
            <div class="flex items-center justify-between">
                <label class="flex items-center">
                    <input type="checkbox" name="remember" class="rounded border-gray-300 text-blue-600 focus:ring-blue-500">
                    <span class="ml-2 text-sm text-gray-600">{{ __('Запомнить меня') }}</span>
                </label>
                <a href="#" class="text-sm text-blue-600 hover:text-blue-800">{{ __('Забыли пароль?') }}</a>
            </div>

            <!-- Submit -->
            <button type="submit"
                    class="w-full bg-blue-600 text-white py-3 rounded-lg font-medium hover:bg-blue-700 transition flex items-center justify-center">
                <i class="fas fa-sign-in-alt mr-2"></i>
                {{ __('Войти') }}
            </button>
        </form>

        <!-- Register Link -->
        <p class="text-center text-sm text-gray-500 mt-6">
            {{ __('Нет аккаунта?') }}
            <a href="{{ route('register') }}" class="text-blue-600 hover:text-blue-800 font-medium">{{ __('Зарегистрироваться') }}</a>
        </p>

    </div>
</body>
</html>
