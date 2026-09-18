using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;
using ComputerRepairSystem.infrastructure.data;
using ComputerRepairSystem_winui.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ComputerRepairSystem_winui.Pages;

public sealed partial class PayrollManagementPage : Page
{
    private readonly TenantDbContextFactory _tenantDbFactory;

    public PayrollManagementPage(
        TenantDbContextFactory tenantDbFactory)
    {
        InitializeComponent();

        _tenantDbFactory = tenantDbFactory;

        Loaded += PayrollManagementPage_Loaded;
    }


    // ==========================================
    // PAGE LOADED
    // ==========================================

    private async void PayrollManagementPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await LoadPayrollsAsync();
    }


    // ==========================================
    // LOAD PAYROLLS
    // ==========================================

    private async Task LoadPayrollsAsync()
    {
        try
        {
            if (CurrentUser.CompanyId == null)
            {
                PayrollList.ItemsSource = null;
                return;
            }

            await using var db =
                await _tenantDbFactory.CreateAsync(
                    CurrentUser.CompanyId.Value);

            var payrolls =
                await db.Payrolls
                    .AsNoTracking()
                    .Include(x => x.Employee)
                    .OrderByDescending(x => x.PayPeriodEnd)
                    .ThenBy(x => x.Employee!.LastName)
                    .ToListAsync();

            var rows =
                payrolls
                    .Select(x => new PayrollRow
                    {
                        PayrollId = x.PayrollId,

                        EmployeeName =
                            x.Employee == null
                                ? "Unknown Employee"
                                : $"{x.Employee.FirstName} {x.Employee.LastName}",

                        PayPeriodStartText =
                            x.PayPeriodStart.ToString("MM/dd/yyyy"),

                        PayPeriodEndText =
                            x.PayPeriodEnd.ToString("MM/dd/yyyy"),

                        BasicSalaryText =
                            $"₱{x.BasicSalary:N2}",

                        DeductionsText =
                            $"₱{x.Deductions:N2}",

                        NetSalaryText =
                            $"₱{x.NetSalary:N2}",

                        EmployeeId = x.EmployeeId,

                        PayPeriodStart = x.PayPeriodStart,

                        PayPeriodEnd = x.PayPeriodEnd,

                        BasicSalary = x.BasicSalary,

                        Deductions = x.Deductions
                    })
                    .ToList();

            PayrollList.ItemsSource = rows;
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Error Loading Payroll",
                ex.Message);
        }
    }


    // ==========================================
    // SEARCH
    // ==========================================

    private async void PayrollSearchBox_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        if (CurrentUser.CompanyId == null)
        {
            PayrollList.ItemsSource = null;
            return;
        }

        try
        {
            await using var db =
                await _tenantDbFactory.CreateAsync(
                    CurrentUser.CompanyId.Value);

            var payrolls =
                await db.Payrolls
                    .AsNoTracking()
                    .Include(x => x.Employee)
                    .OrderByDescending(x => x.PayPeriodEnd)
                    .ThenBy(x => x.Employee!.LastName)
                    .ToListAsync();

            var search =
                PayrollSearchBox.Text.Trim();

            if (!string.IsNullOrWhiteSpace(search))
            {
                payrolls =
                    payrolls
                        .Where(x =>
                            x.Employee != null &&
                            $"{x.Employee.FirstName} {x.Employee.LastName}"
                                .Contains(
                                    search,
                                    StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }

            var rows =
                payrolls
                    .Select(x => new PayrollRow
                    {
                        PayrollId = x.PayrollId,

                        EmployeeName =
                            x.Employee == null
                                ? "Unknown Employee"
                                : $"{x.Employee.FirstName} {x.Employee.LastName}",

                        PayPeriodStartText =
                            x.PayPeriodStart.ToString("MM/dd/yyyy"),

                        PayPeriodEndText =
                            x.PayPeriodEnd.ToString("MM/dd/yyyy"),

                        BasicSalaryText =
                            $"₱{x.BasicSalary:N2}",

                        DeductionsText =
                            $"₱{x.Deductions:N2}",

                        NetSalaryText =
                            $"₱{x.NetSalary:N2}",

                        EmployeeId = x.EmployeeId,

                        PayPeriodStart = x.PayPeriodStart,

                        PayPeriodEnd = x.PayPeriodEnd,

                        BasicSalary = x.BasicSalary,

                        Deductions = x.Deductions
                    })
                    .ToList();

            PayrollList.ItemsSource = rows;
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Search Error",
                ex.Message);
        }
    }


    // ==========================================
    // ADD PAYROLL
    // ==========================================

    private async void AddPayrollButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (CurrentUser.CompanyId == null)
        {
            await ShowMessageAsync(
                "Validation Error",
                "Unable to determine the current company.");

            return;
        }

        try
        {
            await using var db =
                await _tenantDbFactory.CreateAsync(
                    CurrentUser.CompanyId.Value);

            var employees =
                await db.Employees
                    .AsNoTracking()
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .ToListAsync();

            if (employees.Count == 0)
            {
                await ShowMessageAsync(
                    "No Employees",
                    "There are no active employees available for payroll.");

                return;
            }


            var employeeBox =
                new ComboBox
                {
                    Header = "Employee",
                    PlaceholderText = "Select employee",
                    ItemsSource = employees,
                    DisplayMemberPath = "LastName",
                    MinWidth = 320
                };


            var startDatePicker =
                new CalendarDatePicker
                {
                    Header = "Pay Period Start",
                    Date = DateTimeOffset.Now
                };


            var endDatePicker =
                new CalendarDatePicker
                {
                    Header = "Pay Period End",
                    Date = DateTimeOffset.Now
                };


            var basicSalaryBox =
                new NumberBox
                {
                    Header = "Basic Salary",
                    PlaceholderText = "Enter basic salary",
                    Minimum = 0,
                    SpinButtonPlacementMode =
                        NumberBoxSpinButtonPlacementMode.Compact
                };


            var deductionsBox =
                new NumberBox
                {
                    Header = "Deductions",
                    PlaceholderText = "Enter deductions",
                    Minimum = 0,
                    Value = 0,
                    SpinButtonPlacementMode =
                        NumberBoxSpinButtonPlacementMode.Compact
                };


            var netSalaryText =
                new TextBlock
                {
                    Text = "Net Salary: ₱0.00",
                    FontSize = 18,
                    FontWeight =
                        Microsoft.UI.Text.FontWeights.SemiBold
                };


            void UpdateNetSalary()
            {
                var basic =
                    double.IsNaN(basicSalaryBox.Value)
                        ? 0
                        : basicSalaryBox.Value;

                var deductions =
                    double.IsNaN(deductionsBox.Value)
                        ? 0
                        : deductionsBox.Value;

                var net =
                    Math.Max(0, basic - deductions);

                netSalaryText.Text =
                    $"Net Salary: ₱{net:N2}";
            }


            basicSalaryBox.ValueChanged +=
                (_, _) => UpdateNetSalary();

            deductionsBox.ValueChanged +=
                (_, _) => UpdateNetSalary();


            var panel =
                new StackPanel
                {
                    Spacing = 12
                };

            panel.Children.Add(employeeBox);
            panel.Children.Add(startDatePicker);
            panel.Children.Add(endDatePicker);
            panel.Children.Add(basicSalaryBox);
            panel.Children.Add(deductionsBox);
            panel.Children.Add(netSalaryText);


            var dialog =
                new ContentDialog
                {
                    Title = "Add Payroll",
                    Content = panel,

                    PrimaryButtonText = "Add",
                    CloseButtonText = "Cancel",

                    DefaultButton =
                        ContentDialogButton.Primary,

                    XamlRoot = XamlRoot
                };


            var result =
                await dialog.ShowAsync();

            if (result !=
                ContentDialogResult.Primary)
            {
                return;
            }


            // ==========================================
            // VALIDATION
            // ==========================================

            if (employeeBox.SelectedItem
                is not Employee selectedEmployee)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Please select an employee.");

                return;
            }


            if (!startDatePicker.Date.HasValue ||
                !endDatePicker.Date.HasValue)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Please select both pay period dates.");

                return;
            }


            var payPeriodStart =
                startDatePicker.Date.Value.DateTime.Date;

            var payPeriodEnd =
                endDatePicker.Date.Value.DateTime.Date;


            if (payPeriodEnd < payPeriodStart)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Pay period end cannot be earlier than the start date.");

                return;
            }


            var basicSalary =
                (decimal)(
                    double.IsNaN(basicSalaryBox.Value)
                        ? 0
                        : basicSalaryBox.Value);


            var deductions =
                (decimal)(
                    double.IsNaN(deductionsBox.Value)
                        ? 0
                        : deductionsBox.Value);


            if (basicSalary <= 0)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Basic salary must be greater than zero.");

                return;
            }


            if (deductions < 0)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Deductions cannot be negative.");

                return;
            }


            if (deductions > basicSalary)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Deductions cannot be greater than the basic salary.");

                return;
            }


            // ==========================================
            // CHECK DUPLICATE PAYROLL
            // ==========================================

            var duplicate =
                await db.Payrolls
                    .AnyAsync(x =>
                        x.EmployeeId ==
                            selectedEmployee.EmployeeId &&

                        x.PayPeriodStart ==
                            payPeriodStart &&

                        x.PayPeriodEnd ==
                            payPeriodEnd);

            if (duplicate)
            {
                await ShowMessageAsync(
                    "Duplicate Payroll",
                    "A payroll record already exists for this employee and pay period.");

                return;
            }


            // ==========================================
            // CREATE PAYROLL
            // ==========================================

            var payroll =
                new Payroll
                {
                    EmployeeId =
                        selectedEmployee.EmployeeId,

                    PayPeriodStart =
                        payPeriodStart,

                    PayPeriodEnd =
                        payPeriodEnd,

                    BasicSalary =
                        basicSalary,

                    Deductions =
                        deductions,

                    NetSalary =
                        basicSalary - deductions
                };


            db.Payrolls.Add(payroll);

            await db.SaveChangesAsync();


            await LoadPayrollsAsync();


            await ShowMessageAsync(
                "Payroll Added",
                "The payroll record was added successfully.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Error Adding Payroll",
                ex.Message);
        }
    }


    // ==========================================
    // EDIT PAYROLL
    // ==========================================

    private async void EditPayrollButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Tag is not PayrollRow row)
        {
            return;
        }

        if (CurrentUser.CompanyId == null)
        {
            return;
        }

        try
        {
            await using var db =
                await _tenantDbFactory.CreateAsync(
                    CurrentUser.CompanyId.Value);

            var payroll =
                await db.Payrolls
                    .Include(x => x.Employee)
                    .FirstOrDefaultAsync(
                        x => x.PayrollId == row.PayrollId);

            if (payroll == null)
            {
                await ShowMessageAsync(
                    "Not Found",
                    "The payroll record could not be found.");

                return;
            }


            var employees =
                await db.Employees
                    .AsNoTracking()
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .ToListAsync();


            var employeeBox =
                new ComboBox
                {
                    Header = "Employee",
                    ItemsSource = employees,
                    DisplayMemberPath = "LastName",
                    MinWidth = 320,
                    SelectedValuePath = "EmployeeId",
                    SelectedValue = payroll.EmployeeId
                };


            var startDatePicker =
                new CalendarDatePicker
                {
                    Header = "Pay Period Start",
                    Date =
                        new DateTimeOffset(
                            payroll.PayPeriodStart)
                };


            var endDatePicker =
                new CalendarDatePicker
                {
                    Header = "Pay Period End",
                    Date =
                        new DateTimeOffset(
                            payroll.PayPeriodEnd)
                };


            var basicSalaryBox =
                new NumberBox
                {
                    Header = "Basic Salary",
                    Value =
                        (double)payroll.BasicSalary,
                    Minimum = 0,
                    SpinButtonPlacementMode =
                        NumberBoxSpinButtonPlacementMode.Compact
                };


            var deductionsBox =
                new NumberBox
                {
                    Header = "Deductions",
                    Value =
                        (double)payroll.Deductions,
                    Minimum = 0,
                    SpinButtonPlacementMode =
                        NumberBoxSpinButtonPlacementMode.Compact
                };


            var netSalaryText =
                new TextBlock
                {
                    Text =
                        $"Net Salary: ₱{payroll.NetSalary:N2}",
                    FontSize = 18,
                    FontWeight =
                        Microsoft.UI.Text.FontWeights.SemiBold
                };


            void UpdateNetSalary()
            {
                var basic =
                    double.IsNaN(basicSalaryBox.Value)
                        ? 0
                        : basicSalaryBox.Value;

                var deductions =
                    double.IsNaN(deductionsBox.Value)
                        ? 0
                        : deductionsBox.Value;

                var net =
                    Math.Max(0, basic - deductions);

                netSalaryText.Text =
                    $"Net Salary: ₱{net:N2}";
            }


            basicSalaryBox.ValueChanged +=
                (_, _) => UpdateNetSalary();

            deductionsBox.ValueChanged +=
                (_, _) => UpdateNetSalary();


            var panel =
                new StackPanel
                {
                    Spacing = 12
                };

            panel.Children.Add(employeeBox);
            panel.Children.Add(startDatePicker);
            panel.Children.Add(endDatePicker);
            panel.Children.Add(basicSalaryBox);
            panel.Children.Add(deductionsBox);
            panel.Children.Add(netSalaryText);


            var dialog =
                new ContentDialog
                {
                    Title = "Edit Payroll",
                    Content = panel,

                    PrimaryButtonText = "Save",
                    CloseButtonText = "Cancel",

                    DefaultButton =
                        ContentDialogButton.Primary,

                    XamlRoot = XamlRoot
                };


            var result =
                await dialog.ShowAsync();

            if (result !=
                ContentDialogResult.Primary)
            {
                return;
            }


            if (employeeBox.SelectedValue
                is not int employeeId)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Please select an employee.");

                return;
            }


            if (!startDatePicker.Date.HasValue ||
                !endDatePicker.Date.HasValue)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Please select both pay period dates.");

                return;
            }


            var payPeriodStart =
                startDatePicker.Date.Value.DateTime.Date;

            var payPeriodEnd =
                endDatePicker.Date.Value.DateTime.Date;


            if (payPeriodEnd < payPeriodStart)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Pay period end cannot be earlier than the start date.");

                return;
            }


            var basicSalary =
                (decimal)(
                    double.IsNaN(basicSalaryBox.Value)
                        ? 0
                        : basicSalaryBox.Value);


            var deductions =
                (decimal)(
                    double.IsNaN(deductionsBox.Value)
                        ? 0
                        : deductionsBox.Value);


            if (basicSalary <= 0)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Basic salary must be greater than zero.");

                return;
            }


            if (deductions > basicSalary)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Deductions cannot be greater than the basic salary.");

                return;
            }


            var duplicate =
                await db.Payrolls
                    .AnyAsync(x =>
                        x.PayrollId !=
                            payroll.PayrollId &&

                        x.EmployeeId ==
                            employeeId &&

                        x.PayPeriodStart ==
                            payPeriodStart &&

                        x.PayPeriodEnd ==
                            payPeriodEnd);

            if (duplicate)
            {
                await ShowMessageAsync(
                    "Duplicate Payroll",
                    "Another payroll record already exists for this employee and pay period.");

                return;
            }


            payroll.EmployeeId =
                employeeId;

            payroll.PayPeriodStart =
                payPeriodStart;

            payroll.PayPeriodEnd =
                payPeriodEnd;

            payroll.BasicSalary =
                basicSalary;

            payroll.Deductions =
                deductions;

            payroll.NetSalary =
                basicSalary - deductions;


            await db.SaveChangesAsync();


            await LoadPayrollsAsync();


            await ShowMessageAsync(
                "Payroll Updated",
                "The payroll record was updated successfully.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Error Updating Payroll",
                ex.Message);
        }
    }


    // ==========================================
    // DELETE PAYROLL
    // ==========================================

    private async void DeletePayrollButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Tag is not PayrollRow row)
        {
            return;
        }

        if (CurrentUser.CompanyId == null)
        {
            return;
        }

        try
        {
            var confirm =
                new ContentDialog
                {
                    Title = "Delete Payroll",
                    Content =
                        $"Delete payroll #{row.PayrollId} for {row.EmployeeName}?",

                    PrimaryButtonText = "Delete",
                    CloseButtonText = "Cancel",

                    DefaultButton =
                        ContentDialogButton.Close,

                    XamlRoot = XamlRoot
                };


            var result =
                await confirm.ShowAsync();

            if (result !=
                ContentDialogResult.Primary)
            {
                return;
            }


            await using var db =
                await _tenantDbFactory.CreateAsync(
                    CurrentUser.CompanyId.Value);

            var payroll =
                await db.Payrolls
                    .FirstOrDefaultAsync(
                        x =>
                            x.PayrollId ==
                            row.PayrollId);

            if (payroll == null)
            {
                await ShowMessageAsync(
                    "Not Found",
                    "The payroll record could not be found.");

                return;
            }


            db.Payrolls.Remove(payroll);

            await db.SaveChangesAsync();


            await LoadPayrollsAsync();


            await ShowMessageAsync(
                "Payroll Deleted",
                "The payroll record was deleted successfully.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Error Deleting Payroll",
                ex.Message);
        }
    }


    // ==========================================
    // MESSAGE DIALOG
    // ==========================================

    private async Task ShowMessageAsync(
        string title,
        string message)
    {
        var dialog =
            new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = XamlRoot
            };

        await dialog.ShowAsync();
    }


    // ==========================================
    // DISPLAY ROW
    // ==========================================

    private sealed class PayrollRow
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } =
            string.Empty;

        public string PayPeriodStartText { get; set; } =
            string.Empty;

        public string PayPeriodEndText { get; set; } =
            string.Empty;

        public string BasicSalaryText { get; set; } =
            string.Empty;

        public string DeductionsText { get; set; } =
            string.Empty;

        public string NetSalaryText { get; set; } =
            string.Empty;

        public DateTime PayPeriodStart { get; set; }

        public DateTime PayPeriodEnd { get; set; }

        public decimal BasicSalary { get; set; }

        public decimal Deductions { get; set; }
    }
}