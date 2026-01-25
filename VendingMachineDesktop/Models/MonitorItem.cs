namespace VendingMachineDesktop.Models;

public class MonitorItem
{
    public int Number { get; set; }
    public string Status { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public string ConnectionType { get; set; } = string.Empty;
    public double LoadingPercentage { get; set; }
    public decimal CashAmount { get; set; }
    public int EventsCount { get; set; }
    public string EquipmentStatus { get; set; } = string.Empty;
    public string Information { get; set; } = string.Empty;
    public string AdditionalInfo { get; set; } = string.Empty;
    public Guid MachineId { get; set; }
}
