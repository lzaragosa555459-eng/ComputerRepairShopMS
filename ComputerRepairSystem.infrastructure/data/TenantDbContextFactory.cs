using ComputerRepairSystem.company.Data;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem.infrastructure.data;

public class TenantDbContextFactory : ITenantDbContextFactory
{
    private readonly MasterErpDbContext _masterDb;

    public TenantDbContextFactory(MasterErpDbContext masterDb)
    {
        _masterDb = masterDb;
    }

    public async Task<TenantDbContext> CreateAsync(int companyId)
    {
        if (companyId <= 0)
        {
            throw new InvalidOperationException(
                "No company is assigned to the current user.");
        }

        var companyDatabase =
            await _masterDb.CompanyDatabases
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CompanyId == companyId &&
                    x.IsActive);

        if (companyDatabase == null)
        {
            throw new InvalidOperationException(
                "No active tenant database was found for this company.");
        }
            
        var connectionString =
            $"Server={companyDatabase.ServerName};" +
            $"Database={companyDatabase.DatabaseName};" +
            "Trusted_Connection=True;" +
            "TrustServerCertificate=True;";

        var options =
            new DbContextOptionsBuilder<TenantDbContext>()
                .UseSqlServer(connectionString)
                .Options;
        System.Diagnostics.Debug.WriteLine(
            $"TENANT TEST: CompanyId={companyId}, " +
            $"Server={companyDatabase.ServerName}, " +
            $"Database={companyDatabase.DatabaseName}");

        return new TenantDbContext(options);
    }
}