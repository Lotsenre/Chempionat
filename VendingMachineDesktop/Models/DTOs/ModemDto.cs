namespace VendingMachineDesktop.Models.DTOs;

public class ModemDto
{
    public Guid Id { get; set; }
    public string ModemNumber { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string Status { get; set; } = "Active";
    public Guid? VendingMachineId { get; set; }
    public string? VendingMachineName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateModemRequest
{
    public string ModemNumber { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Status { get; set; }
    public Guid? VendingMachineId { get; set; }
}

public class UpdateModemRequest
{
    public string? ModemNumber { get; set; }
    public string? Model { get; set; }
    public string? Status { get; set; }
    public Guid? VendingMachineId { get; set; }
}
