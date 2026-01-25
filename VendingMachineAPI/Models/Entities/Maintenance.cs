using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachineAPI.Models.Entities;

[Table("maintenance")]
public class Maintenance
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("vending_machine_id")]
    public Guid VendingMachineId { get; set; }

    [Required]
    [Column("date")]
    public DateTime Date { get; set; }

    [Required]
    [Column("work_description")]
    public string WorkDescription { get; set; } = string.Empty;

    [Column("issues_found")]
    public string? IssuesFound { get; set; }

    [Column("technician_id")]
    public Guid? TechnicianId { get; set; }

    [Column("full_name")]
    [MaxLength(255)]
    public string? FullName { get; set; }

    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Completed";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("VendingMachineId")]
    public VendingMachine VendingMachine { get; set; } = null!;
}
