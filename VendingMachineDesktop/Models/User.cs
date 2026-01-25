namespace VendingMachineDesktop.Models;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsManager { get; set; }
    public bool IsEngineer { get; set; }
    public bool IsOperator { get; set; }
    public Guid? CompanyId { get; set; }
    public string? FranchiseeCode { get; set; }
}
