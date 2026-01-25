using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VendingMachineAPI.Models.Entities;

[Table("available_machines")]
public class AvailableMachine
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("vending_machine_id")]
    public Guid VendingMachineId { get; set; }

    [Required]
    [Column("monthly_rent")]
    [Precision(10, 2)]
    public decimal MonthlyRent { get; set; }

    [Required]
    [Column("yearly_rent")]
    [Precision(10, 2)]
    public decimal YearlyRent { get; set; }

    [Column("payback_period_months")]
    public int? PaybackPeriodMonths { get; set; }

    [Column("placement_location")]
    [MaxLength(500)]
    public string? PlacementLocation { get; set; }

    [Column("is_available")]
    public bool IsAvailable { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("VendingMachineId")]
    public VendingMachine VendingMachine { get; set; } = null!;
}
