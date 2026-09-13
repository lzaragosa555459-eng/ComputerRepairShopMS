using ComputerRepairSystem.infrastructure.data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem_winui.Services;

public class TenantConnectionResolver
{
    private readonly MasterErpDbContext _masterDb;

    public TenantConnectionResolver(
        MasterErpDbContext masterDb)
    {
        _masterDb = masterDb;
    }

    public async Task<string?> GetTenantConnectionStringAsync()
    {
        if (CurrentUser.CompanyId <= 0)
        {
            return null;
        }

        var companyDatabase =
            await _masterDb.CompanyDatabases
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.CompanyId == CurrentUser.CompanyId &&
                        x.IsActive);

        if (companyDatabase == null)
        {
            return null;
        }

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = companyDatabase.ServerName,
            InitialCatalog = companyDatabase.DatabaseName,
            IntegratedSecurity = true,
            TrustServerCertificate = true
        };

        return builder.ConnectionString;
    }
}