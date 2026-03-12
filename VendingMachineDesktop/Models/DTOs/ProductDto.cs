namespace VendingMachineDesktop.Models.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int MinStock { get; set; }
    public string? Category { get; set; }
    public Guid? VendingMachineId { get; set; }
    public string? VendingMachineName { get; set; }
    public int QuantityAvailable { get; set; }
    public decimal? SalesTrend { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int MinStock { get; set; } = 10;
    public string? Category { get; set; }
    public Guid? VendingMachineId { get; set; }
    public int QuantityAvailable { get; set; }
}

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? MinStock { get; set; }
    public string? Category { get; set; }
    public Guid? VendingMachineId { get; set; }
    public int? QuantityAvailable { get; set; }
}
