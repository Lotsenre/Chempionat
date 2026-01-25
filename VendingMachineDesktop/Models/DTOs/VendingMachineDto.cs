namespace VendingMachineDesktop.Models.DTOs;

public class VendingMachineDto
{
    public Guid Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Place { get; set; }
    public string? Coordinates { get; set; }
    public DateTime InstallDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public string? WorkingHours { get; set; }
    public string? Timezone { get; set; }
    public decimal TotalIncome { get; set; }
    public string? Company { get; set; }
    public string? Manager { get; set; }
    public string? Engineer { get; set; }
    public string? Operator { get; set; }
    public string? WorkMode { get; set; }
    public string? PaymentType { get; set; }
    public string? KitOnlineId { get; set; }
    public string? ServicePriority { get; set; }
    public string? CriticalThresholdTemplate { get; set; }
    public string? NotificationTemplate { get; set; }
    public string? RfidService { get; set; }
    public string? RfidCashCollection { get; set; }
    public string? RfidLoading { get; set; }
    public string? Notes { get; set; }
}
