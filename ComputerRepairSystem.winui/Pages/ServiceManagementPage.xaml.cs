using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;
using ComputerRepairSystem.company.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ComputerRepairSystem_winui.Pages;

public sealed partial class ServiceManagementPage : Page
{
    private readonly CustomerService _customerService;
    private readonly IDbContextFactory<TenantDbContext> _tenantDbFactory;

    private Customer? _selectedCustomer;
    private List<Customer> _customers = new();
    public ServiceManagementPage(
        CustomerService customerService,
        IDbContextFactory<TenantDbContext> tenantDbFactory)
    {
        InitializeComponent();

        _customerService = customerService;
        _tenantDbFactory = tenantDbFactory;

        Loaded += ServiceManagementPage_Loaded;
    }

    private async void ServiceManagementPage_Loaded (
        object sender,
        RoutedEventArgs e)
    {
        await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        try
        {
            var customers =
                await _customerService.GetAllAsync();

            _customers = customers.ToList();

            CustomerList.ItemsSource = _customers;
        }
        catch (Exception ex)
        {
            ShowStatus(
                "Failed to load customers: " + ex.Message,
                InfoBarSeverity.Error);
        }
    }

    private async void AddButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!ValidateInput())
            return;

        try
        {
            var customer = new Customer
            {
                FirstName = FirstNameBox.Text.Trim(),

                MiddleName = string.IsNullOrWhiteSpace(
                    MiddleNameBox.Text)
                    ? null
                    : MiddleNameBox.Text.Trim(),

                LastName = LastNameBox.Text.Trim(),

                Phone = string.IsNullOrWhiteSpace(
                    PhoneBox.Text)
                    ? null
                    : PhoneBox.Text.Trim(),

                Email = string.IsNullOrWhiteSpace(
                    EmailBox.Text)
                    ? null
                    : EmailBox.Text.Trim(),

                Address = string.IsNullOrWhiteSpace(
                    AddressBox.Text)
                    ? null
                    : AddressBox.Text.Trim()
            };

            await _customerService.AddAsync(customer);

            ClearForm();

            await LoadCustomersAsync();

            ShowStatus(
                "Customer added successfully.",
                InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowStatus(
                "Failed to add customer: " + ex.Message,
                InfoBarSeverity.Error);
        }
    }

    private async void UpdateButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_selectedCustomer == null)
        {
            ShowStatus(
                "Please select a customer first.",
                InfoBarSeverity.Warning);

            return;
        }

        if (!ValidateInput())
            return;

        try
        {
            _selectedCustomer.FirstName =
                FirstNameBox.Text.Trim();

            _selectedCustomer.MiddleName =
                string.IsNullOrWhiteSpace(MiddleNameBox.Text)
                    ? null
                    : MiddleNameBox.Text.Trim();

            _selectedCustomer.LastName =
                LastNameBox.Text.Trim();

            _selectedCustomer.Phone =
                string.IsNullOrWhiteSpace(PhoneBox.Text)
                    ? null
                    : PhoneBox.Text.Trim();

            _selectedCustomer.Email =
                string.IsNullOrWhiteSpace(EmailBox.Text)
                    ? null
                    : EmailBox.Text.Trim();

            _selectedCustomer.Address =
                string.IsNullOrWhiteSpace(AddressBox.Text)
                    ? null
                    : AddressBox.Text.Trim();

            await _customerService.UpdateAsync(
                _selectedCustomer);

            ClearForm();

            await LoadCustomersAsync();

            ShowStatus(
                "Customer updated successfully.",
                InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowStatus(
                "Failed to update customer: " + ex.Message,
                InfoBarSeverity.Error);
        }
    }

    private async void DeleteButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_selectedCustomer == null)
        {
            ShowStatus(
                "Please select a customer first.",
                InfoBarSeverity.Warning);

            return;
        }

        try
        {
            await _customerService.DeleteAsync(
                _selectedCustomer.CustomerId);

            ClearForm();

            await LoadCustomersAsync();

            ShowStatus(
                "Customer deleted successfully.",
                InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowStatus(
                "Failed to delete customer: " + ex.Message,
                InfoBarSeverity.Error);
        }
    }

    private void ClearButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ClearForm();
    }

    private async void CustomerList_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {

        if (CustomerList.SelectedItem is not Customer customer)
        {
            DeviceList.ItemsSource = null;
            return;
        }

        _selectedCustomer = customer;

        FirstNameBox.Text =
            customer.FirstName;

        MiddleNameBox.Text =
            customer.MiddleName ?? string.Empty;

        LastNameBox.Text =
            customer.LastName;

        PhoneBox.Text =
            customer.Phone ?? string.Empty;

        EmailBox.Text =
            customer.Email ?? string.Empty;

        AddressBox.Text =
            customer.Address ?? string.Empty;

        ServiceRequestList.ItemsSource = null;

        await LoadCustomerDevicesAsync(
            customer.CustomerId);

    }


    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(
                FirstNameBox.Text))
        {
            ShowStatus(
                "First Name is required.",
                InfoBarSeverity.Warning);

            return false;
        }

        if (string.IsNullOrWhiteSpace(
                LastNameBox.Text))
        {
            ShowStatus(
                "Last Name is required.",
                InfoBarSeverity.Warning);

            return false;
        }

        return true;
    }
    private void ClearForm()
    {
        _selectedCustomer = null;

        // Customer
        FirstNameBox.Text = string.Empty;
        MiddleNameBox.Text = string.Empty;
        LastNameBox.Text = string.Empty;
        PhoneBox.Text = string.Empty;
        EmailBox.Text = string.Empty;
        AddressBox.Text = string.Empty;

        // Device
        DeviceTypeBox.Text = string.Empty;
        BrandBox.Text = string.Empty;
        ModelBox.Text = string.Empty;
        SerialNumberBox.Text = string.Empty;
        DeviceConditionBox.Text = string.Empty;

        // Service Request
        DescriptionBox.Text = string.Empty;

        PriorityBox.SelectedIndex = 1;
        StatusBox.SelectedIndex = 0;

        CustomerList.SelectedItem = null;
    }

    private void ShowStatus(
        string message,
        InfoBarSeverity severity)
    {
        StatusBar.Message = message;
        StatusBar.Severity = severity;
        StatusBar.IsOpen = true;
    }

    //Customer info
    private async void SubmitRequestButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        // ==========================================
        // VALIDATE CUSTOMER
        // ==========================================

        if (!ValidateCustomerInput())
            return;


        // ==========================================
        // VALIDATE DEVICE
        // ==========================================

        if (!ValidateDeviceInput())
            return;


        // ==========================================
        // VALIDATE SERVICE REQUEST
        // ==========================================

        if (string.IsNullOrWhiteSpace(
                DescriptionBox.Text))
        {
            ShowStatus(
                "Service request description is required.",
                InfoBarSeverity.Warning);

            return;
        }


        try
        {
            int customerId;


            // ==========================================
            // 1. CREATE OR REUSE CUSTOMER
            // ==========================================

            if (_selectedCustomer == null)
            {
                // NEW CUSTOMER

                var customer = new Customer
                {
                    FirstName =
                        FirstNameBox.Text.Trim(),

                    MiddleName =
                        string.IsNullOrWhiteSpace(
                            MiddleNameBox.Text)
                            ? null
                            : MiddleNameBox.Text.Trim(),

                    LastName =
                        LastNameBox.Text.Trim(),

                    Phone =
                        string.IsNullOrWhiteSpace(
                            PhoneBox.Text)
                            ? null
                            : PhoneBox.Text.Trim(),

                    Email =
                        string.IsNullOrWhiteSpace(
                            EmailBox.Text)
                            ? null
                            : EmailBox.Text.Trim(),

                    Address =
                        string.IsNullOrWhiteSpace(
                            AddressBox.Text)
                            ? null
                            : AddressBox.Text.Trim()
                };


                await _customerService.AddAsync(
                    customer);


                customerId =
                    customer.CustomerId;
            }
            else
            {
                // EXISTING CUSTOMER

                customerId =
                    _selectedCustomer.CustomerId;
            }


            // ==========================================
            // 2. CREATE DEVICE
            // ==========================================

            await using var context =
                await _tenantDbFactory.CreateDbContextAsync();


            var device = new Device
            {
                CustomerId =
                    customerId,

                DeviceType =
                    DeviceTypeBox.Text.Trim(),

                Brand =
                    BrandBox.Text.Trim(),

                Model =
                    ModelBox.Text.Trim(),

                SerialNumber =
                    string.IsNullOrWhiteSpace(
                        SerialNumberBox.Text)
                        ? null
                        : SerialNumberBox.Text.Trim(),

                DeviceCondition =
                    string.IsNullOrWhiteSpace(
                        DeviceConditionBox.Text)
                        ? null
                        : DeviceConditionBox.Text.Trim()
            };


            context.Devices.Add(device);

            await context.SaveChangesAsync();


            // ==========================================
            // 3. CREATE SERVICE REQUEST
            // ==========================================

            var priority =
                (PriorityBox.SelectedItem
                    as ComboBoxItem)?
                    .Content?
                    .ToString()
                ?? "Medium";


            var status =
                (StatusBox.SelectedItem
                    as ComboBoxItem)?
                    .Content?
                    .ToString()
                ?? "Pending";


            var serviceRequest =
                new ServiceRequest
                {
                    DeviceId =
                        device.DeviceId,

                    Description =
                        DescriptionBox.Text.Trim(),

                    Priority =
                        priority,

                    Status =
                        status,

                    RequestDate =
                        DateTime.UtcNow
                };


            context.ServiceRequests.Add(
                serviceRequest);

            await context.SaveChangesAsync();


            // ==========================================
            // SUCCESS
            // ==========================================

            ClearForm();

            await LoadCustomersAsync();

            ShowStatus(
                "Service request created successfully.",
                InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowStatus(
                "Failed to create service request: "
                + ex.Message,
                InfoBarSeverity.Error);
        }
    }
    private bool ValidateCustomerInput()
    {
        if (string.IsNullOrWhiteSpace(
                FirstNameBox.Text))
        {
            ShowStatus(
                "First Name is required.",
                InfoBarSeverity.Warning);

            return false;
        }

        if (string.IsNullOrWhiteSpace(
                LastNameBox.Text))
        {
            ShowStatus(
                "Last Name is required.",
                InfoBarSeverity.Warning);

            return false;
        }

        return true;
    }
    private bool ValidateDeviceInput()
    {
        if (string.IsNullOrWhiteSpace(
                DeviceTypeBox.Text))
        {
            ShowStatus(
                "Device Type is required.",
                InfoBarSeverity.Warning);

            return false;
        }

        if (string.IsNullOrWhiteSpace(
                BrandBox.Text))
        {
            ShowStatus(
                "Brand is required.",
                InfoBarSeverity.Warning);

            return false;
        }

        if (string.IsNullOrWhiteSpace(
                ModelBox.Text))
        {
            ShowStatus(
                "Model is required.",
                InfoBarSeverity.Warning);

            return false;
        }

        return true;
    }
    private async Task LoadCustomerDevicesAsync(
        int customerId)
    {
        try
        {
            await using var db =
                await _tenantDbFactory.CreateDbContextAsync();

            var devices =
                await db.Devices
                    .AsNoTracking()
                    .Where(d =>
                        d.CustomerId == customerId)
                    .OrderBy(d => d.DeviceId)
                    .ToListAsync();

            DeviceList.ItemsSource = devices;
        }
        catch (Exception ex)
        {
            DeviceList.ItemsSource = null;

            ShowStatus(
                "Failed to load devices: " + ex.Message,
                InfoBarSeverity.Error);
        }
    }
    private async void DeviceList_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (DeviceList.SelectedItem is not Device device)
        {
            ServiceRequestList.ItemsSource = null;
            return;
        }

        await LoadServiceRequestsAsync(
            device.DeviceId);
    }
    private async Task LoadServiceRequestsAsync(
        int deviceId)
    {
        try
        {
            await using var db =
                await _tenantDbFactory.CreateDbContextAsync();

            var requests =
                await db.ServiceRequests
                    .AsNoTracking()
                    .Where(r =>
                        r.DeviceId == deviceId)
                    .OrderByDescending(
                        r => r.RequestDate)
                    .ToListAsync();

            ServiceRequestList.ItemsSource = requests;
        }
        catch (Exception ex)
        {
            ServiceRequestList.ItemsSource = null;

            ShowStatus(
                "Failed to load service requests: "
                + ex.Message,
                InfoBarSeverity.Error);
        }
    }
    private void CustomerSearchBox_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        var search =
            CustomerSearchBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(search))
        {
            CustomerList.ItemsSource =
                _customers;

            return;
        }

        var results =
            _customers
                .Where(c =>
                    c.FirstName.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    c.LastName.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    (
                        c.FirstName + " " + c.LastName
                    ).Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

        CustomerList.ItemsSource =
            results;
    }
    private void NewCustomerButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        _selectedCustomer = null;

        CustomerList.SelectedItem = null;

        CustomerSearchBox.Text =
            string.Empty;

        FirstNameBox.Text =
            string.Empty;

        MiddleNameBox.Text =
            string.Empty;

        LastNameBox.Text =
            string.Empty;

        PhoneBox.Text =
            string.Empty;

        EmailBox.Text =
            string.Empty;

        AddressBox.Text =
            string.Empty;

        DeviceList.ItemsSource =
            null;

        ServiceRequestList.ItemsSource =
            null;

        ShowStatus(
            "Ready to register a new customer.",
            InfoBarSeverity.Informational);
    }
}