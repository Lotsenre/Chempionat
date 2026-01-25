using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachineAPI.Models.Entities;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("full_name")]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Column("phone")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [Column("password_hash")]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column("role")]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;

    [Column("image")]
    public string? Image { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("company_id")]
    public Guid? CompanyId { get; set; }

    [Column("franchisee_code")]
    [MaxLength(50)]
    public string? FranchiseeCode { get; set; }

    [Column("is_manager")]
    public bool IsManager { get; set; } = false;

    [Column("is_engineer")]
    public bool IsEngineer { get; set; } = false;

    [Column("is_operator")]
    public bool IsOperator { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    [ForeignKey("CompanyId")]
    public Company? Company { get; set; }

    public ICollection<VendingMachine> VendingMachines { get; set; } = new List<VendingMachine>();
    public ICollection<News> NewsArticles { get; set; } = new List<News>();
}
