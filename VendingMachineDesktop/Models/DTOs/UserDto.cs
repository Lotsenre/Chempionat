namespace VendingMachineDesktop.Models.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string? FranchiseeCode { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = "Engineer";
    public Guid? CompanyId { get; set; }
    public string? FranchiseeCode { get; set; }
}

public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Role { get; set; }
    public Guid? CompanyId { get; set; }
    public string? FranchiseeCode { get; set; }
    public bool? IsActive { get; set; }
}
