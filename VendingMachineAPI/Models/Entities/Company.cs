using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachineAPI.Models.Entities;

[Table("companies")]
public class Company
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("address")]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Column("contact_info")]
    [MaxLength(500)]
    public string? ContactInfo { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("parent_company_id")]
    public Guid? ParentCompanyId { get; set; }

    [Required]
    [Column("working_since")]
    public DateTime WorkingSince { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("ParentCompanyId")]
    public Company? ParentCompany { get; set; }

    public ICollection<Company> SubCompanies { get; set; } = new List<Company>();
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
