<?php

use App\Http\Controllers\VendingMachineController;
use App\Http\Controllers\AuthController;
use App\Http\Controllers\DashboardController;
use App\Http\Controllers\ContractController;
use Illuminate\Support\Facades\Route;

// Auth routes (public)
Route::get('/login', [AuthController::class, 'showLogin'])->name('login');
Route::post('/login', [AuthController::class, 'login'])->name('login.submit');
Route::get('/register', [AuthController::class, 'showRegister'])->name('register');
Route::post('/register', [AuthController::class, 'register'])->name('register.submit');
Route::post('/logout', [AuthController::class, 'logout'])->name('logout');

// Protected routes
Route::middleware('api.auth')->group(function () {
    // Dashboard
    Route::get('/', [DashboardController::class, 'index'])->name('dashboard');

    // Vending Machines
    Route::get('/vending-machines', [VendingMachineController::class, 'index'])->name('vending-machines.index');
    Route::get('/vending-machines/export', [VendingMachineController::class, 'export'])->name('vending-machines.export');
    Route::get('/vending-machines/create', [VendingMachineController::class, 'create'])->name('vending-machines.create');
    Route::post('/vending-machines', [VendingMachineController::class, 'store'])->name('vending-machines.store');
    Route::get('/vending-machines/{id}/edit', [VendingMachineController::class, 'edit'])->name('vending-machines.edit');
    Route::put('/vending-machines/{id}', [VendingMachineController::class, 'update'])->name('vending-machines.update');
    Route::delete('/vending-machines/{id}', [VendingMachineController::class, 'destroy'])->name('vending-machines.destroy');
    Route::post('/vending-machines/{id}/detach-modem', [VendingMachineController::class, 'detachModem'])->name('vending-machines.detach-modem');

    // Contracts
    Route::get('/contracts', [ContractController::class, 'index'])->name('contracts.index');
});

// Language switcher
Route::get('/lang/{locale}', function ($locale) {
    if (in_array($locale, ['ru', 'en'])) {
        session(['locale' => $locale]);
    }
    return back();
})->name('lang.switch');
