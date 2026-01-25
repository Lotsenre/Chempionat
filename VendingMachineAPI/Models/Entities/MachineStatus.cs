using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachineAPI.Models.Entities;

[Table("machine_status")]
public class MachineStatus
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("vending_machine_id")]
    public Guid VendingMachineId { get; set; }

    [Column("connection_status")]
    [MaxLength(50)]
    public string ConnectionStatus { get; set; } = "Offline";

    [Column("last_online_at")]
    public DateTime? LastOnlineAt { get; set; }

    [Column("coins_amount")]
    public int CoinsAmount { get; set; } = 0;

    [Column("bills_amount")]
    public int BillsAmount { get; set; } = 0;

    [Column("coffee_level")]
    public int CoffeeLevel { get; set; } = 0;

    [Column("sugar_level")]
    public int SugarLevel { get; set; } = 0;

    [Column("milk_level")]
    public int MilkLevel { get; set; } = 0;

    [Column("cups_level")]
    public int CupsLevel { get; set; } = 0;

    [Column("lids_level")]
    public int LidsLevel { get; set; } = 0;

    [Column("payment_system_status")]
    [MaxLength(50)]
    public string PaymentSystemStatus { get; set; } = "OK";

    [Column("dispenser_status")]
    [MaxLength(50)]
    public string DispenserStatus { get; set; } = "OK";

    [Column("cooling_system_status")]
    [MaxLength(50)]
    public string CoolingSystemStatus { get; set; } = "OK";

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("VendingMachineId")]
    public VendingMachine VendingMachine { get; set; } = null!;
}
