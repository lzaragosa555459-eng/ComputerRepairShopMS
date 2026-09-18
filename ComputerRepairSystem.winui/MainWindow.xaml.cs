using ComputerRepairSystem.company.Services;
using ComputerRepairSystem_winui.Pages;
using ComputerRepairSystem_winui.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ComputerRepairSystem.infrastructure.data;
namespace ComputerRepairSystem_winui;

public sealed partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();

        ShowLogin();


        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption =
            TitleBarHeightOption.Tall;

        AppWindow.SetIcon("Assets/AppIcon.ico");
    }

    private void TitleBar_PaneToggleRequested(
        object sender,
        RoutedEventArgs e)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }


    public void ShowApplication()
    {
        UpdateUserHeader();

        HomeItem.Visibility =
            Visibility.Collapsed;

        ServiceManagementItem.Visibility =
            Visibility.Collapsed;

        InventoryItem.Visibility =
            Visibility.Collapsed;

        RepairManagementItem.Visibility =
            Visibility.Collapsed;

        BillingItem.Visibility =
            Visibility.Collapsed;

        UserManagementItem.Visibility =
            Visibility.Collapsed;

        CompanyManagementItem.Visibility =
            Visibility.Collapsed;

        SystemSettingsItem.Visibility =
            Visibility.Collapsed;

        CustomerManagementItem.Visibility =
            Visibility.Collapsed;

        EmployeeManagementItem.Visibility =
            Visibility.Collapsed;

        AttendanceManagementItem.Visibility =
            Visibility.Collapsed;

        PayrollManagementItem.Visibility =
            Visibility.Collapsed;

        FinanceManagementItem.Visibility =
            Visibility.Collapsed;

        var role = CurrentUser.Role;

        if (role == "Super Admin")
        {

            UserManagementItem.Visibility =
                Visibility.Visible;

            CompanyManagementItem.Visibility =
                Visibility.Visible;

            CompanyManagementItem.IsSelected = true;

            NavFrame.Content =
                App.Services
                    .GetRequiredService<CompanyManagementPage>();

            return;
        }

        if (role == "Admin")
        {
            HomeItem.Visibility = 
                Visibility.Visible;

            ServiceManagementItem.Visibility =
                Visibility.Visible;

            RepairManagementItem.Visibility =
                Visibility.Visible;

            BillingItem.Visibility =
                Visibility.Visible;

            UserManagementItem.Visibility =
                Visibility.Visible;

            InventoryItem.Visibility =
                Visibility.Visible;

            SystemSettingsItem.Visibility =
                Visibility.Visible;
            CustomerManagementItem.Visibility =
                Visibility.Visible;

            EmployeeManagementItem.Visibility =
                Visibility.Visible;

            AttendanceManagementItem.Visibility =
                Visibility.Visible;

            PayrollManagementItem.Visibility =
                Visibility.Visible;

            FinanceManagementItem.Visibility =
                Visibility.Visible;

            HomeItem.IsSelected = true;

            NavFrame.Content = 
		App.Services
		   .GetRequiredService<HomePage>();

	    return;
 
        }
        else if (role == "Technician")
        {
            RepairManagementItem.Visibility =
                Visibility.Visible;

	        InventoryItem.Visibility = 
		        Visibility.Visible;

            RepairManagementItem.IsSelected = true;

            NavFrame.Content =
            App.Services
               .GetRequiredService<RepairManagementPage>();
        }
        else if (role == "Receptionist")
        {

            ServiceManagementItem.Visibility =
                Visibility.Visible;

            BillingItem.Visibility =
                Visibility.Visible;

            CustomerManagementItem.Visibility =
                Visibility.Visible;

            ServiceManagementItem.IsSelected = true;

            NavFrame.Content =
            App.Services
               .GetRequiredService<ServiceManagementPage>();

        }
    }
    public void ShowLogin()
    {
        HomeItem.Visibility =
            Visibility.Collapsed;

        ServiceManagementItem.Visibility =
            Visibility.Collapsed;

        RepairManagementItem.Visibility =
            Visibility.Collapsed;

        BillingItem.Visibility =
            Visibility.Collapsed;

        UserManagementItem.Visibility =
            Visibility.Collapsed;

        InventoryItem.Visibility =
            Visibility.Collapsed;

        SystemSettingsItem.Visibility =
            Visibility.Collapsed;

        CustomerManagementItem.Visibility =
            Visibility.Collapsed;

        CompanyManagementItem.Visibility =
            Visibility.Collapsed;

        EmployeeManagementItem.Visibility =
            Visibility.Collapsed;

        AttendanceManagementItem.Visibility =
            Visibility.Collapsed;

        PayrollManagementItem.Visibility =
            Visibility.Collapsed;

        FinanceManagementItem.Visibility =
            Visibility.Collapsed;

        HomeItem.IsSelected = false;

        NavFrame.Content =
            App.Services
                .GetRequiredService<LoginPage>();
    }
    private void NavView_SelectionChanged(
       NavigationView sender,
       NavigationViewSelectionChangedEventArgs args)
    {
        if (!CurrentUser.IsLoggedIn)
        {
            return;
        }
        else if (args.SelectedItem is NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "home":
                    NavigateToHomePage();
                    break;

                case "customers":
                    NavigateToServiceManagementPage();
                    break;

                case "repairs":
                    NavigateToRepairManagementPage();
                    break;

                case "billing":
                    NavigateToBillingPage();
                    break;

                case "customer-management":
                    NavigateToCustomerManagementPage();
                    break;

                case "users":
                    NavigateToUserManagementPage();
                    break;
                case "companies":
                    NavigateToCompanyManagementPage();
                    break;
                case "employee-management":
                    NavigateToEmployeeManagement();
                    break;
                case "attendance-management":
                    NavigateToAttendanceManagementPage();
                    break;
                    
                case "payroll-management":
                    NavigateToPayrollManagementPage();
                    break;

                case "finance-management":
                    NavigateToFinanceManagementPage();
                    break;

                case "inventory":
                    NavigateToInventoryPage();
                    break;

                case "settings":
                    NavigateToSettingsPage();
                    break;
            }
        }
    }
    private void NavigateToServiceManagementPage()
    {
        var page =
            App.Services.GetRequiredService<ServiceManagementPage>();

        NavFrame.Content = page;
    }

    private void NavigateToUserManagementPage()
    {
        var page =
            App.Services.GetRequiredService<UserManagementPage>();

        NavFrame.Content = page;
    }
    private void NavigateToRepairManagementPage()
    {
        var page =
            App.Services.GetRequiredService<RepairManagementPage>();

        NavFrame.Content = page;
    }   
    private void NavigateToBillingPage()
    {
        var page =
            App.Services.GetRequiredService<BillingPage>();

        NavFrame.Content = page;
    }

    private void NavigateToHomePage()
    {
        var page =
            App.Services.GetRequiredService<HomePage>();

        NavFrame.Content = page;
    }

    private void NavigateToInventoryPage()
    {
        var page =
            App.Services.GetRequiredService<InventoryManagementPage>();

        NavFrame.Content = page;
    }
    private void UpdateUserHeader()
    {
        UserNameText.Text = CurrentUser.UserName;
    }
    private async void GlobalSearchButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = "Search",
            Content = new TextBox
            {
                PlaceholderText = "Search customers, repairs, devices..."
            },
            CloseButtonText = "Close",
            XamlRoot = Content.XamlRoot
        };

        await dialog.ShowAsync();
    }
    private async void NotificationButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = "Notifications",
            Content = new TextBlock
            {
                Text = "No new notifications.",
                TextWrapping = TextWrapping.Wrap
            },
            CloseButtonText = "Close",
            XamlRoot = Content.XamlRoot
        };

        await dialog.ShowAsync();
    }
    private async void ProfileButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = CurrentUser.UserName,
            Content = new StackPanel
            {
                Spacing = 8,
                Children =
            {
                new TextBlock
                {
                    Text = $"Username: {CurrentUser.UserName}"
                },
                new TextBlock
                {
                    Text = $"Role: {CurrentUser.Role}"
                }
            }
            },
            PrimaryButtonText = "Logout",
            CloseButtonText = "Close",
            XamlRoot = Content.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            CurrentUser.Logout();
            ShowLogin();
        }
    }
    private void NavigateToSettingsPage()
    {
        var page =
            App.Services.GetRequiredService<SettingsPage>();

        NavFrame.Content = page;
    }
    private void NavigateToCustomerManagementPage()
    {
        var page =
            App.Services.GetRequiredService<CustomerManagementPage>();

        NavFrame.Content = page;
    }
    private void NavigateToCompanyManagementPage()
    {
        var page =
            App.Services
                .GetRequiredService<CompanyManagementPage>();

        NavFrame.Content = page;
    }
    private void NavigateToEmployeeManagement()
    {
        var page =
            App.Services
                .GetRequiredService<EmployeeManagementPage>();

        NavFrame.Content = page;
    }

    private void NavigateToAttendanceManagementPage()
    {
        var page =
            App.Services
                .GetRequiredService<AttendanceManagementPage>();

        NavFrame.Content = page;
    }

    private void NavigateToPayrollManagementPage()
    {
        var page =
            App.Services
                .GetRequiredService<PayrollManagementPage>();

        NavFrame.Content = page;
    }

    private void NavigateToFinanceManagementPage()
    {
        var page =
            App.Services
                .GetRequiredService<FinanceManagementPage>();

        NavFrame.Content = page;
    }
}
