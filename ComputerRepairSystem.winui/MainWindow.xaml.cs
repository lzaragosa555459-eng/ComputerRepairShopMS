using ComputerRepairSystem.company.Services;
using ComputerRepairSystem_winui.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ComputerRepairSystem_winui;

public sealed partial class MainWindow : Window
{

    private readonly CustomerService _customerService;

    public MainWindow(CustomerService customerService)
    {
        InitializeComponent();

        _customerService = customerService;

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
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
                    NavFrame.Navigate(typeof(HomePage));
                    break;

                case "customers":
                    NavigateToCustomerPage();
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
    private void NavigateToCustomerPage()
    {
        var page = new CustomerPage(
            App.Services.GetRequiredService<CustomerService>());

        NavFrame.Content = page;
    }
}