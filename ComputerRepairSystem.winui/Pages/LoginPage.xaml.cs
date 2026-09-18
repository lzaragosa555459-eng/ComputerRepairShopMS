using ComputerRepairSystem.infrastructure.Entities;
using ComputerRepairSystem_winui.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ComputerRepairSystem_winui.Pages;

public sealed partial class LoginPage : Page
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    public LoginPage(
        UserManager<ApplicationUser> userManager)
    {
        InitializeComponent();

        _userManager = userManager;
    }


    private async void LoginButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ErrorText.Visibility =
            Visibility.Collapsed;

        var username =
            UsernameTextBox.Text.Trim();

        var password =
            PasswordBox.Password;


        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            ShowError(
                "Username and password are required.");

            return;
        }


        LoginButton.IsEnabled = false;


        try
        {
            var user =
                await _userManager.FindByNameAsync(
                    username);


            if (user == null)
            {
                ShowError(
                    "Invalid username or password.");

                return;
            }


            var passwordValid =
                await _userManager.CheckPasswordAsync(
                    user,
                    password);


            if (!passwordValid)
            {
                ShowError(
                    $"Password check failed. PasswordHash is: " +
                    $"{user.PasswordHash ?? "NULL"}");

                return;
            }


            var roles =
                await _userManager.GetRolesAsync(
                    user);


            if (roles.Count == 0)
            {
                ShowError(
                    "Your account has no assigned role.");

                return;
            }


            var role =
                roles[0];


            CurrentUser.Login(
                user.Id,
                user.UserName ?? string.Empty,
                role,
                user.CompanyId);


            var mainWindow =
                App.Services
                    .GetRequiredService<MainWindow>();

            mainWindow.ShowApplication();


        }
        catch (Exception ex)
        {
            ShowError(
                ex.Message);
        }
        finally
        {
            LoginButton.IsEnabled = true;
        }
    }


    private void ShowError(
        string message)
    {
        ErrorText.Text = message;

        ErrorText.Visibility =
            Visibility.Visible;
    }
}