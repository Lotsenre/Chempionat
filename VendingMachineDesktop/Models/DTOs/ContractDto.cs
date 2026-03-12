namespace VendingMachineDesktop.Models.DTOs;

public class ContractDto
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public Guid VendingMachineId { get; set; }
    public string VendingMachineName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal YearlyRent { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ManagementType { get; set; }
    public bool InsuranceRequired { get; set; }
    public DateTime? SignedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateContractRequest
{
    public string? ContractNumber { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid VendingMachineId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal YearlyRent { get; set; }
    public int? PaybackPeriodMonths { get; set; }
    public bool InsuranceRequired { get; set; }
    public string? ManagementType { get; set; }
}

public class UpdateContractRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MonthlyRent { get; set; }
    public decimal? YearlyRent { get; set; }
    public string? Status { get; set; }
    public string? ManagementType { get; set; }
    public bool? InsuranceRequired { get; set; }
}
