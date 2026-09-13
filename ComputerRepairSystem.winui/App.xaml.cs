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

        services.AddTransient<LoginPage>();

        services.AddTransient<ServiceManagementPage>();

        services.AddTransient<UserManagementPage>();

        services.AddTransient<RepairManagementPage>();

        services.AddTransient<BillingPage>();

        services.AddTransient<InventoryManagementPage>();

        services.AddTransient<HomePage>();

        services.AddTransient<SettingsPage>();

        // ==========================================
        // MAIN WINDOW
        // ==========================================

        services.AddSingleton<MainWindow>();


        Services = services.BuildServiceProvider();
    }


    protected override async void OnLaunched(
        LaunchActivatedEventArgs args)
    {
        // Seed admin account / role
        await InitializeAdminAsync();

        /* Seed tenant customers and devices
        using var scope =
            Services.CreateScope();

        var dbFactory =
            scope.ServiceProvider
                .GetRequiredService<
                    IDbContextFactory<TenantDbContext>>();

        await using var db =
            await dbFactory.CreateDbContextAsync();

        await TenantDbSeeder.SeedAsync(db);*/

        // Start application
        _window =
            Services.GetRequiredService<MainWindow>();

        _window.Activate();
    }
    private async Task InitializeAdminAsync()
    {

        using var scope = Services.CreateScope();
        var roleManager =
            scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole("Admin"));
                }
        var userManager =
            scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

        var admin =
            await userManager.FindByNameAsync("admin");

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@techfix.com",
                EmailConfirmed = true,
                CompanyId = 4,
                IsActive = true
            };

            var createResult =
                await userManager.CreateAsync(
                    admin,
                    "Admin123!");

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"{error.Code}: {error.Description}");
                }

                return;
            }

            admin =
                await userManager.FindByNameAsync("admin");
        }
        else if (string.IsNullOrEmpty(admin.PasswordHash))
        {
            var passwordResult =
                await userManager.AddPasswordAsync(
                    admin,
                    "Admin123!");

            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"{error.Code}: {error.Description}");
                }

                return;
            }
        }

        // Make sure the admin has the Admin role
        if (!await userManager.IsInRoleAsync(
                admin,
                "Admin"))
        {
            await userManager.AddToRoleAsync(
                admin,
                "Admin");
        }
    }


}