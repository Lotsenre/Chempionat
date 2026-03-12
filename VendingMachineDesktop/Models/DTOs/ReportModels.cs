namespace VendingMachineDesktop.Models.DTOs;

public class SalesReportResponse
{
    public SalesReportSummary Summary { get; set; } = new();
    public List<SalesReportItem> Data { get; set; } = new();
}

public class SalesReportSummary
{
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalQuantity { get; set; }
    public decimal AverageCheck { get; set; }
    public ReportPeriod Period { get; set; } = new();
}

public class ReportPeriod
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class SalesReportItem
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public string MachineSerial { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

public class MaintenanceReportResponse
{
    public MaintenanceReportSummary Summary { get; set; } = new();
    public List<MaintenanceReportItem> Data { get; set; } = new();
}

public class MaintenanceReportSummary
{
    public int Total { get; set; }
    public int Completed { get; set; }
    public int Pending { get; set; }
    public ReportPeriod Period { get; set; } = new();
}

public class MaintenanceReportItem
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string MachineSerial { get; set; } = string.Empty;
    public string WorkDescription { get; set; } = string.Empty;
    public string? IssuesFound { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class RevenueReportResponse
{
    public List<DailyRevenueItem> DailyRevenue { get; set; } = new();
    public List<MachineRevenueItem> ByMachine { get; set; } = new();
    public List<PaymentMethodRevenueItem> ByPaymentMethod { get; set; } = new();
}

public class DailyRevenueItem
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int SalesCount { get; set; }
    public int Quantity { get; set; }
}

public class MachineRevenueItem
{
    public Guid MachineId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int SalesCount { get; set; }
}

public class PaymentMethodRevenueItem
{
    public string Method { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Count { get; set; }
}
