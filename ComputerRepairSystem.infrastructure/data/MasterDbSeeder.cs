using ComputerRepairSystem.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem.infrastructure.data;

public static class MasterDbSeeder
{
    public static async Task SeedAsync(
        MasterErpDbContext context)
    {
        // ============================
        // SAMPLE COMPANIES
        // ============================

        if (!await context.Companies.AnyAsync())
        {
            var companies = new List<Company>
            {
                new Company
                {
                    CompanyCode = "COMP001",
                    CompanyName = "TechFix Solutions"
                },

                new Company
                {
                    CompanyCode = "COMP002",
                    CompanyName = "PC Repair Hub"
                },

                new Company
                {
                    CompanyCode = "COMP003",
                    CompanyName = "Davao Computer Services"
                }
            };

            context.Companies.AddRange(companies);

            await context.SaveChangesAsync();
        }
    }
}