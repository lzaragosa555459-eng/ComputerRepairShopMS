using ComputerRepairSystem.company.Services;
using ComputerRepairSystem_winui.Pages;
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


    private void NavView_SelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
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

                default:
                    throw new InvalidOperationException(
                        $"Unknown navigation item tag: {item.Tag}");
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