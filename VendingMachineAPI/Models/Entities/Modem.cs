using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachineAPI.Models.Entities;

[Table("modems")]
public class Modem
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("modem_number")]
    [MaxLength(50)]
    public string ModemNumber { get; set; } = string.Empty;

    [Column("model")]
    [MaxLength(100)]
    public string? Model { get; set; }

    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Active";

    [Column("vending_machine_id")]
    public Guid? VendingMachineId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("VendingMachineId")]
    public VendingMachine? VendingMachine { get; set; }
}
