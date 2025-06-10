using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wheels_in_Csharp.Models;

namespace Wheels_in_Csharp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Bicycle> Bicycles { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Motorcycle> Motorcycles { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vehicle>()
                .HasDiscriminator<string>("VehicleType")
                .HasValue<Bicycle>("Bicycle")
                .HasValue<Car>("Car")
                .HasValue<Motorcycle>("Motorcycle");

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName).HasMaxLength(100);
                entity.Property(u => u.CPF).HasMaxLength(11);
                entity.Property(u => u.PhoneNumber).HasMaxLength(20);
                entity.Property(u => u.Address).HasMaxLength(200);
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.Property(v => v.Model).HasMaxLength(100);
                entity.Property(v => v.LicensePlate).HasMaxLength(15);
                entity.Property(v => v.HourlyRate).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.HasOne(r => r.Customer)
                      .WithMany(u => u.Rentals)
                      .HasForeignKey(r => r.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.RentedVehicle)
                      .WithMany()
                      .HasForeignKey(r => r.VehicleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.PaymentInfo)
                      .WithMany()
                      .HasForeignKey(r => r.PaymentId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.Property(r => r.TotalCost).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.Payer)
                      .WithMany()
                      .HasForeignKey(p => p.PayerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.PaymentMethod)
                      .WithMany(pm => pm.Payments)
                      .HasForeignKey(p => p.PaymentMethodId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasOne(pm => pm.User)
                      .WithMany(u => u.PaymentMethods)
                      .HasForeignKey(pm => pm.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(pm => pm.Payments)
                      .WithOne(p => p.PaymentMethod)
                      .HasForeignKey(p => p.PaymentMethodId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(pm => pm.Type).HasMaxLength(20);
                entity.Property(pm => pm.CardNumber).HasMaxLength(20);
                entity.Property(pm => pm.CardHolderName).HasMaxLength(100);
                entity.Property(pm => pm.PixKey).HasMaxLength(50);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasOne(r => r.GeneratedBy)
                      .WithMany()
                      .HasForeignKey(r => r.GeneratedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => new { v.Available, v.Status });

            modelBuilder.Entity<Rental>()
                .HasIndex(r => r.CustomerId);

            modelBuilder.Entity<Rental>()
                .HasIndex(r => r.VehicleId);

            modelBuilder.Entity<Rental>()
                .HasIndex(r => r.Status);

            modelBuilder.Entity<PaymentMethod>()
                .HasIndex(pm => pm.UserId);

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Status)
                .HasDefaultValue(VehicleStatus.AVAILABLE);

            modelBuilder.Entity<Rental>()
                .Property(r => r.Status)
                .HasDefaultValue(RentalStatus.ACTIVE);

            modelBuilder.Entity<PaymentMethod>()
                .Property(pm => pm.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
