<?php

namespace App\Http\Controllers;

use App\Services\VendingApiService;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Session;

class AuthController extends Controller
{
    protected VendingApiService $apiService;

    public function __construct(VendingApiService $apiService)
    {
        $this->apiService = $apiService;
    }

    public function showLogin()
    {
        return view('auth.login');
    }

    public function login(Request $request)
    {
        $credentials = $request->validate([
            'email' => 'required|email',
            'password' => 'required',
        ]);

        $response = $this->apiService->login($credentials['email'], $credentials['password']);

        if ($response['success']) {
            $userData = $response['data'];

            // Store user info in session
            Session::put('user', [
                'id' => $userData['userId'] ?? null,
                'email' => $userData['email'] ?? $credentials['email'],
                'name' => $userData['fullName'] ?? 'User',
                'role' => $userData['role'] ?? 'User',
            ]);

            Session::put('logged_in', true);

            $request->session()->regenerate();
            return redirect()->intended('/vending-machines');
        }

        return back()->withErrors([
            'email' => 'Неверные учетные данные.',
        ]);
    }

    public function showRegister()
    {
        return view('auth.register');
    }

    public function register(Request $request)
    {
        $validated = $request->validate([
            'name' => 'required|string|max:255',
            'email' => 'required|email:rfc,dns',
            'password' => [
                'required',
                'confirmed',
                'min:8',
                'regex:/[0-9]/',      // минимум одна цифра
                'regex:/[@$!%*#?&]/', // минимум один спецсимвол
            ],
        ], [
            'email.email' => 'Введите корректный email адрес.',
            'password.min' => 'Пароль должен содержать минимум 8 символов.',
            'password.regex' => 'Пароль должен содержать цифры и спецсимволы (@$!%*#?&).',
            'password.confirmed' => 'Пароли не совпадают.',
        ]);

        $response = $this->apiService->register(
            $validated['email'],
            $validated['password'],
            $validated['name']
        );

        if ($response['success']) {
            return redirect()->route('login')->with('success', 'Регистрация успешна! Теперь вы можете войти.');
        }

        $errorMessage = $response['message'] ?? 'Ошибка регистрации';
        if (str_contains($errorMessage, 'already exists')) {
            $errorMessage = 'Пользователь с таким email уже существует.';
        }

        return back()->withErrors([
            'email' => $errorMessage,
        ])->withInput();
    }

    public function logout(Request $request)
    {
        $this->apiService->clearToken();

        Session::forget('user');
        Session::forget('logged_in');

        $request->session()->invalidate();
        $request->session()->regenerateToken();

        return redirect('/login');
    }
}
