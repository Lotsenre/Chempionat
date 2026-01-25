using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachineAPI.Models.Entities;

[Table("events")]
public class Event
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("vending_machine_id")]
    public Guid VendingMachineId { get; set; }

    [Required]
    [Column("event_type")]
    [MaxLength(100)]
    public string EventType { get; set; } = string.Empty;

    [Required]
    [Column("event_description")]
    public string EventDescription { get; set; } = string.Empty;

    [Required]
    [Column("event_date_time")]
    public DateTime EventDateTime { get; set; } = DateTime.UtcNow;

    [Column("severity")]
    [MaxLength(50)]
    public string Severity { get; set; } = "Info";

    // Navigation properties
    [ForeignKey("VendingMachineId")]
    public VendingMachine VendingMachine { get; set; } = null!;
}
