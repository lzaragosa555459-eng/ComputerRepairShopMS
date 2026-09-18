using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;
using ComputerRepairSystem.infrastructure.data;
using ComputerRepairSystem_winui.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ComputerRepairSystem_winui.Pages;

public sealed partial class HomePage : Page
{
    private readonly TenantDbContextFactory
        _tenantDbFactory;

    public HomePage(
        TenantDbContextFactory tenantDbFactory)
    {
        InitializeComponent();

        _tenantDbFactory =
            tenantDbFactory;

        Loaded += HomePage_Loaded;
    }


    private async void HomePage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await LoadDashboardAsync();
    }


    private async Task LoadDashboardAsync()
    {
        try
        {
            if (CurrentUser.CompanyId == null)
            {
                return;
            }

            await using var db =
                await _tenantDbFactory.CreateAsync(
                    CurrentUser.CompanyId.Value);

            var settings =
                await db.SystemSettings
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

            ShopNameText.Text =
                settings?.ShopName ?? "Computer Repair Shop";


            // ==========================================
            // CUSTOMERS
            // ==========================================

            var customerCount =
                await db.Customers.CountAsync();


            // ==========================================
            // SERVICE REQUESTS
            // ==========================================

            var serviceRequestCount =
                await db.ServiceRequests.CountAsync();


            // ==========================================
            // ACTIVE REPAIRS
            // ==========================================

            var activeRepairCount =
                await db.Repairs
                    .CountAsync(r =>
                        r.Status != "Completed");


            // ==========================================
            // COMPLETED REPAIRS
            // ==========================================

            var completedRepairCount =
                await db.Repairs
                    .CountAsync(r =>
                        r.Status == "Completed");


            // ==========================================
            // TOTAL REVENUE
            // ==========================================

            var totalRevenue =
                await db.Invoices
                    .Where(i =>
                        i.Status == "Paid")
                    .Select(i =>
                        (decimal?)i.TotalAmount)
                    .SumAsync()
                ?? 0;


            // ==========================================
            // UPDATE UI
            // ==========================================

            CustomersCountText.Text =
                customerCount.ToString();

            ServiceRequestsCountText.Text =
                serviceRequestCount.ToString();

            ActiveRepairsCountText.Text =
                activeRepairCount.ToString();

            CompletedRepairsCountText.Text =
                completedRepairCount.ToString();

            TotalRevenueText.Text =
                $"₱{totalRevenue:N2}";


            //System Settings


        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                "Failed to load dashboard: " +
                ex);
        }
    }
}