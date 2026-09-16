using ComputerRepairSystem.domain.Entities;
using ComputerRepairSystem.infrastructure.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ComputerRepairSystem_winui.Pages;

public sealed partial class CompanyManagementPage : Page
{
    private readonly MasterErpDbContext _masterDb;

    private List<CompanyRow> _companies = new();
    private CompanyRow? _selectedCompany;

    public CompanyManagementPage(
        MasterErpDbContext masterDb)
    {
        InitializeComponent();

        _masterDb = masterDb;

        Loaded += CompanyManagementPage_Loaded;
    }

    private async void CompanyManagementPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await LoadCompaniesAsync();
    }

    // ==========================================
    // DISPLAY MODEL
    // ==========================================

    private class CompanyRow
    {
        public Company Company { get; set; } = null!;

        public string CompanyCode =>
            Company.CompanyCode;

        public string CompanyName =>
            Company.CompanyName;

        public string ServerName { get; set; } =
            string.Empty;

        public string DatabaseName { get; set; } =
            string.Empty;

        public string Status =>
            Company.IsActive
                ? "Active"
                : "Inactive";
    }

    // ==========================================
    // LOAD COMPANIES
    // ==========================================

    private async Task LoadCompaniesAsync()
    {
        try
        {
            var companies =
                await _masterDb.Companies
                    .AsNoTracking()
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

            var companyDatabases =
                await _masterDb.CompanyDatabases
                    .AsNoTracking()
                    .ToListAsync();

            _companies =
                companies
                    .Select(company =>
                    {
                        var database =
                            companyDatabases
                                .FirstOrDefault(
                                    d =>
                                        d.CompanyId ==
                                        company.CompanyId &&
                                        d.IsActive);

                        return new CompanyRow
                        {
                            Company = company,

                            ServerName =
                                database?.ServerName
                                ?? "Not configured",

                            DatabaseName =
                                database?.DatabaseName
                                ?? "Not configured"
                        };
                    })
                    .ToList();

            CompanyList.ItemsSource =
                _companies;
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Error Loading Companies",
                ex.Message);
        }
    }

    // ==========================================
    // SELECT COMPANY
    // ==========================================

    private void CompanyList_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        _selectedCompany =
            CompanyList.SelectedItem as CompanyRow;
    }

    // ==========================================
    // ADD COMPANY
    // ==========================================

    private async void NewCompanyButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var codeBox = new TextBox
        {
            Header = "Company Code",
            PlaceholderText = "Example: COMP004"
        };

        var nameBox = new TextBox
        {
            Header = "Company Name",
            PlaceholderText = "Enter company name"
        };

        var serverBox = new TextBox
        {
            Header = "Server Name",
            PlaceholderText =
                @"Example: (localdb)\TenantLocalDB"
        };

        var databaseBox = new TextBox
        {
            Header = "Database Name",
            PlaceholderText = "Enter database name"
        };

        var panel = new StackPanel
        {
            Spacing = 10
        };

        panel.Children.Add(codeBox);
        panel.Children.Add(nameBox);
        panel.Children.Add(serverBox);
        panel.Children.Add(databaseBox);

        var dialog = new ContentDialog
        {
            Title = "Add Company",
            Content = panel,
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            DefaultButton =
                ContentDialogButton.Primary,
            XamlRoot = XamlRoot
        };

        var result =
            await dialog.ShowAsync();

        if (result != ContentDialogResult.Primary)
            return;

        if (string.IsNullOrWhiteSpace(codeBox.Text) ||
            string.IsNullOrWhiteSpace(nameBox.Text))
        {
            await ShowMessageAsync(
                "Validation",
                "Company Code and Company Name are required.");

            return;
        }

        if (string.IsNullOrWhiteSpace(serverBox.Text) ||
            string.IsNullOrWhiteSpace(databaseBox.Text))
        {
            await ShowMessageAsync(
                "Validation",
                "Server Name and Database Name are required.");

            return;
        }

        try
        {
            var code =
                codeBox.Text.Trim();

            var existingCompany =
                await _masterDb.Companies
                    .FirstOrDefaultAsync(
                        c => c.CompanyCode == code);

            if (existingCompany != null)
            {
                await ShowMessageAsync(
                    "Company Already Exists",
                    "That company code is already being used.");

                return;
            }

            var company = new Company
            {
                CompanyCode = code,

                CompanyName =
                    nameBox.Text.Trim(),

                IsActive = true
            };

            _masterDb.Companies.Add(company);

            await _masterDb.SaveChangesAsync();

            var companyDatabase =
                new CompanyDatabase
                {
                    CompanyId =
                        company.CompanyId,

                    ServerName =
                        serverBox.Text.Trim(),

                    DatabaseName =
                        databaseBox.Text.Trim(),

                    IsActive = true
                };

            _masterDb.CompanyDatabases.Add(
                companyDatabase);

            await _masterDb.SaveChangesAsync();

            await LoadCompaniesAsync();

            await ShowMessageAsync(
                "Success",
                "Company added successfully.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Error",
                ex.Message);
        }
    }

    // ==========================================
    // EDIT COMPANY
    // ==========================================

    private async void EditCompanyButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_selectedCompany == null)
        {
            await ShowMessageAsync(
                "No Company Selected",
                "Please select a company first.");

            return;
        }

        var company =
            await _masterDb.Companies
                .FirstOrDefaultAsync(
                    c =>
                        c.CompanyId ==
                        _selectedCompany.Company.CompanyId);

        if (company == null)
        {
            await ShowMessageAsync(
                "Company Not Found",
                "The selected company no longer exists.");

            await LoadCompaniesAsync();

            return;
        }

        var database =
            await _masterDb.CompanyDatabases
                .FirstOrDefaultAsync(
                    d =>
                        d.CompanyId ==
                        company.CompanyId &&
                        d.IsActive);

        var codeBox = new TextBox
        {
            Header = "Company Code",
            Text = company.CompanyCode
        };

        var nameBox = new TextBox
        {
            Header = "Company Name",
            Text = company.CompanyName
        };

        var serverBox = new TextBox
        {
            Header = "Server Name",
            Text = database?.ServerName ?? string.Empty
        };

        var databaseBox = new TextBox
        {
            Header = "Database Name",
            Text = database?.DatabaseName ?? string.Empty
        };

        var panel = new StackPanel
        {
            Spacing = 10
        };

        panel.Children.Add(codeBox);
        panel.Children.Add(nameBox);
        panel.Children.Add(serverBox);
        panel.Children.Add(databaseBox);

        var dialog = new ContentDialog
        {
            Title = "Edit Company",
            Content = panel,
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            DefaultButton =
                ContentDialogButton.Primary,
            XamlRoot = XamlRoot
        };

        var result =
            await dialog.ShowAsync();

        if (result != ContentDialogResult.Primary)
            return;

        if (string.IsNullOrWhiteSpace(codeBox.Text) ||
            string.IsNullOrWhiteSpace(nameBox.Text))
        {
            await ShowMessageAsync(
                "Validation",
                "Company Code and Company Name are required.");

            return;
        }

        try
        {
            company.CompanyCode =
                codeBox.Text.Trim();

            company.CompanyName =
                nameBox.Text.Trim();

            if (database == null)
            {
                database =
                    new CompanyDatabase
                    {
                        CompanyId =
                            company.CompanyId,

                        IsActive = true
                    };

                _masterDb.CompanyDatabases.Add(
                    database);
            }

            database.ServerName =
                serverBox.Text.Trim();

            database.DatabaseName =
                databaseBox.Text.Trim();

            await _masterDb.SaveChangesAsync();

            await LoadCompaniesAsync();

            await ShowMessageAsync(
                "Success",
                "Company updated successfully.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Error",
                ex.Message);
        }
    }

    // ==========================================
    // ACTIVATE / DEACTIVATE
    // ==========================================

    private async void ToggleCompanyButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_selectedCompany == null)
        {
            await ShowMessageAsync(
                "No Company Selected",
                "Please select a company first.");

            return;
        }

        var company =
            await _masterDb.Companies
                .FirstOrDefaultAsync(
                    c =>
                        c.CompanyId ==
                        _selectedCompany.Company.CompanyId);

        if (company == null)
            return;

        company.IsActive =
            !company.IsActive;

        await _masterDb.SaveChangesAsync();

        await LoadCompaniesAsync();

        await ShowMessageAsync(
            "Company Updated",
            company.IsActive
                ? "Company activated."
                : "Company deactivated.");
    }

    // ==========================================
    // MESSAGE
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