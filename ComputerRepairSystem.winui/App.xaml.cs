using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Interfaces;
using ComputerRepairSystem.company.Repositories;
using ComputerRepairSystem.company.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace ComputerRepairSystem_winui;

public partial class App : Application
{
    private Window? _window;

    public static IServiceProvider Services { get; private set; } = null!;

    public App()
    {
        InitializeComponent();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        // Local tenant database
        services.AddDbContext<TenantDbContext>(
            options => options.UseSqlServer(
                configuration.GetConnectionString("TenantLocal")),
            ServiceLifetime.Singleton);

        // Repositories
        services.AddSingleton<ICustomerRepository, CustomerRepository>();

        // Application services
        services.AddSingleton<CustomerService>();

        // Windows
        services.AddSingleton<MainWindow>();

        // Build dependency injection container
        Services = services.BuildServiceProvider();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = Services.GetRequiredService<MainWindow>();
        _window.Activate();
    }
}