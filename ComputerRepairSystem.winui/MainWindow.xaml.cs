using ComputerRepairSystem.company.Services;
using ComputerRepairSystem_winui.Pages;
using ComputerRepairSystem_winui.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        NavFrame.GoBack();
    }

    public void ShowApplication()
    {
        HomeItem.Visibility =
            Visibility.Visible;

        ServiceManagementItem.Visibility =
            Visibility.Collapsed;

        AboutItem.Visibility =
            Visibility.Visible;

        RepairManagementItem.Visibility =
            Visibility.Collapsed;

        BillingItem.Visibility =
            Visibility.Collapsed;

        UserManagementItem.Visibility =
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

        AboutItem.Visibility =
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

        if (args.IsSettingsSelected)
        {
            NavFrame.Navigate(typeof(SettingsPage));
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

                case "about":
                    NavFrame.Navigate(typeof(AboutPage));
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
}