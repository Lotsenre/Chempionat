namespace VendingMachineDesktop.Models.DTOs;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public Guid? ParentCompanyId { get; set; }
    public string ParentCompanyName { get; set; } = string.Empty;
    public DateTime WorkingSince { get; set; }
    public DateTime CreatedAt { get; set; }
}
