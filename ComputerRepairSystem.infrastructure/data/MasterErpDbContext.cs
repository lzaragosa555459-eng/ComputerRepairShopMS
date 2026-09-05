using ComputerRepairSystem.domain.entities;
using ComputerRepairSystem.domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace ComputerRepairSystem.domain.data
{
    public class MasterErpDbContext : IdentityDbContext
    {

        public DbSet<Company> Companies => Set<Company>();



        public DbSet<CompanyDatabase> CompanyDatabases => Set<CompanyDatabase>();


        public DbSet<DeviceSample> DeviceSamples => Set<DeviceSample>();

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();

        public MasterErpDbContext(DbContextOptions<MasterErpDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)

                {

                    base.OnModelCreating(builder);



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



                    builder.Entity<CompanyDatabase>(entity =>

                    {

                        entity.HasKey(x => x.CompanyDatabaseId);



                        entity.Property(x => x.ServerName)

                          .HasMaxLength(200)

                          .IsRequired();



                        entity.Property(x => x.DatabaseName)

                          .HasMaxLength(200)

                          .IsRequired();



                        entity.HasOne(x => x.Company)

                          .WithMany()

                          .HasForeignKey(x => x.CompanyId)

                          .OnDelete(DeleteBehavior.Restrict);

                    });

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
                            .OnDelete(DeleteBehavior.Restrict);

                        entity.HasIndex(x => new { x.CompanyID, x.DeviceCode})
                            .IsUnique();
                    });

                    builder.Entity<Role>(entity =>
                    {
                        entity.HasKey(x => x.RoleId);

                        entity.Property(x => x.RoleName)
                            .HasMaxLength(100)
                            .IsRequired();

                        entity.Property(x => x.Description)
                            .HasMaxLength(300);

                        entity.HasIndex(x => x.RoleName)
                            .IsUnique();
                    });

                    builder.Entity<User>(entity =>
                    {
                        entity.HasKey(x => x.UserId);

                        entity.Property(x => x.Username)
                            .HasMaxLength(100)
                            .IsRequired();

                        entity.Property(x => x.PasswordHash)
                            .HasMaxLength(500)
                            .IsRequired();

                        entity.HasIndex(x => x.Username)
                            .IsUnique();

                        entity.HasOne(x => x.Company)
                            .WithMany()
                            .HasForeignKey(x => x.CompanyId)
                            .OnDelete(DeleteBehavior.Restrict);

                        entity.HasOne(x => x.Role)
                            .WithMany(x => x.Users)
                            .HasForeignKey(x => x.RoleId)
                            .OnDelete(DeleteBehavior.Restrict);
                    });

        }

    }
}
