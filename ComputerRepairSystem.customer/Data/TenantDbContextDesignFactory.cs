using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ComputerRepairSystem.company.Data;

public class TenantDbContextDesignFactory
    : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<TenantDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\TenantLocalDB;" +
            "Database=CRSMS_Tenant;" +
            "Trusted_Connection=True;" +
            "TrustServerCertificate=True;");

        return new TenantDbContext(
            optionsBuilder.Options);
    }
}