<?php

namespace App\Services;

use Illuminate\Support\Facades\Http;
use Illuminate\Support\Facades\Session;

class VendingApiService
{
    protected string $baseUrl;

    public function __construct()
    {
        $this->baseUrl = config('services.vending_api.url', 'http://localhost:5000');
    }

    /**
     * Get stored JWT token from session
     */
    protected function getToken(): ?string
    {
        return Session::get('api_token');
    }

    /**
     * Store JWT token in session
     */
    public function setToken(string $token): void
    {
        Session::put('api_token', $token);
    }

    /**
     * Clear stored token
     */
    public function clearToken(): void
    {
        Session::forget('api_token');
    }

    /**
     * Register new user via API
     */
    public function register(string $email, string $password, string $fullName): array
    {
        $response = Http::withOptions([
            'verify' => false,
        ])->post("{$this->baseUrl}/api/auth/register", [
            'email' => $email,
            'password' => $password,
            'fullName' => $fullName,
        ]);

        if ($response->successful()) {
            return [
                'success' => true,
                'data' => $response->json(),
            ];
        }

        return [
            'success' => false,
            'message' => $response->json('message') ?? 'Registration failed',
            'status' => $response->status(),
        ];
    }

    /**
     * Login to API and get JWT token
     */
    public function login(string $email, string $password): array
    {
        $response = Http::withOptions([
            'verify' => false, // Disable SSL verification for localhost
        ])->post("{$this->baseUrl}/api/auth/login", [
            'email' => $email,
            'password' => $password,
        ]);

        if ($response->successful()) {
            $data = $response->json();
            if (isset($data['token'])) {
                $this->setToken($data['token']);
            }
            return [
                'success' => true,
                'data' => $data,
            ];
        }

        return [
            'success' => false,
            'message' => $response->json('message') ?? 'Login failed',
            'status' => $response->status(),
        ];
    }

    /**
     * Make authenticated GET request
     */
    public function get(string $endpoint, array $query = []): array
    {
        $token = $this->getToken();

        $response = Http::withOptions([
            'verify' => false,
        ])->withHeaders([
            'Authorization' => "Bearer {$token}",
        ])->get("{$this->baseUrl}{$endpoint}", $query);

        return $this->handleResponse($response);
    }

    /**
     * Make authenticated POST request
     */
    public function post(string $endpoint, array $data = []): array
    {
        $token = $this->getToken();

        $response = Http::withOptions([
            'verify' => false,
        ])->withHeaders([
            'Authorization' => "Bearer {$token}",
        ])->post("{$this->baseUrl}{$endpoint}", $data);

        return $this->handleResponse($response);
    }

    /**
     * Make authenticated PUT request
     */
    public function put(string $endpoint, array $data = []): array
    {
        $token = $this->getToken();

        $response = Http::withOptions([
            'verify' => false,
        ])->withHeaders([
            'Authorization' => "Bearer {$token}",
        ])->put("{$this->baseUrl}{$endpoint}", $data);

        return $this->handleResponse($response);
    }

    /**
     * Make authenticated DELETE request
     */
    public function delete(string $endpoint): array
    {
        $token = $this->getToken();

        $response = Http::withOptions([
            'verify' => false,
        ])->withHeaders([
            'Authorization' => "Bearer {$token}",
        ])->delete("{$this->baseUrl}{$endpoint}");

        return $this->handleResponse($response);
    }

    /**
     * Handle API response
     */
    protected function handleResponse($response): array
    {
        if ($response->successful()) {
            return [
                'success' => true,
                'data' => $response->json(),
                'total' => $response->header('X-Total-Count'),
            ];
        }

        return [
            'success' => false,
            'message' => $response->json('message') ?? 'Request failed',
            'status' => $response->status(),
        ];
    }

    /**
     * Get vending machines list
     */
    public function getVendingMachines(int $page = 1, int $pageSize = 10, ?string $search = null): array
    {
        $query = [
            'page' => $page,
            'pageSize' => $pageSize,
        ];

        if ($search) {
            $query['search'] = $search;
        }

        return $this->get('/api/vendingmachines', $query);
    }

    /**
     * Get single vending machine
     */
    public function getVendingMachine(string $id): array
    {
        return $this->get("/api/vendingmachines/{$id}");
    }

    /**
     * Create vending machine
     */
    public function createVendingMachine(array $data): array
    {
        return $this->post('/api/vendingmachines', $data);
    }

    /**
     * Update vending machine
     */
    public function updateVendingMachine(string $id, array $data): array
    {
        return $this->put("/api/vendingmachines/{$id}", $data);
    }

    /**
     * Delete vending machine
     */
    public function deleteVendingMachine(string $id): array
    {
        return $this->delete("/api/vendingmachines/{$id}");
    }

    /**
     * Detach modem from vending machine
     */
    public function detachModem(string $id): array
    {
        return $this->post("/api/vendingmachines/{$id}/detach-modem");
    }

    /**
     * Get dashboard statistics
     */
    public function getDashboard(): array
    {
        return $this->get('/api/dashboard');
    }
}
