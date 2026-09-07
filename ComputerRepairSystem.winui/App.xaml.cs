using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Interfaces;
using ComputerRepairSystem.company.Repositories;
using ComputerRepairSystem.company.Services;

using ComputerRepairSystem.infrastructure.Entities;
using ComputerRepairSystem_winui.Pages;
using ComputerRepairSystem.infrastructure.data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace ComputerRepairSystem_winui;

public partial class App : Application
{
    private Window? _window;

    public static IServiceProvider Services { get; private set; }
        = null!;

    public App()
    {
        InitializeComponent();


        // ==========================================
        // CONFIGURATION
        // ==========================================

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: true)
            .Build();


        var services = new ServiceCollection();


        // ==========================================
        // CONNECTION STRINGS
        // ==========================================

        var masterConnectionString =
            configuration.GetConnectionString("MasterLocal")
            ?? throw new InvalidOperationException(
                "Connection string 'MasterLocal' was not found.");


        var tenantConnectionString =
            configuration.GetConnectionString("TenantLocal")
            ?? throw new InvalidOperationException(
                "Connection string 'TenantLocal' was not found.");


        // ==========================================
        // MASTER DATABASE
        // ==========================================

        services.AddDbContext<MasterErpDbContext>(
            options =>
                options.UseSqlServer(
                    masterConnectionString));


        // ==========================================
        // ASP.NET IDENTITY
        // ==========================================

        services.AddIdentityCore<ApplicationUser>(
                options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;

                    options.Password.RequiredLength = 6;
                })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MasterErpDbContext>();


        // ==========================================
        // TENANT DATABASE
        // ==========================================

        services.AddDbContextFactory<TenantDbContext>(
            options =>
                options.UseSqlServer(
                    tenantConnectionString));


        // ==========================================
        // CUSTOMER REPOSITORY
        // ==========================================

        services.AddTransient<
            ICustomerRepository,
            CustomerRepository>();


        // ==========================================
        // SERVICES
        // ==========================================

        services.AddTransient<CustomerService>();


        // ==========================================
        // PAGES
        // ==========================================

        services.AddTransient<UserManagementPage>();


        // ==========================================
        // MAIN WINDOW
        // ==========================================

        services.AddSingleton<MainWindow>();


        Services = services.BuildServiceProvider();
    }


    protected override void OnLaunched(
        LaunchActivatedEventArgs args)
    {
        _window =
            Services.GetRequiredService<MainWindow>();

        _window.Activate();
    }
}