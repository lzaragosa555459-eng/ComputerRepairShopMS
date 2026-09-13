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
            Visibility.Visible;

        ServiceManagementItem.Visibility =
            Visibility.Collapsed;

        InventoryItem.Visibility =
            Visibility.Visible;

        RepairManagementItem.Visibility =
            Visibility.Collapsed;

        BillingItem.Visibility =
            Visibility.Collapsed;

        UserManagementItem.Visibility =
            Visibility.Collapsed;

        SystemSettingsItem.Visibility =
            Visibility.Collapsed;

        var role = CurrentUser.Role;

        if (role == "Admin")
        {
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
        }
        else if (role == "Technician")
        {
            RepairManagementItem.Visibility =
                Visibility.Visible;
        }
        else if (role == "Receptionist")
        {
            ServiceManagementItem.Visibility =
                Visibility.Visible;

            BillingItem.Visibility =
                Visibility.Visible;
        }

        HomeItem.IsSelected = true;

        NavFrame.Content =
            App.Services
                .GetRequiredService<HomePage>();
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

                case "users":
                    NavigateToUserManagementPage();
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
}