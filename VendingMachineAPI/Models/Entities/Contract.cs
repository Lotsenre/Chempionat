using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VendingMachineAPI.Models.Entities;

[Table("contracts")]
public class Contract
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("contract_number")]
    [MaxLength(50)]
    public string ContractNumber { get; set; } = string.Empty;

    [Column("company_id")]
    public Guid? CompanyId { get; set; }

    [Required]
    [Column("vending_machine_id")]
    public Guid VendingMachineId { get; set; }

    [Required]
    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Required]
    [Column("end_date")]
    public DateTime EndDate { get; set; }

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

    [Column("insurance_required")]
    public bool InsuranceRequired { get; set; } = false;

    [Column("management_type")]
    [MaxLength(100)]
    public string? ManagementType { get; set; }

    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Draft";

    [Column("signed_at")]
    public DateTime? SignedAt { get; set; }

    [Column("signature_path")]
    [MaxLength(500)]
    public string? SignaturePath { get; set; }

    [Column("pdf_path")]
    [MaxLength(500)]
    public string? PdfPath { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CompanyId")]
    public Company? Company { get; set; }

    [ForeignKey("VendingMachineId")]
    public VendingMachine VendingMachine { get; set; } = null!;
}
