using ComputerRepairSystem.domain.Entities;
using ComputerRepairSystem.infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem.infrastructure.data
{
    public class MasterErpDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DbSet<Company> Companies
            => Set<Company>();

        public DbSet<CompanyDatabase> CompanyDatabases
            => Set<CompanyDatabase>();

        public DbSet<DeviceSample> DeviceSamples
            => Set<DeviceSample>();


        public MasterErpDbContext(
            DbContextOptions<MasterErpDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            // ==========================================
            // ASP.NET IDENTITY
            // ==========================================

            base.OnModelCreating(builder);


            // ==========================================
            // COMPANY
            // ==========================================

            builder.Entity<Company>(entity =>
            {
                entity.HasKey(x => x.CompanyId);

                entity.Property(x => x.CompanyCode)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.CompanyName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.HasIndex(x => x.CompanyCode)
                    .IsUnique();
            });


            // ==========================================
            // COMPANY DATABASE
            // ==========================================

            builder.Entity<CompanyDatabase>(entity =>
            {
                entity.HasKey(
                    x => x.CompanyDatabaseId);

                entity.Property(x => x.ServerName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.DatabaseName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.HasOne(x => x.Company)
                    .WithMany()
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(
                        DeleteBehavior.Restrict);
            });


            // ==========================================
            // DEVICE SAMPLE
            // ==========================================

            builder.Entity<DeviceSample>(entity =>
            {
                entity.ToTable("DeviceSample");

                entity.HasKey(x => x.DeviceID);

                entity.Property(x => x.DeviceCode)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.DeviceName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.HasOne(x => x.Company)
                    .WithMany(x => x.Devices)
                    .HasForeignKey(x => x.CompanyID)
                    .OnDelete(
                        DeleteBehavior.Restrict);

                entity.HasIndex(
                    x => new
                    {
                        x.CompanyID,
                        x.DeviceCode
                    })
                    .IsUnique();
            });


            // ==========================================
            // APPLICATION USER
            // ==========================================

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(x => x.Company)
                    .WithMany()
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(
                        DeleteBehavior.Restrict);
            });
        }
    }
}