using ComputerRepairSystem.company.Interfaces;

namespace ComputerRepairSystem_winui.Services;

public class CompanyContext : ICompanyContext
{
    public int? CompanyId => CurrentUser.CompanyId;
}