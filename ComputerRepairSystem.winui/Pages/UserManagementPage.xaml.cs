using ComputerRepairSystem.infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem_winui.Pages;

public sealed partial class UserManagementPage : Page
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    public UserManagementPage(
        UserManager<ApplicationUser> userManager)
    {
        InitializeComponent();

        _userManager = userManager;

        Loaded += UserManagementPage_Loaded;
    }


    private async void UserManagementPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            var dialog = new ContentDialog
            {
                Title = "Error Loading Users",
                Content = ex.ToString(),
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }


    private async Task LoadUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        UsersListView.ItemsSource = users;
    }


    private async void RefreshButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await LoadUsersAsync();
    }


    private void AddUserButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        // We will implement Add User next
    }
}