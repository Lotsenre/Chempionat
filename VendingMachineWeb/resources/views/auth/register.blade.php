<!DOCTYPE html>
<html lang="{{ app()->getLocale() }}">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="csrf-token" content="{{ csrf_token() }}">
    <title>{{ __('Регистрация') }} - Вендинг Система</title>
    <script src="https://cdn.tailwindcss.com"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
</head>
<body class="bg-gradient-to-br from-green-500 to-blue-600 min-h-screen flex items-center justify-center p-4">
    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-md p-8">
        <!-- Logo -->
        <div class="text-center mb-8">
            <img src="{{ asset('images/logo.png') }}" alt="Джутсу Вендинг" class="h-20 mx-auto mb-4">
            <h1 class="text-2xl font-bold text-gray-800">{{ __('Регистрация') }}</h1>
            <p class="text-gray-500 text-sm mt-1" id="step-description">{{ __('Создайте новый аккаунт') }}</p>
        </div>

        <!-- Progress Steps -->
        <div class="flex justify-center mb-6">
            <div class="flex items-center space-x-2">
                <div id="step-1-indicator" class="w-8 h-8 rounded-full bg-green-600 text-white flex items-center justify-center text-sm font-bold">1</div>
                <div class="w-8 h-1 bg-gray-300" id="progress-1"></div>
                <div id="step-2-indicator" class="w-8 h-8 rounded-full bg-gray-300 text-gray-600 flex items-center justify-center text-sm font-bold">2</div>
                <div class="w-8 h-1 bg-gray-300" id="progress-2"></div>
                <div id="step-3-indicator" class="w-8 h-8 rounded-full bg-gray-300 text-gray-600 flex items-center justify-center text-sm font-bold">3</div>
            </div>
        </div>

        <!-- Errors -->
        <div id="error-container" class="hidden mb-4 p-4 bg-red-100 border border-red-400 text-red-700 rounded-lg text-sm">
            <p id="error-message"></p>
        </div>

        @if($errors->any())
            <div class="mb-4 p-4 bg-red-100 border border-red-400 text-red-700 rounded-lg text-sm">
                @foreach($errors->all() as $error)
                    <p>{{ $error }}</p>
                @endforeach
            </div>
        @endif

        @if(session('success'))
            <div class="mb-4 p-4 bg-green-100 border border-green-400 text-green-700 rounded-lg text-sm">
                <p>{{ session('success') }}</p>
            </div>
        @endif

        <!-- Step 1: Registration Form -->
        <div id="step-1" class="step-content">
            <form id="registration-form" class="space-y-5">
                @csrf

                <!-- Name -->
                <div>
                    <label for="name" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Имя') }}
                    </label>
                    <div class="relative">
                        <input type="text" name="name" id="name" value="{{ old('name') }}"
                               class="w-full border border-gray-300 rounded-lg pl-10 pr-4 py-3 focus:ring-2 focus:ring-green-500 focus:border-green-500 transition"
                               placeholder="{{ __('Иван Иванов') }}" required autofocus>
                        <i class="fas fa-user absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"></i>
                    </div>
                </div>

                <!-- Email -->
                <div>
                    <label for="email" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Email') }}
                    </label>
                    <div class="relative">
                        <input type="email" name="email" id="email" value="{{ old('email') }}"
                               class="w-full border border-gray-300 rounded-lg pl-10 pr-4 py-3 focus:ring-2 focus:ring-green-500 focus:border-green-500 transition"
                               placeholder="email@example.com" required>
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
                               class="w-full border border-gray-300 rounded-lg pl-10 pr-4 py-3 focus:ring-2 focus:ring-green-500 focus:border-green-500 transition"
                               placeholder="••••••••" required minlength="8">
                        <i class="fas fa-lock absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"></i>
                    </div>
                    <p class="text-xs text-gray-500 mt-1">
                        <i class="fas fa-info-circle mr-1"></i>
                        {{ __('Минимум 8 символов, цифры и спецсимволы (@$!%*#?&)') }}
                    </p>
                </div>

                <!-- Confirm Password -->
                <div>
                    <label for="password_confirmation" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ __('Подтвердите пароль') }}
                    </label>
                    <div class="relative">
                        <input type="password" name="password_confirmation" id="password_confirmation"
                               class="w-full border border-gray-300 rounded-lg pl-10 pr-4 py-3 focus:ring-2 focus:ring-green-500 focus:border-green-500 transition"
                               placeholder="••••••••" required>
                        <i class="fas fa-lock absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"></i>
                    </div>
                </div>

                <!-- Next Button -->
                <button type="button" onclick="goToStep2()"
                        class="w-full bg-green-600 text-white py-3 rounded-lg font-medium hover:bg-green-700 transition flex items-center justify-center">
                    {{ __('Далее') }}
                    <i class="fas fa-arrow-right ml-2"></i>
                </button>
            </form>
        </div>

        <!-- Step 2: CAPTCHA -->
        <div id="step-2" class="step-content hidden">
            <div class="text-center mb-4">
                <i class="fas fa-calculator text-4xl text-green-600 mb-2"></i>
                <p class="text-gray-600">{{ __('Решите 3 простых примера') }}</p>
            </div>

            <div id="captcha-container" class="space-y-4">
                <!-- CAPTCHA examples will be generated by JavaScript -->
            </div>

            <div class="flex space-x-3 mt-6">
                <button type="button" onclick="goToStep1()"
                        class="flex-1 bg-gray-300 text-gray-700 py-3 rounded-lg font-medium hover:bg-gray-400 transition">
                    <i class="fas fa-arrow-left mr-2"></i>
                    {{ __('Назад') }}
                </button>
                <button type="button" onclick="verifyCaptcha()"
                        class="flex-1 bg-green-600 text-white py-3 rounded-lg font-medium hover:bg-green-700 transition">
                    {{ __('Проверить') }}
                    <i class="fas fa-check ml-2"></i>
                </button>
            </div>
        </div>

        <!-- Step 3: Email Verification Code -->
        <div id="step-3" class="step-content hidden">
            <div class="text-center mb-4">
                <i class="fas fa-envelope-open-text text-4xl text-green-600 mb-2"></i>
                <p class="text-gray-600">{{ __('Введите код подтверждения') }}</p>
            </div>

            <div class="relative mb-4">
                <input type="text" id="verification-code-input"
                       class="w-full border border-gray-300 rounded-lg px-4 py-3 text-center text-2xl tracking-widest focus:ring-2 focus:ring-green-500 focus:border-green-500 transition"
                       placeholder="000000" maxlength="6" pattern="[0-9]{6}">
            </div>

            <div class="flex space-x-3">
                <button type="button" onclick="goToStep2()"
                        class="flex-1 bg-gray-300 text-gray-700 py-3 rounded-lg font-medium hover:bg-gray-400 transition">
                    <i class="fas fa-arrow-left mr-2"></i>
                    {{ __('Назад') }}
                </button>
                <button type="button" onclick="verifyCodeAndSubmit()"
                        class="flex-1 bg-green-600 text-white py-3 rounded-lg font-medium hover:bg-green-700 transition">
                    <i class="fas fa-user-plus mr-2"></i>
                    {{ __('Регистрация') }}
                </button>
            </div>
        </div>

        <!-- Login Link -->
        <p class="text-center text-sm text-gray-500 mt-6">
            {{ __('Уже есть аккаунт?') }}
            <a href="{{ route('login') }}" class="text-green-600 hover:text-green-800 font-medium">{{ __('Войти') }}</a>
        </p>
    </div>

    <!-- Email Code Modal -->
    <div id="code-modal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 hidden">
        <div class="bg-white rounded-2xl shadow-2xl p-6 max-w-sm w-full mx-4">
            <div class="text-center">
                <div class="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
                    <i class="fas fa-envelope text-green-600 text-2xl"></i>
                </div>
                <h3 class="text-xl font-bold text-gray-800 mb-2">{{ __('Код подтверждения') }}</h3>
                <p class="text-gray-500 text-sm mb-4">{{ __('Ваш код подтверждения email (эмуляция):') }}</p>
                <div class="bg-gray-100 rounded-lg py-4 px-6 mb-4">
                    <span id="generated-code" class="text-3xl font-mono font-bold text-green-600 tracking-widest"></span>
                </div>
                <p class="text-gray-400 text-xs mb-4">{{ __('Скопируйте этот код и введите его в поле ниже') }}</p>
                <button type="button" onclick="closeCodeModal()"
                        class="w-full bg-green-600 text-white py-2 rounded-lg font-medium hover:bg-green-700 transition">
                    {{ __('Понятно') }}
                </button>
            </div>
        </div>
    </div>

    <script>
        // State management
        let currentStep = 1;
        let formData = {};
        let captchaProblems = [];
        let verificationCode = '';

        // Generate random number between min and max
        function randomInt(min, max) {
            return Math.floor(Math.random() * (max - min + 1)) + min;
        }

        // Generate CAPTCHA problems
        function generateCaptcha() {
            const operations = ['+', '-', '×'];
            captchaProblems = [];
            const container = document.getElementById('captcha-container');
            container.innerHTML = '';

            for (let i = 0; i < 3; i++) {
                const op = operations[randomInt(0, 2)];
                let num1, num2, answer;

                switch (op) {
                    case '+':
                        num1 = randomInt(1, 20);
                        num2 = randomInt(1, 20);
                        answer = num1 + num2;
                        break;
                    case '-':
                        num1 = randomInt(10, 30);
                        num2 = randomInt(1, num1);
                        answer = num1 - num2;
                        break;
                    case '×':
                        num1 = randomInt(2, 10);
                        num2 = randomInt(2, 10);
                        answer = num1 * num2;
                        break;
                }

                captchaProblems.push({ problem: `${num1} ${op} ${num2}`, answer: answer });

                const problemHtml = `
                    <div class="flex items-center space-x-3">
                        <div class="flex-1 bg-gray-100 rounded-lg px-4 py-3 text-center font-mono text-lg">
                            ${num1} ${op} ${num2} = ?
                        </div>
                        <input type="number" id="captcha-answer-${i}"
                               class="w-24 border border-gray-300 rounded-lg px-3 py-3 text-center focus:ring-2 focus:ring-green-500 focus:border-green-500 transition"
                               placeholder="?">
                    </div>
                `;
                container.innerHTML += problemHtml;
            }
        }

        // Generate 6-digit verification code
        function generateVerificationCode() {
            verificationCode = String(randomInt(100000, 999999));
            document.getElementById('generated-code').textContent = verificationCode;
        }

        // Show error message
        function showError(message) {
            const container = document.getElementById('error-container');
            const messageEl = document.getElementById('error-message');
            messageEl.textContent = message;
            container.classList.remove('hidden');
        }

        // Hide error message
        function hideError() {
            document.getElementById('error-container').classList.add('hidden');
        }

        // Mark field with error
        function markFieldError(fieldId) {
            const field = document.getElementById(fieldId);
            field.classList.add('border-red-500', 'bg-red-50');
            field.classList.remove('border-gray-300');
        }

        // Clear all field errors
        function clearFieldErrors() {
            const fields = ['name', 'email', 'password', 'password_confirmation'];
            fields.forEach(fieldId => {
                const field = document.getElementById(fieldId);
                field.classList.remove('border-red-500', 'bg-red-50');
                field.classList.add('border-gray-300');
            });
        }

        // Update step indicators
        function updateStepIndicators(step) {
            for (let i = 1; i <= 3; i++) {
                const indicator = document.getElementById(`step-${i}-indicator`);
                if (i < step) {
                    indicator.classList.remove('bg-gray-300', 'text-gray-600');
                    indicator.classList.add('bg-green-600', 'text-white');
                } else if (i === step) {
                    indicator.classList.remove('bg-gray-300', 'text-gray-600');
                    indicator.classList.add('bg-green-600', 'text-white');
                } else {
                    indicator.classList.remove('bg-green-600', 'text-white');
                    indicator.classList.add('bg-gray-300', 'text-gray-600');
                }
            }

            // Update progress bars
            document.getElementById('progress-1').classList.toggle('bg-green-600', step > 1);
            document.getElementById('progress-2').classList.toggle('bg-green-600', step > 2);

            // Update description
            const descriptions = {
                1: '{{ __("Создайте новый аккаунт") }}',
                2: '{{ __("Подтвердите, что вы не робот") }}',
                3: '{{ __("Подтвердите ваш email") }}'
            };
            document.getElementById('step-description').textContent = descriptions[step];
        }

        // Step navigation
        function goToStep1() {
            hideError();
            document.getElementById('step-2').classList.add('hidden');
            document.getElementById('step-3').classList.add('hidden');
            document.getElementById('step-1').classList.remove('hidden');
            currentStep = 1;
            updateStepIndicators(1);
        }

        // Validate email format
        function isValidEmail(email) {
            // Проверка формата email
            const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
            return emailRegex.test(email);
        }

        // Validate password strength
        function validatePassword(password) {
            const errors = [];

            if (password.length < 8) {
                errors.push('минимум 8 символов');
            }

            if (!/[0-9]/.test(password)) {
                errors.push('минимум одну цифру');
            }

            if (!/[@$!%*#?&]/.test(password)) {
                errors.push('минимум один спецсимвол (@$!%*#?&)');
            }

            return errors;
        }

        function goToStep2() {
            hideError();
            clearFieldErrors();

            // Validate form
            const name = document.getElementById('name').value.trim();
            const email = document.getElementById('email').value.trim();
            const password = document.getElementById('password').value;
            const passwordConfirmation = document.getElementById('password_confirmation').value;

            if (!name || !email || !password || !passwordConfirmation) {
                showError('{{ __("Заполните все поля") }}');
                return;
            }

            // Email validation
            if (!isValidEmail(email)) {
                showError('{{ __("Введите корректный email адрес") }}');
                markFieldError('email');
                return;
            }

            // Password validation
            const passwordErrors = validatePassword(password);
            if (passwordErrors.length > 0) {
                showError('{{ __("Пароль должен содержать:") }} ' + passwordErrors.join(', '));
                markFieldError('password');
                return;
            }

            if (password !== passwordConfirmation) {
                showError('{{ __("Пароли не совпадают") }}');
                markFieldError('password_confirmation');
                return;
            }

            // Save form data
            formData = { name, email, password, passwordConfirmation };

            // Generate new CAPTCHA
            generateCaptcha();

            document.getElementById('step-1').classList.add('hidden');
            document.getElementById('step-3').classList.add('hidden');
            document.getElementById('step-2').classList.remove('hidden');
            currentStep = 2;
            updateStepIndicators(2);
        }

        function verifyCaptcha() {
            hideError();
            let allCorrect = true;

            for (let i = 0; i < 3; i++) {
                const input = document.getElementById(`captcha-answer-${i}`);
                const userAnswer = parseInt(input.value);

                if (isNaN(userAnswer) || userAnswer !== captchaProblems[i].answer) {
                    allCorrect = false;
                    input.classList.add('border-red-500', 'bg-red-50');
                } else {
                    input.classList.remove('border-red-500', 'bg-red-50');
                    input.classList.add('border-green-500', 'bg-green-50');
                }
            }

            if (!allCorrect) {
                showError('{{ __("Неправильные ответы. Попробуйте снова.") }}');
                setTimeout(() => {
                    generateCaptcha();
                }, 1500);
                return;
            }

            // Go to step 3 and show code modal
            document.getElementById('step-2').classList.add('hidden');
            document.getElementById('step-1').classList.add('hidden');
            document.getElementById('step-3').classList.remove('hidden');
            currentStep = 3;
            updateStepIndicators(3);

            // Generate and show verification code
            generateVerificationCode();
            document.getElementById('code-modal').classList.remove('hidden');
        }

        function closeCodeModal() {
            document.getElementById('code-modal').classList.add('hidden');
        }

        function verifyCodeAndSubmit() {
            hideError();

            const inputCode = document.getElementById('verification-code-input').value.trim();

            if (inputCode !== verificationCode) {
                showError('{{ __("Неверный код подтверждения") }}');
                return;
            }

            // Submit the form
            submitRegistration();
        }

        async function submitRegistration() {
            const submitBtn = document.querySelector('#step-3 button:last-child');
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin mr-2"></i>{{ __("Регистрация...") }}';

            // Create form and submit
            const form = document.createElement('form');
            form.method = 'POST';
            form.action = '{{ route("register.submit") }}';

            const csrfInput = document.createElement('input');
            csrfInput.type = 'hidden';
            csrfInput.name = '_token';
            csrfInput.value = document.querySelector('meta[name="csrf-token"]').content;
            form.appendChild(csrfInput);

            const fields = ['name', 'email', 'password', 'password_confirmation'];
            fields.forEach(field => {
                const input = document.createElement('input');
                input.type = 'hidden';
                input.name = field;
                input.value = field === 'password_confirmation' ? formData.passwordConfirmation : formData[field];
                form.appendChild(input);
            });

            document.body.appendChild(form);
            form.submit();
        }

        // Initialize
        document.addEventListener('DOMContentLoaded', function() {
            updateStepIndicators(1);
        });
    </script>
</body>
</html>
