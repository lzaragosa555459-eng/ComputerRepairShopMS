using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Interfaces;
using ComputerRepairSystem.company.Repositories;
using ComputerRepairSystem.company.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ComputerRepairSystem_winforms;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        var connectionString =
            configuration.GetConnectionString("TenantLocal");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            MessageBox.Show(
                "TenantLocal connection string was not found.",
                "Configuration Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return;
        }

        services.AddDbContext<TenantDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<CustomerService>();

        services.AddTransient<lblFirstName>();

        using var serviceProvider = services.BuildServiceProvider();

        Application.Run(
            serviceProvider.GetRequiredService<lblFirstName>());
    }
}