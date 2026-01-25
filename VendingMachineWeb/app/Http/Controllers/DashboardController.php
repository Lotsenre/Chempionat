<?php

namespace App\Http\Controllers;

use App\Models\VendingMachine;
use Illuminate\Http\Request;

class DashboardController extends Controller
{
    public function index()
    {
        $stats = [
            'total_machines' => VendingMachine::count(),
            'working_machines' => VendingMachine::where('status', 'Работает')->count(),
            'not_working_machines' => VendingMachine::where('status', '!=', 'Работает')->count(),
            'total_income' => VendingMachine::sum('total_income'),
        ];

        return view('dashboard', compact('stats'));
    }
}
