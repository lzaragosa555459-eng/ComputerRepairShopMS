using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ComputerRepairSystem_winui.Pages;

public sealed partial class RepairManagementPage : Page
{
    private readonly IDbContextFactory<TenantDbContext> _tenantDbFactory;

    public RepairManagementPage(
        IDbContextFactory<TenantDbContext> tenantDbFactory)
    {
        InitializeComponent();

        _tenantDbFactory = tenantDbFactory;

        Loaded += RepairManagementPage_Loaded;
    }


    private async void RepairManagementPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await LoadServiceRequestsAsync();
        await LoadRepairsAsync();
    }


    private async Task LoadServiceRequestsAsync()
    {
        try
        {
            await using var db =
                await _tenantDbFactory.CreateDbContextAsync();

            var requests =
                await db.ServiceRequests
                    .AsNoTracking()
                    .Include(x => x.Device)
                    .ThenInclude(x => x.Customer)
                    .OrderByDescending(
                        x => x.RequestDate)
                    .ToListAsync();

            ServiceRequestList.ItemsSource =
                requests;
        }
        catch (Exception ex)
        {
            ServiceRequestList.ItemsSource = null;

            // We don't have a status bar on this page yet.
            System.Diagnostics.Debug.WriteLine(
                "Failed to load service requests: "
                + ex);
        }
    }


    private async void DiagnoseButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.DataContext is not ServiceRequest request)
        {
            return;
        }

        var diagnosisBox = new TextBox
        {
            Header = "Diagnosis",
            PlaceholderText =
                "Enter the technician's findings...",
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 120
        };

        var repairDescriptionBox = new TextBox
        {
            Header = "Repair Description",
            PlaceholderText =
                "Describe the repair that will be performed...",
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 100
        };

        var content = new StackPanel
        {
            Spacing = 12
        };

        content.Children.Add(
            new TextBlock
            {
                Text = $"Service Request #{request.ServiceRequestId}",
                FontSize = 20,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });

        content.Children.Add(
            new TextBlock
            {
                Text =
                    $"Customer: " +
                    $"{request.Device.Customer.FirstName} " +
                    $"{request.Device.Customer.LastName}"
            });

        content.Children.Add(
            new TextBlock
            {
                Text =
                    $"Device: " +
                    $"{request.Device.Brand} " +
                    $"{request.Device.Model}"
            });

        content.Children.Add(
            new TextBlock
            {
                Text =
                    $"Reported Problem: " +
                    $"{request.Description}",
                TextWrapping = TextWrapping.Wrap
            });

        content.Children.Add(
            new TextBlock
            {
                Text = $"Priority: {request.Priority}"
            });

        content.Children.Add(diagnosisBox);
        content.Children.Add(repairDescriptionBox);

        var dialog = new ContentDialog
        {
            Title = "Diagnose Service Request",
            Content = content,

            PrimaryButtonText = "Proceed with Repair",
            SecondaryButtonText = "Cancel Request",
            CloseButtonText = "Close",

            DefaultButton = ContentDialogButton.Primary,

            XamlRoot = XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.None)
        {
            return;
        }

        // Customer does not approve the repair.
        if (result == ContentDialogResult.Secondary)
        {
            await CancelServiceRequestAsync(
                request.ServiceRequestId);

            return;
        }

        // Customer approves the repair.
        if (string.IsNullOrWhiteSpace(
                diagnosisBox.Text))
        {
            await ShowMessageAsync(
                "Diagnosis Required",
                "Please enter a diagnosis before proceeding.");

            return;
        }

        if (string.IsNullOrWhiteSpace(
                repairDescriptionBox.Text))
        {
            await ShowMessageAsync(
                "Repair Description Required",
                "Please enter the repair description before proceeding.");

            return;
        }

        await CreateRepairAsync(
            request,
            diagnosisBox.Text.Trim(),
            repairDescriptionBox.Text.Trim());
    }

    private async Task CancelServiceRequestAsync(
        int serviceRequestId)
    {
        try
        {
            await using var db =
                await _tenantDbFactory.CreateDbContextAsync();

            var request =
                await db.ServiceRequests
                    .FirstOrDefaultAsync(
                        x =>
                            x.ServiceRequestId ==
                            serviceRequestId);

            if (request == null)
            {
                await ShowMessageAsync(
                    "Not Found",
                    "The service request could not be found.");

                return;
            }

            request.Status = "Cancelled";

            await db.SaveChangesAsync();

            await LoadServiceRequestsAsync();

            await ShowMessageAsync(
                "Request Cancelled",
                "The service request has been cancelled.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Unable to Cancel Request",
                ex.Message);
        }
    }
    private async Task CreateRepairAsync(
    ServiceRequest request,
    string diagnosis,
    string repairDescription)
    {
        try
        {
            await using var db =
                await _tenantDbFactory.CreateDbContextAsync();

            var existingRepair =
                await db.Repairs
                    .AnyAsync(
                        x =>
                            x.ServiceRequestId ==
                            request.ServiceRequestId);

            if (existingRepair)
            {
                await ShowMessageAsync(
                    "Repair Already Exists",
                    "A repair already exists for this service request.");

                return;
            }

            var repair = new Repair
            {
                ServiceRequestId =
                    request.ServiceRequestId,

                Diagnosis =
                    diagnosis,

                RepairDescription =
                    repairDescription,

                Status =
                    "Pending",

                StartDate = null,

                EndDate = null,

                TechnicianId = null,

                BranchId = null
            };

            db.Repairs.Add(repair);

            request.Status =
                "Approved";

            await db.SaveChangesAsync();

            await LoadServiceRequestsAsync();

            await ShowMessageAsync(
                "Repair Created",
                "The repair was created successfully.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Unable to Create Repair",
                ex.Message);
        }
    }
    private async Task ShowMessageAsync(
    string title,
    string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = XamlRoot
        };

        await dialog.ShowAsync();
    }
    private async Task LoadRepairsAsync()
    {
        try
        {
            await using var db =
                await _tenantDbFactory.CreateDbContextAsync();

            var repairs =
                await db.Repairs
                    .AsNoTracking()
                    .Include(r => r.ServiceRequest)
                        .ThenInclude(sr => sr.Device)
                            .ThenInclude(d => d.Customer)
                    .OrderByDescending(
                        r => r.RepairId)
                    .ToListAsync();

            RepairList.ItemsSource = repairs;

            System.Diagnostics.Debug.WriteLine(
                $"Loaded {repairs.Count} repair(s).");
        }
        catch (Exception ex)
        {
            RepairList.ItemsSource = null;

            System.Diagnostics.Debug.WriteLine(
                "Failed to load repairs: " + ex);
        }
    }

    private async void OpenRepairButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.DataContext is not Repair repair)
        {
            return;
        }

        var content = new StackPanel
        {
            Spacing = 10
        };

        content.Children.Add(
            new TextBlock
            {
                Text = $"Repair #{repair.RepairId}",
                FontSize = 22,
                FontWeight =
                    Microsoft.UI.Text.FontWeights.SemiBold
            });

        content.Children.Add(
            new TextBlock
            {
                Text =
                    $"Service Request ID: " +
                    $"{repair.ServiceRequestId}"
            });

        if (repair.ServiceRequest?.Device?.Customer != null)
        {
            var customer =
                repair.ServiceRequest.Device.Customer;

            content.Children.Add(
                new TextBlock
                {
                    Text =
                        $"Customer: " +
                        $"{customer.FirstName} " +
                        $"{customer.LastName}"
                });
        }

        if (repair.ServiceRequest?.Device != null)
        {
            var device =
                repair.ServiceRequest.Device;

            content.Children.Add(
                new TextBlock
                {
                    Text =
                        $"Device: " +
                        $"{device.Brand} " +
                        $"{device.Model}"
                });
        }

        content.Children.Add(
            new TextBlock
            {
                Text =
                    $"Diagnosis: " +
                    $"{repair.Diagnosis ?? "Not provided"}",
                TextWrapping = TextWrapping.Wrap
            });

        content.Children.Add(
            new TextBlock
            {
                Text =
                    $"Repair Description: " +
                    $"{repair.RepairDescription ?? "Not provided"}",
                TextWrapping = TextWrapping.Wrap
            });

        content.Children.Add(
            new TextBlock
            {
                Text =
                    $"Status: {repair.Status}"
            });

        content.Children.Add(
            new TextBlock
            {
                Text =
                    $"Start Date: " +
                    $"{repair.StartDate?.ToString("g") ?? "Not started"}"
            });

        content.Children.Add(
            new TextBlock
            {
                Text =
                    $"End Date: " +
                    $"{repair.EndDate?.ToString("g") ?? "Not completed"}"
            });

        var dialog = new ContentDialog
        {
            Title = "Repair Details",
            Content = content,
            CloseButtonText = "Close",
            XamlRoot = XamlRoot
        };

        await dialog.ShowAsync();
    }


}