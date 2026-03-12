using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Models.Entities;

namespace VendingMachineAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<VendingMachine> VendingMachines { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Maintenance> MaintenanceRecords { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<MachineStatus> MachineStatuses { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<AvailableMachine> AvailableMachines { get; set; }
    public DbSet<Modem> Modems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.FranchiseeCode).IsUnique();

            entity.HasOne(e => e.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Company entity configuration
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasOne(e => e.ParentCompany)
                .WithMany(c => c.SubCompanies)
                .HasForeignKey(e => e.ParentCompanyId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // VendingMachine entity configuration
        modelBuilder.Entity<VendingMachine>(entity =>
        {
            entity.HasIndex(e => e.SerialNumber).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.VendingMachines)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.MachineStatus)
                .WithOne(ms => ms.VendingMachine)
                .HasForeignKey<MachineStatus>(ms => ms.VendingMachineId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Product entity configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasOne(e => e.VendingMachine)
                .WithMany(vm => vm.Products)
                .HasForeignKey(e => e.VendingMachineId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Sale entity configuration
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasOne(e => e.VendingMachine)
                .WithMany(vm => vm.Sales)
                .HasForeignKey(e => e.VendingMachineId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.Sales)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Maintenance entity configuration
        modelBuilder.Entity<Maintenance>(entity =>
        {
            entity.HasOne(e => e.VendingMachine)
                .WithMany(vm => vm.MaintenanceRecords)
                .HasForeignKey(e => e.VendingMachineId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Event entity configuration
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasOne(e => e.VendingMachine)
                .WithMany(vm => vm.Events)
                .HasForeignKey(e => e.VendingMachineId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // News entity configuration
        modelBuilder.Entity<News>(entity =>
        {
            entity.HasOne(e => e.Author)
                .WithMany(u => u.NewsArticles)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // MachineStatus entity configuration
        modelBuilder.Entity<MachineStatus>(entity =>
        {
            entity.HasIndex(e => e.VendingMachineId).IsUnique();
        });

        // Contract entity configuration
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasIndex(e => e.ContractNumber).IsUnique();

            entity.HasOne(e => e.Company)
                .WithMany(c => c.Contracts)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.VendingMachine)
                .WithMany(vm => vm.Contracts)
                .HasForeignKey(e => e.VendingMachineId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AvailableMachine entity configuration
        modelBuilder.Entity<AvailableMachine>(entity =>
        {
            entity.HasOne(e => e.VendingMachine)
                .WithMany(vm => vm.AvailableMachines)
                .HasForeignKey(e => e.VendingMachineId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Modem entity configuration
        modelBuilder.Entity<Modem>(entity =>
        {
            entity.HasIndex(e => e.ModemNumber).IsUnique();
        });
    }
}
