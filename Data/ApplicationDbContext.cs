using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Composition;
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

            // Configuração da herança de Vehicle (TPH - Table Per Hierarchy)
            modelBuilder.Entity<Vehicle>()
                .HasDiscriminator<string>("VehicleType")
                .HasValue<Bicycle>("Bicycle")
                .HasValue<Car>("Car")
                .HasValue<Motorcycle>("Motorcycle");

            // Configuração do ApplicationUser
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName).HasMaxLength(100);
                entity.Property(u => u.CPF).HasMaxLength(11);
                entity.Property(u => u.PhoneNumber).HasMaxLength(20);
                entity.Property(u => u.Address).HasMaxLength(200);
            });

            // Configuração de Vehicle
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.Property(v => v.Model).HasMaxLength(100);
                entity.Property(v => v.LicensePlate).HasMaxLength(15);
                entity.Property(v => v.HourlyRate).HasColumnType("decimal(18,2)");
            });

            // Configuração de Rental
            modelBuilder.Entity<Rental>(entity =>
            {
                // Relacionamento com ApplicationUser (Customer)
                entity.HasOne(r => r.Customer)
                      .WithMany(u => u.Rentals)
                      .HasForeignKey(r => r.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relacionamento com Vehicle
                entity.HasOne(r => r.RentedVehicle)
                      .WithMany()
                      .HasForeignKey(r => r.VehicleId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relacionamento com Payment (opcional)
                entity.HasOne(r => r.PaymentInfo)
                      .WithMany()
                      .HasForeignKey(r => r.PaymentId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.Property(r => r.TotalCost).HasColumnType("decimal(18,2)");
            });

            // Configuração de Payment
            modelBuilder.Entity<Payment>(entity =>
            {
                // Relacionamento com ApplicationUser (Payer)
                entity.HasOne(p => p.Payer)
                      .WithMany()
                      .HasForeignKey(p => p.PayerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Correção: Relacionamento com PaymentMethod
                entity.HasOne(p => p.PaymentMethod)
                      .WithMany(pm => pm.Payments)  // Adicionando a navegação inversa
                      .HasForeignKey(p => p.PaymentMethodId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            });

            // Configuração de PaymentMethod (atualizada para incluir navegação inversa)
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                // Relacionamento com ApplicationUser
                entity.HasOne(pm => pm.User)
                      .WithMany(u => u.PaymentMethods)
                      .HasForeignKey(pm => pm.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Adicionando a coleção de Payments
                entity.HasMany(pm => pm.Payments)
                      .WithOne(p => p.PaymentMethod)
                      .HasForeignKey(p => p.PaymentMethodId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(pm => pm.Type).HasMaxLength(20);
                entity.Property(pm => pm.CardNumber).HasMaxLength(20);
                entity.Property(pm => pm.CardHolderName).HasMaxLength(100);
                entity.Property(pm => pm.PixKey).HasMaxLength(50);
            });

            // Configuração de Report
            modelBuilder.Entity<Report>(entity =>
            {
                // Relacionamento com ApplicationUser (GeneratedBy)
                entity.HasOne(r => r.GeneratedBy)
                      .WithMany()
                      .HasForeignKey(r => r.GeneratedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração de índices para melhor performance
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

            // Configuração de valores padrão
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