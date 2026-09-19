using ComputerRepairSystem.domain.Entities;
using ComputerRepairSystem.infrastructure.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace ComputerRepairSystem_winui.Pages;

public sealed partial class SubscriptionManagementPage : Page
{
    private readonly MasterErpDbContext _masterDb;

    private readonly ObservableCollection<SubscriptionPlan> _plans = new();

    private readonly ObservableCollection<ModuleSelectionItem> _modules = new();

    private readonly ObservableCollection<SubscriptionDisplayItem> _subscriptions = new();


    // ==========================================
    // CONSTRUCTOR
    // ==========================================

    public SubscriptionManagementPage(
        MasterErpDbContext masterDb)
    {
        this.InitializeComponent();

        _masterDb = masterDb;

        PlansListView.ItemsSource = _plans;

        ModulesListView.ItemsSource = _modules;

        CompanySubscriptionsListView.ItemsSource =
            _subscriptions;

        Loaded += SubscriptionManagementPage_Loaded;
    }


    // ==========================================
    // PAGE LOADED
    // ==========================================

    private async void SubscriptionManagementPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await LoadPlansAsync();

        await LoadCompanySubscriptionsAsync();
    }


    // ==========================================
    // LOAD PLANS
    // ==========================================

    private async Task LoadPlansAsync()
    {
        _plans.Clear();

        var plans =
            await _masterDb.SubscriptionPlans
                .OrderBy(x => x.Price)
                .ThenBy(x => x.PlanName)
                .ToListAsync();

        foreach (var plan in plans)
        {
            _plans.Add(plan);
        }
    }


    // ==========================================
    // PLAN SELECTION
    // ==========================================

    private async void PlansListView_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (PlansListView.SelectedItem
            is not SubscriptionPlan selectedPlan)
        {
            _modules.Clear();

            return;
        }

        await LoadModulesAsync(
            selectedPlan.SubscriptionPlanId);
    }


    // ==========================================
    // LOAD MODULES
    // ==========================================

    private async Task LoadModulesAsync(
        int subscriptionPlanId)
    {
        _modules.Clear();

        var modules =
            await _masterDb.ModuleDefinitions
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

        var includedModuleIds =
            await _masterDb.SubscriptionPlanModules
                .Where(x =>
                    x.SubscriptionPlanId ==
                    subscriptionPlanId)
                .Select(x => x.ModuleDefinitionId)
                .ToListAsync();

        foreach (var module in modules)
        {
            _modules.Add(
                new ModuleSelectionItem
                {
                    ModuleDefinitionId =
                        module.ModuleDefinitionId,

                    ModuleName =
                        module.ModuleName,

                    IsIncluded =
                        includedModuleIds.Contains(
                            module.ModuleDefinitionId)
                });
        }
    }


    // ==========================================
    // ADD PLAN
    // ==========================================

    private async void AddPlanButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var planNameBox = new TextBox
        {
            Header = "Plan Name",
            PlaceholderText = "Enter plan name"
        };


        var priceBox = new NumberBox
        {
            Header = "Price",
            PlaceholderText = "Enter plan price",
            Minimum = 0,
            SmallChange = 100,
            SpinButtonPlacementMode =
                NumberBoxSpinButtonPlacementMode.Compact
        };


        var durationBox = new NumberBox
        {
            Header = "Duration (days)",
            PlaceholderText = "Enter duration",
            Minimum = 1,
            Value = 30,
            SpinButtonPlacementMode =
                NumberBoxSpinButtonPlacementMode.Compact
        };


        var activeCheckBox = new CheckBox
        {
            Content = "Active",
            IsChecked = true
        };


        var panel = new StackPanel
        {
            Spacing = 12
        };

        panel.Children.Add(planNameBox);
        panel.Children.Add(priceBox);
        panel.Children.Add(durationBox);
        panel.Children.Add(activeCheckBox);


        var dialog = new ContentDialog
        {
            Title = "Add Subscription Plan",

            Content = panel,

            PrimaryButtonText = "Add",

            CloseButtonText = "Cancel",

            DefaultButton =
                ContentDialogButton.Primary,

            XamlRoot = XamlRoot
        };


        var result =
            await dialog.ShowAsync();


        if (result != ContentDialogResult.Primary)
            return;


        var planName =
            planNameBox.Text.Trim();


        if (string.IsNullOrWhiteSpace(planName))
        {
            await ShowMessageAsync(
                "Validation Error",
                "Plan name is required.");

            return;
        }


        if (priceBox.Value < 0)
        {
            await ShowMessageAsync(
                "Validation Error",
                "Price cannot be negative.");

            return;
        }


        if (durationBox.Value < 1)
        {
            await ShowMessageAsync(
                "Validation Error",
                "Duration must be at least 1 day.");

            return;
        }


        var existingPlan =
            await _masterDb.SubscriptionPlans
                .FirstOrDefaultAsync(x =>
                    x.PlanName == planName);


        if (existingPlan != null)
        {
            await ShowMessageAsync(
                "Plan Already Exists",
                "A subscription plan with that name already exists.");

            return;
        }


        var plan = new SubscriptionPlan
        {
            PlanName = planName,

            Price =
                (decimal)priceBox.Value,

            DurationInDays =
                (int)durationBox.Value,

            IsActive =
                activeCheckBox.IsChecked == true
        };


        _masterDb.SubscriptionPlans.Add(plan);

        await _masterDb.SaveChangesAsync();


        await LoadPlansAsync();


        await ShowMessageAsync(
            "Plan Added",
            $"Subscription plan '{plan.PlanName}' was added successfully.");
    }


    // ==========================================
    // SAVE PLAN MODULES
    // ==========================================

    private async void SaveModulesButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (PlansListView.SelectedItem
            is not SubscriptionPlan selectedPlan)
        {
            await ShowMessageAsync(
                "No Plan Selected",
                "Please select a subscription plan first.");

            return;
        }


        try
        {
            var existingModules =
                await _masterDb.SubscriptionPlanModules
                    .Where(x =>
                        x.SubscriptionPlanId ==
                        selectedPlan.SubscriptionPlanId)
                    .ToListAsync();


            _masterDb.SubscriptionPlanModules
                .RemoveRange(existingModules);


            foreach (var module in
                _modules.Where(x => x.IsIncluded))
            {
                _masterDb.SubscriptionPlanModules.Add(
                    new SubscriptionPlanModule
                    {
                        SubscriptionPlanId =
                            selectedPlan.SubscriptionPlanId,

                        ModuleDefinitionId =
                            module.ModuleDefinitionId
                    });
            }


            await _masterDb.SaveChangesAsync();


            await LoadModulesAsync(
                selectedPlan.SubscriptionPlanId);


            await ShowMessageAsync(
                "Modules Saved",
                $"Modules for '{selectedPlan.PlanName}' were updated successfully.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Save Failed",
                ex.Message);
        }
    }


    // ==========================================
    // LOAD COMPANY SUBSCRIPTIONS
    // ==========================================

    private async Task LoadCompanySubscriptionsAsync()
    {
        _subscriptions.Clear();

        var subscriptions =
            await _masterDb.Subscriptions
                .Include(x => x.Company)
                .Include(x => x.SubscriptionPlan)
                .OrderByDescending(x => x.StartDate)
                .ToListAsync();


        foreach (var subscription in subscriptions)
        {
            _subscriptions.Add(
                new SubscriptionDisplayItem
                {
                    CompanyName =
                        subscription.Company?.CompanyName
                        ?? "Unknown Company",

                    PlanName =
                        subscription.SubscriptionPlan?.PlanName
                        ?? "Unknown Plan",

                    Status =
                        subscription.Status,

                    StartDateText =
                        subscription.StartDate
                            .ToLocalTime()
                            .ToString("MMM dd, yyyy"),

                    EndDateText =
                        subscription.EndDate.HasValue
                            ? subscription.EndDate.Value
                                .ToLocalTime()
                                .ToString("MMM dd, yyyy")
                            : "No End Date"
                });
        }
    }


    // ==========================================
    // ADD COMPANY SUBSCRIPTION
    // ==========================================

    private async void AddSubscriptionButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var companies =
            await _masterDb.Companies
                .Where(x => x.IsActive)
                .OrderBy(x => x.CompanyName)
                .ToListAsync();


        var plans =
            await _masterDb.SubscriptionPlans
                .Where(x => x.IsActive)
                .OrderBy(x => x.Price)
                .ThenBy(x => x.PlanName)
                .ToListAsync();


        if (companies.Count == 0)
        {
            await ShowMessageAsync(
                "No Companies",
                "There are no active companies available.");

            return;
        }


        if (plans.Count == 0)
        {
            await ShowMessageAsync(
                "No Subscription Plans",
                "There are no active subscription plans available.");

            return;
        }


        var companyBox = new ComboBox
        {
            Header = "Company",
            PlaceholderText = "Select a company",
            DisplayMemberPath = "CompanyName"
        };


        foreach (var company in companies)
        {
            companyBox.Items.Add(company);
        }


        companyBox.SelectedIndex = 0;


        var planBox = new ComboBox
        {
            Header = "Subscription Plan",
            PlaceholderText = "Select a plan",
            DisplayMemberPath = "PlanName"
        };


        foreach (var plan in plans)
        {
            planBox.Items.Add(plan);
        }


        planBox.SelectedIndex = 0;


        var panel = new StackPanel
        {
            Spacing = 12
        };


        panel.Children.Add(companyBox);

        panel.Children.Add(planBox);


        var dialog = new ContentDialog
        {
            Title = "Add Company Subscription",

            Content = panel,

            PrimaryButtonText = "Activate",

            CloseButtonText = "Cancel",

            DefaultButton =
                ContentDialogButton.Primary,

            XamlRoot = XamlRoot
        };


        var result =
            await dialog.ShowAsync();


        if (result != ContentDialogResult.Primary)
            return;


        if (companyBox.SelectedItem
            is not Company selectedCompany)
        {
            await ShowMessageAsync(
                "Validation Error",
                "Please select a company.");

            return;
        }


        if (planBox.SelectedItem
            is not SubscriptionPlan selectedPlan)
        {
            await ShowMessageAsync(
                "Validation Error",
                "Please select a subscription plan.");

            return;
        }


        var existingActiveSubscription =
            await _masterDb.Subscriptions
                .AnyAsync(x =>
                    x.CompanyId ==
                        selectedCompany.CompanyId
                    && x.Status == "Active"
                    && (
                        x.EndDate == null
                        || x.EndDate >
                            DateTime.UtcNow));


        if (existingActiveSubscription)
        {
            await ShowMessageAsync(
                "Active Subscription Exists",
                $"'{selectedCompany.CompanyName}' already has an active subscription.");

            return;
        }


        var startDate =
            DateTime.UtcNow;


        var subscription =
            new Subscription
            {
                CompanyId =
                    selectedCompany.CompanyId,

                SubscriptionPlanId =
                    selectedPlan.SubscriptionPlanId,

                StartDate =
                    startDate,

                EndDate =
                    startDate.AddDays(
                        selectedPlan.DurationInDays),

                TrialEndsAt =
                    selectedPlan.PlanName.Equals(
                        "Trial",
                        StringComparison.OrdinalIgnoreCase)
                        ? startDate.AddDays(
                            selectedPlan.DurationInDays)
                        : null,

                Status = "Active"
            };


        _masterDb.Subscriptions.Add(
            subscription);


        await _masterDb.SaveChangesAsync();


        await LoadCompanySubscriptionsAsync();


        await ShowMessageAsync(
            "Subscription Activated",
            $"'{selectedCompany.CompanyName}' is now subscribed to '{selectedPlan.PlanName}'.");
    }


    // ==========================================
    // MESSAGE DIALOG
    // ==========================================

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
}


// ==============================================
// MODULE SELECTION UI MODEL
// ==============================================

public class ModuleSelectionItem
{
    public int ModuleDefinitionId { get; set; }

    public string ModuleName { get; set; }
        = string.Empty;

    public bool IsIncluded { get; set; }
}


// ==============================================
// SUBSCRIPTION DISPLAY UI MODEL
// ==============================================

public class SubscriptionDisplayItem
{
    public string CompanyName { get; set; }
        = string.Empty;

    public string PlanName { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public string StartDateText { get; set; }
        = string.Empty;

    public string EndDateText { get; set; }
        = string.Empty;
}