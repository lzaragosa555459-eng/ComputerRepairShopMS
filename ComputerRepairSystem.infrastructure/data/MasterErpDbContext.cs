using ComputerRepairSystem.domain.entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerRepairSystem.domain.data
{
    public class MasterErpDbContext : IdentityDbContext
    {

        public DbSet<Company> Companies => Set<Company>();



        public DbSet<CompanyDatabase> CompanyDatabases => Set<CompanyDatabase>();


        public DbSet<DeviceSample> Devices => Set<DeviceSample>();

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

                }

    }
}
