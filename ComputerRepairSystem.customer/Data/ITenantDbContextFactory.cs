using System.Threading.Tasks;

namespace ComputerRepairSystem.company.Data;

public interface ITenantDbContextFactory
{
    Task<TenantDbContext> CreateAsync(int companyId);
}