using ComputerRepairSystem.company.Entities;
using ComputerRepairSystem.company.Services;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ComputerRepairSystem_winui.Pages;

public sealed partial class CustomerPage : Page
{
    private readonly CustomerService _customerService;

    private Customer? _selectedCustomer;

    public CustomerPage(CustomerService customerService)
    {
        InitializeComponent();

        _customerService = customerService;

        Loaded += CustomerPage_Loaded;
    }

    private async void CustomerPage_Loaded(
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

            CustomerList.ItemsSource = customers;
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

    private void CustomerList_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (CustomerList.SelectedItem is not Customer customer)
        {
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

        FirstNameBox.Text = string.Empty;
        MiddleNameBox.Text = string.Empty;
        LastNameBox.Text = string.Empty;
        PhoneBox.Text = string.Empty;
        EmailBox.Text = string.Empty;
        AddressBox.Text = string.Empty;

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
}