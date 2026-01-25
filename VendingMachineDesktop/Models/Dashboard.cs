namespace VendingMachineDesktop.Models;

public class DashboardData
{
    public double NetworkEfficiency { get; set; }
    public NetworkStatus NetworkStatus { get; set; } = new();
    public Summary Summary { get; set; } = new();
    public List<SalesDynamics> SalesDynamics { get; set; } = new();
    public List<News> News { get; set; } = new();
}

public class NetworkStatus
{
    public int Working { get; set; }
    public int NotWorking { get; set; }
    public int OnMaintenance { get; set; }
}

public class Summary
{
    public decimal TotalSales { get; set; }
    public decimal ChangeAmount { get; set; }
    public decimal YesterdayRevenue { get; set; }
    public int SalesCount { get; set; }
    public decimal TotalCollection { get; set; }
    public int CollectionCount { get; set; }
    public int MaintenanceCount { get; set; }
}

public class SalesDynamics
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int Quantity { get; set; }
}

public class News
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
