using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VendingMachineAPI.Models.Entities;

[Table("vending_machines")]
public class VendingMachine
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("serial_number")]
    [MaxLength(50)]
    public string SerialNumber { get; set; } = string.Empty;

    [Required]
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("model")]
    [MaxLength(255)]
    public string Model { get; set; } = string.Empty;

    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Working";

    [Required]
    [Column("location")]
    [MaxLength(500)]
    public string Location { get; set; } = string.Empty;

    [Column("place")]
    [MaxLength(500)]
    public string? Place { get; set; }

    [Column("coordinates")]
    [MaxLength(100)]
    public string? Coordinates { get; set; }

    [Required]
    [Column("install_date")]
    public DateTime InstallDate { get; set; }

    [Column("last_maintenance_date")]
    public DateTime? LastMaintenanceDate { get; set; }

    [Column("working_hours")]
    [MaxLength(50)]
    public string? WorkingHours { get; set; }

    [Column("timezone")]
    [MaxLength(50)]
    public string? Timezone { get; set; }

    [Column("total_income")]
    [Precision(18, 2)]
    public decimal TotalIncome { get; set; } = 0;

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("work_mode")]
    [MaxLength(100)]
    public string? WorkMode { get; set; }

    [Column("rfid_cash_collection")]
    [MaxLength(50)]
    public string? RfidCashCollection { get; set; }

    [Column("rfid_loading")]
    [MaxLength(50)]
    public string? RfidLoading { get; set; }

    [Column("rfid_service")]
    [MaxLength(50)]
    public string? RfidService { get; set; }

    [Column("kit_online_id")]
    [MaxLength(50)]
    public string? KitOnlineId { get; set; }

    [Column("company")]
    [MaxLength(255)]
    public string? Company { get; set; }

    [Column("payment_type")]
    [MaxLength(255)]
    public string? PaymentType { get; set; }

    [Column("critical_threshold_template")]
    [MaxLength(100)]
    public string? CriticalThresholdTemplate { get; set; }

    [Column("service_priority")]
    [MaxLength(50)]
    public string? ServicePriority { get; set; }

    [Column("manager")]
    [MaxLength(255)]
    public string? Manager { get; set; }

    [Column("notification_template")]
    [MaxLength(100)]
    public string? NotificationTemplate { get; set; }

    [Column("engineer")]
    [MaxLength(255)]
    public string? Engineer { get; set; }

    [Column("operator")]
    [MaxLength(255)]
    public string? Operator { get; set; }

    [Column("technician")]
    [MaxLength(255)]
    public string? Technician { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public User? User { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public ICollection<Maintenance> MaintenanceRecords { get; set; } = new List<Maintenance>();
    public ICollection<Event> Events { get; set; } = new List<Event>();
    public MachineStatus? MachineStatus { get; set; }
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    public ICollection<AvailableMachine> AvailableMachines { get; set; } = new List<AvailableMachine>();
}
