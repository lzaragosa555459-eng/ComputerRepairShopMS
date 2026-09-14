using ComputerRepairSystem.company.Entities;
using ComputerRepairSystem.company.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ComputerRepairSystem_winui.Pages;

public sealed partial class CustomerManagementPage : Page
{
    private readonly CustomerService _customerService;

    private List<Customer> _customers = new();
    private Customer? _selectedCustomer;

    public CustomerManagementPage(
        CustomerService customerService)
    {
        InitializeComponent();

        _customerService = customerService;

        Loaded += CustomerManagementPage_Loaded;
    }

    private async void CustomerManagementPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        try
        {
            _customers =
                await _customerService.GetAllAsync();

            CustomerList.ItemsSource =
                _customers;
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Error Loading Customers",
                ex.Message);
        }
    }

    private void CustomerList_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        _selectedCustomer =
            CustomerList.SelectedItem as Customer;
    }

    private async void NewCustomerButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var firstNameBox = new TextBox
        {
            Header = "First Name"
        };

        var lastNameBox = new TextBox
        {
            Header = "Last Name"
        };

        var phoneBox = new TextBox
        {
            Header = "Phone"
        };

        var emailBox = new TextBox
        {
            Header = "Email"
        };

        var addressBox = new TextBox
        {
            Header = "Address"
        };

        var content = new StackPanel
        {
            Spacing = 10
        };

        content.Children.Add(firstNameBox);
        content.Children.Add(lastNameBox);
        content.Children.Add(phoneBox);
        content.Children.Add(emailBox);
        content.Children.Add(addressBox);

        var dialog = new ContentDialog
        {
            Title = "Add Customer",
            Content = content,
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            XamlRoot = XamlRoot
        };

        var result =
            await dialog.ShowAsync();

        if (result != ContentDialogResult.Primary)
            return;

        if (string.IsNullOrWhiteSpace(
                firstNameBox.Text) ||
            string.IsNullOrWhiteSpace(
                lastNameBox.Text))
        {
            await ShowMessageAsync(
                "Validation",
                "First Name and Last Name are required.");

            return;
        }

        var customer = new Customer
        {
            FirstName = firstNameBox.Text.Trim(),
            LastName = lastNameBox.Text.Trim(),
            Phone = string.IsNullOrWhiteSpace(phoneBox.Text)
                ? null
                : phoneBox.Text.Trim(),
            Email = string.IsNullOrWhiteSpace(emailBox.Text)
                ? null
                : emailBox.Text.Trim(),
            Address = string.IsNullOrWhiteSpace(addressBox.Text)
                ? null
                : addressBox.Text.Trim()
        };

        try
        {
            await _customerService.AddAsync(customer);

            await LoadCustomersAsync();

            await ShowMessageAsync(
                "Success",
                "Customer added successfully.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Error",
                ex.Message);
        }
    }

    private async void EditCustomerButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_selectedCustomer == null)
        {
            await ShowMessageAsync(
                "Edit Customer",
                "Please select a customer first.");

            return;
        }

        var firstNameBox = new TextBox
        {
            Header = "First Name",
            Text = _selectedCustomer.FirstName
        };

        var lastNameBox = new TextBox
        {
            Header = "Last Name",
            Text = _selectedCustomer.LastName
        };

        var phoneBox = new TextBox
        {
            Header = "Phone",
            Text = _selectedCustomer.Phone ?? string.Empty
        };

        var emailBox = new TextBox
        {
            Header = "Email",
            Text = _selectedCustomer.Email ?? string.Empty
        };

        var addressBox = new TextBox
        {
            Header = "Address",
            Text = _selectedCustomer.Address ?? string.Empty
        };

        var content = new StackPanel
        {
            Spacing = 10
        };

        content.Children.Add(firstNameBox);
        content.Children.Add(lastNameBox);
        content.Children.Add(phoneBox);
        content.Children.Add(emailBox);
        content.Children.Add(addressBox);

        var dialog = new ContentDialog
        {
            Title = "Edit Customer",
            Content = content,
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            XamlRoot = XamlRoot
        };

        var result =
            await dialog.ShowAsync();

        if (result != ContentDialogResult.Primary)
            return;

        if (string.IsNullOrWhiteSpace(
                firstNameBox.Text) ||
            string.IsNullOrWhiteSpace(
                lastNameBox.Text))
        {
            await ShowMessageAsync(
                "Validation",
                "First Name and Last Name are required.");

            return;
        }

        _selectedCustomer.FirstName =
            firstNameBox.Text.Trim();

        _selectedCustomer.LastName =
            lastNameBox.Text.Trim();

        _selectedCustomer.Phone =
            string.IsNullOrWhiteSpace(phoneBox.Text)
                ? null
                : phoneBox.Text.Trim();

        _selectedCustomer.Email =
            string.IsNullOrWhiteSpace(emailBox.Text)
                ? null
                : emailBox.Text.Trim();

        _selectedCustomer.Address =
            string.IsNullOrWhiteSpace(addressBox.Text)
                ? null
                : addressBox.Text.Trim();

        try
        {
            await _customerService.UpdateAsync(
                _selectedCustomer);

            await LoadCustomersAsync();

            await ShowMessageAsync(
                "Success",
                "Customer updated successfully.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Error",
                ex.Message);
        }
    }

    private async void DeleteCustomerButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_selectedCustomer == null)
        {
            await ShowMessageAsync(
                "Delete Customer",
                "Please select a customer first.");

            return;
        }

        var dialog = new ContentDialog
        {
            Title = "Delete Customer",
            Content =
                $"Delete {_selectedCustomer.FirstName} " +
                $"{_selectedCustomer.LastName}?",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            XamlRoot = XamlRoot
        };

        var result =
            await dialog.ShowAsync();

        if (result != ContentDialogResult.Primary)
            return;

        try
        {
            await _customerService.DeleteAsync(
                _selectedCustomer.CustomerId);

            _selectedCustomer = null;

            await LoadCustomersAsync();
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Error",
                ex.Message);
        }
    }

    private void SearchBox_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        var search =
            SearchBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(search))
        {
            CustomerList.ItemsSource =
                _customers;

            return;
        }

        CustomerList.ItemsSource =
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
                    (c.Phone?.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)
                        ?? false))
                .ToList();
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
}