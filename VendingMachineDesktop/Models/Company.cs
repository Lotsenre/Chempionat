namespace VendingMachineDesktop.Models;

public class Company
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
    public string INN { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public Guid? ParentCompanyId { get; set; }
    public string ParentCompanyName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime WorkingSince { get; set; }
}
