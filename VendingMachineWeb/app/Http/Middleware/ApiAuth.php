<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Session;
use Symfony\Component\HttpFoundation\Response;

class ApiAuth
{
    /**
     * Handle an incoming request.
     */
    public function handle(Request $request, Closure $next): Response
    {
        if (!Session::get('logged_in') || !Session::get('api_token')) {
            return redirect()->route('login')
                ->with('error', 'Необходима авторизация');
        }

        return $next($request);
    }
}
