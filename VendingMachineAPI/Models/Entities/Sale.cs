using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VendingMachineAPI.Models.Entities;

[Table("sales")]
public class Sale
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("vending_machine_id")]
    public Guid? VendingMachineId { get; set; }

    [Required]
    [Column("product_id")]
    public Guid ProductId { get; set; }

    [Required]
    [Column("quantity")]
    public int Quantity { get; set; }

    [Required]
    [Column("total_price")]
    [Precision(10, 2)]
    public decimal TotalPrice { get; set; }

    [Required]
    [Column("payment_method")]
    [MaxLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    [Column("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("VendingMachineId")]
    public VendingMachine? VendingMachine { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;
}
