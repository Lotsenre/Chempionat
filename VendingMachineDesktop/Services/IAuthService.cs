using VendingMachineDesktop.Models.DTOs;

namespace VendingMachineDesktop.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(string email, string password);
    string? GetToken();
    void SetToken(string token);
    void ClearToken();
    LoginResponse? GetCurrentUser();
    void SetCurrentUser(LoginResponse user);
}
