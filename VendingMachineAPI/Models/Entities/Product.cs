using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VendingMachineAPI.Models.Entities;

[Table("products")]
public class Product
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("price")]
    [Precision(10, 2)]
    public decimal Price { get; set; }

    [Column("min_stock")]
    public int MinStock { get; set; } = 10;

    [Column("category")]
    [MaxLength(100)]
    public string? Category { get; set; }

    [Column("vending_machine_id")]
    public Guid? VendingMachineId { get; set; }

    [Column("quantity_available")]
    public int QuantityAvailable { get; set; } = 0;

    [Column("sales_trend")]
    [Precision(5, 2)]
    public decimal? SalesTrend { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("VendingMachineId")]
    public VendingMachine? VendingMachine { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
