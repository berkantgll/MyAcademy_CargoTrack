using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Context;

public class CargoTrackDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public CargoTrackDbContext(DbContextOptions<CargoTrackDbContext> options) : base(options)
    {

    }

    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<CargoMovement> CargoMovements { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<TransferCenter> TransferCenters { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<DeliveryException> DeliveryExceptions { get; set; }
    public DbSet<CargoPrice> CargoPrices { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cargo>()
           .Property(x => x.WeightKg)
           .HasPrecision(18, 3);

        modelBuilder.Entity<Cargo>()
           .Property(x => x.WidthCm)
           .HasPrecision(18, 2);

        modelBuilder.Entity<Cargo>()
           .Property(x => x.HeightCm)
           .HasPrecision(18, 2);

        modelBuilder.Entity<Cargo>()
            .Property(x => x.LengthCm)
            .HasPrecision(18, 2);

        modelBuilder.Entity<CargoPrice>()
           .Property(x => x.BasePrice)
           .HasPrecision(18, 2);

        modelBuilder.Entity<CargoPrice>()
            .Property(x => x.PricePerUnit)
            .HasPrecision(18, 2);

        modelBuilder.Entity<CargoPrice>()
            .Property(x => x.SpecialPackagingFee)
            .HasPrecision(18, 2);

        modelBuilder.Entity<CargoPrice>()
            .Property(x => x.GiftWrappingFee)
            .HasPrecision(18, 2);

        modelBuilder.Entity<CargoPrice>()
            .Property(x => x.InsuranceFee)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Cargo>()
            .HasOne(x => x.Sender)
            .WithMany(x => x.SentCargos)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cargo>()
            .HasOne(x => x.Receiver)
            .WithMany(x => x.ReceivedCargos)
            .HasForeignKey(x => x.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cargo>()
            .HasOne(x => x.OriginBranch)
            .WithMany(x => x.OriginCargos)
            .HasForeignKey(x => x.OriginBranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cargo>()
            .HasOne(x => x.DestinationBranch)
            .WithMany(x => x.DestinationCargos)
            .HasForeignKey(x => x.DestinationBranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CargoMovement>()
            .HasOne(x => x.Cargo)
            .WithMany(x => x.CargoMovements)
            .HasForeignKey(x => x.CargoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CargoMovement>()
             .HasOne(x => x.Branch)
             .WithMany(x => x.CargoMovements)
             .HasForeignKey(x => x.BranchId)
             .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CargoMovement>()
            .HasOne(x => x.TransferCenter)
            .WithMany(x => x.CargoMovements)
            .HasForeignKey(x => x.TransferCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CargoMovement>()
            .HasOne(x => x.Employee)
            .WithMany(x => x.ProcessedMovements)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasOne(x => x.Branch)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Delivery>()
            .HasOne(x => x.Cargo)
            .WithOne(x => x.Delivery)
            .HasForeignKey<Delivery>(x => x.CargoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DeliveryException>()
            .HasOne(x => x.Cargo)
            .WithMany(x => x.DeliveryExceptions)
            .HasForeignKey(x => x.CargoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cargo>()
            .HasOne(x => x.DeliveryAddress)
            .WithMany()
            .HasForeignKey(x => x.DeliveryAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AppUser>()
            .HasOne(x => x.Customer)
            .WithOne()
            .HasForeignKey<AppUser>(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AppUser>()
            .HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cargo>()
            .HasIndex(x => x.TrackingNumber)
            .IsUnique();

    }
}
