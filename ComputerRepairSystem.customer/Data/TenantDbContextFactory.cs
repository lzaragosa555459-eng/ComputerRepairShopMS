using ComputerRepairSystem.company.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ComputerRepairSystem.company.Data;

public class TenantDbContextFactory
    : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(
        string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<TenantDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=db66781.public.databaseasp.net;" +
            "Database=db66781;" +
            "User Id=db66781;" +
            "Password=YOUR_PASSWORD;" +
            "Encrypt=True;" +
            "TrustServerCertificate=True;");

        return new TenantDbContext(
            optionsBuilder.Options);
    }
}