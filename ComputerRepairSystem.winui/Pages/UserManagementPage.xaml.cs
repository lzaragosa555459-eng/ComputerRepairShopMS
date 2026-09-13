using ComputerRepairSystem.infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Contacts;
using ComputerRepairSystem.infrastructure.data;
using ComputerRepairSystem.domain.Entities;
namespace ComputerRepairSystem_winui.Pages;

public sealed partial class UserManagementPage : Page
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly MasterErpDbContext _masterDb;

    public UserManagementPage(
        UserManager<ApplicationUser> userManager,
        MasterErpDbContext masterDb)
    {
        InitializeComponent();

        _userManager = userManager;
        _masterDb = masterDb;

        Loaded += UserManagementPage_Loaded;
    }


    // ==========================================
    // PAGE LOADED
    // ==========================================

    private async void UserManagementPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await LoadUsersAsync();
    }


    // ==========================================
    // LOAD USERS
    // ==========================================

    private async Task LoadUsersAsync()
    {
        try
        {
            var users =
                await _userManager.Users
                    .AsNoTracking()
                    .ToListAsync();

            var companies =
                await _masterDb.Companies
                    .AsNoTracking()
                    .ToListAsync();

            var userRows =
                users
                    .Select(user =>
                    {
                        var company =
                            companies.FirstOrDefault(
                                c => c.CompanyId == user.CompanyId);

                        return new UserRow
                        {
                            User = user,
                            CompanyName =
                                company?.CompanyName
                                ?? "Unknown Company"
                        };
                    })
                    .ToList();

            UsersListView.ItemsSource = userRows;

            EditUserButton.IsEnabled =
                UsersListView.SelectedItem != null;

            DeleteUserButton.IsEnabled =
                UsersListView.SelectedItem != null;
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Error Loading Users",
                ex.Message);
        }
    }


    // ==========================================
    // SELECTION CHANGED
    // ==========================================

    private void UsersListView_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        bool hasSelection =
            UsersListView.SelectedItem != null;

        EditUserButton.IsEnabled = hasSelection;
        DeleteUserButton.IsEnabled = hasSelection;
    }


    // ==========================================
    // ADD USER
    // ==========================================

    private async void AddUserButton_Click(
        object sender,
        RoutedEventArgs e)
    {

        try
        {

            var usernameBox = new TextBox
            {
                Header = "Username",
                PlaceholderText = "Enter username"
            };

            var emailBox = new TextBox
            {
                Header = "Email",
                PlaceholderText = "Enter email"
            };

            var passwordBox = new PasswordBox
            {
                Header = "Password",
                PlaceholderText = "Enter password"
            };
            var roleBox = new ComboBox
            {
                Header = "Role",
                PlaceholderText = "Select a role"
            };

            roleBox.Items.Add("Admin");
            roleBox.Items.Add("Technician");
            roleBox.Items.Add("Receptionist");

            roleBox.SelectedIndex = 1;
            var companies =
                await _masterDb.Companies
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

            var companyBox = new ComboBox
            {
                Header = "Company",
                PlaceholderText = "Select a company",
                DisplayMemberPath = "CompanyName"
            };

            foreach (var company in companies)
            {
                companyBox.Items.Add(company);
            }

            if (companies.Count > 0)
            {
                companyBox.SelectedIndex = 0;
            }

            var activeCheckBox = new CheckBox
            {
                Content = "Active",
                IsChecked = true
            };

            var panel = new StackPanel
            {
                Spacing = 12
            };

            panel.Children.Add(usernameBox);
            panel.Children.Add(emailBox);
            panel.Children.Add(passwordBox);
            panel.Children.Add(roleBox);
            panel.Children.Add(companyBox);
            panel.Children.Add(activeCheckBox);


            var dialog = new ContentDialog
            {
                Title = "Add User",
                Content = panel,

                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel",

                DefaultButton =
                    ContentDialogButton.Primary,

                XamlRoot = XamlRoot
            };


            var result =
                await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
                return;


            // ==========================================
            // VALIDATION
            // ==========================================

            if (string.IsNullOrWhiteSpace(usernameBox.Text))
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Username is required.");

                return;
            }

            if (string.IsNullOrWhiteSpace(emailBox.Text))
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Email is required.");

                return;
            }

            if (string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Password is required.");

                return;
            }

            if (roleBox.SelectedItem == null)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Please select a role.");

                return;
            }

            if (companyBox.SelectedItem is not Company selectedCompany)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Please select a company.");

                return;
            }


            // ==========================================
            // CHECK DUPLICATE USERNAME
            // ==========================================

            var existingUsername =
                await _userManager.FindByNameAsync(
                    usernameBox.Text.Trim());

            if (existingUsername != null)
            {
                await ShowMessageAsync(
                    "User Already Exists",
                    "That username is already being used.");

                return;
            }


            // ==========================================
            // CHECK DUPLICATE EMAIL
            // ==========================================

            var existingEmail =
                await _userManager.FindByEmailAsync(
                    emailBox.Text.Trim());

            if (existingEmail != null)
            {
                await ShowMessageAsync(
                    "Email Already Exists",
                    "That email is already being used.");

                return;
            }


            // ==========================================
            // CREATE USER
            // ==========================================
            var selectedRole =
                roleBox.SelectedItem.ToString();
            var user = new ApplicationUser
            {
                UserName =
                    usernameBox.Text.Trim(),

                Email =
                    emailBox.Text.Trim(),

                CompanyId =
                    selectedCompany.CompanyId,

                IsActive =
                    activeCheckBox.IsChecked == true
            };


            var createResult =
                await _userManager.CreateAsync(
                    user,
                    passwordBox.Password);

            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    selectedRole);

            if (!roleResult.Succeeded)
            {
                await ShowIdentityErrorsAsync(
                    "Unable to Assign Role",
                    roleResult);

                await _userManager.DeleteAsync(user);

                return;
            }


            if (!createResult.Succeeded)
            {
                await ShowIdentityErrorsAsync(
                    "Unable to Add User",
                    createResult);

                return;
            }


            await LoadUsersAsync();

            await ShowMessageAsync(
                "User Added",
                $"User '{user.UserName}' was created successfully.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Add User Error",
                ex.ToString());

            System.Diagnostics.Debug.WriteLine(ex);
        }

    }


    // ==========================================
    // EDIT USER
    // ==========================================

    private async void EditUserButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        // ==========================================
        // CHECK SELECTION
        // ==========================================

        if (UsersListView.SelectedItem
            is not UserRow selectedRow)
        {
            await ShowMessageAsync(
                "No User Selected",
                "Please select a user first.");

            return;
        }

        var selectedUser = selectedRow.User;


        try
        {
            // ==========================================
            // GET FRESH TRACKED USER
            // ==========================================

            var user =
                await _userManager.FindByIdAsync(
                    selectedUser.Id);

            if (user == null)
            {
                await ShowMessageAsync(
                    "User Not Found",
                    "The selected user could not be found.");

                await LoadUsersAsync();

                return;
            }


            // ==========================================
            // INPUT FIELDS
            // ==========================================

            var usernameBox = new TextBox
            {
                Header = "Username",
                Text = user.UserName ?? string.Empty,
                PlaceholderText = "Enter username"
            };

            var emailBox = new TextBox
            {
                Header = "Email",
                Text = user.Email ?? string.Empty,
                PlaceholderText = "Enter email"
            };

            var companies =
                await _masterDb.Companies
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

            var companyBox = new ComboBox
            {
                Header = "Company",
                PlaceholderText = "Select a company",
                DisplayMemberPath = "CompanyName"
            };

            foreach (var company in companies)
            {
                companyBox.Items.Add(company);
            }

            var currentCompany =
                companies.FirstOrDefault(
                    c => c.CompanyId == user.CompanyId);

            if (currentCompany != null)
            {
                companyBox.SelectedItem = currentCompany;
            }

            var activeCheckBox = new CheckBox
            {
                Content = "Active",
                IsChecked = user.IsActive
            };


            // ==========================================
            // DIALOG CONTENT
            // ==========================================

            var panel = new StackPanel
            {
                Spacing = 12
            };

            panel.Children.Add(usernameBox);
            panel.Children.Add(emailBox);
            panel.Children.Add(companyBox);
            panel.Children.Add(activeCheckBox);


            var dialog = new ContentDialog
            {
                Title = "Edit User",
                Content = panel,

                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",

                DefaultButton =
                    ContentDialogButton.Primary,

                XamlRoot = XamlRoot
            };


            var dialogResult =
                await dialog.ShowAsync();

            if (dialogResult != ContentDialogResult.Primary)
                return;


            // ==========================================
            // VALIDATION
            // ==========================================

            var username =
                usernameBox.Text.Trim();

            var email =
                emailBox.Text.Trim();

            if (companyBox.SelectedItem is not Company selectedCompany)
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Please select a company.");

                return;
            }


            if (string.IsNullOrWhiteSpace(username))
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Username is required.");

                return;
            }


            if (string.IsNullOrWhiteSpace(email))
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Email is required.");

                return;
            }



            // ==========================================
            // CHECK DUPLICATE USERNAME
            // ==========================================

            var usernameExists =
                await _userManager.FindByNameAsync(
                    username);

            if (usernameExists != null &&
                usernameExists.Id != user.Id)
            {
                await ShowMessageAsync(
                    "Username Already Exists",
                    "That username is already being used.");

                return;
            }


            // ==========================================
            // CHECK DUPLICATE EMAIL
            // ==========================================

            var emailExists =
                await _userManager.FindByEmailAsync(
                    email);

            if (emailExists != null &&
                emailExists.Id != user.Id)
            {
                await ShowMessageAsync(
                    "Email Already Exists",
                    "That email is already being used.");

                return;
            }


            // ==========================================
            // UPDATE USERNAME
            // ==========================================

            if (!string.Equals(
                    user.UserName,
                    username,
                    StringComparison.OrdinalIgnoreCase))
            {
                var usernameResult =
                    await _userManager.SetUserNameAsync(
                        user,
                        username);

                if (!usernameResult.Succeeded)
                {
                    await ShowIdentityErrorsAsync(
                        "Unable to Update Username",
                        usernameResult);

                    return;
                }
            }


            // ==========================================
            // UPDATE EMAIL
            // ==========================================

            if (!string.Equals(
                    user.Email,
                    email,
                    StringComparison.OrdinalIgnoreCase))
            {
                var emailResult =
                    await _userManager.SetEmailAsync(
                        user,
                        email);

                if (!emailResult.Succeeded)
                {
                    await ShowIdentityErrorsAsync(
                        "Unable to Update Email",
                        emailResult);

                    return;
                }
            }


            // ==========================================
            // UPDATE CUSTOM FIELDS
            // ==========================================

            user.CompanyId =
                selectedCompany.CompanyId;

            user.IsActive =
                activeCheckBox.IsChecked == true;


            // ==========================================
            // SAVE CHANGES
            // ==========================================

            var updateResult =
                await _userManager.UpdateAsync(user);


            if (!updateResult.Succeeded)
            {
                await ShowIdentityErrorsAsync(
                    "Unable to Update User",
                    updateResult);

                return;
            }


            // ==========================================
            // REFRESH LIST
            // ==========================================

            await LoadUsersAsync();


            await ShowMessageAsync(
                "User Updated",
                $"User '{user.UserName}' was updated successfully.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Edit User Error",
                ex.ToString());
        }
    }

    // ==========================================
    // DELETE USER
    // ==========================================

    private async void DeleteUserButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (UsersListView.SelectedItem
            is not UserRow selectedRow)
        {
            await ShowMessageAsync(
                "No User Selected",
                "Please select a user first.");

            return;
        }

        var selectedUser = selectedRow.User;

        var username =
            selectedUser.UserName ?? "this user";

        var dialog = new ContentDialog
        {
            Title = "Delete User",

            Content =
                $"Are you sure you want to delete '{username}'?",

            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",

            DefaultButton =
                ContentDialogButton.Close,

            XamlRoot = XamlRoot
        };

        var result =
            await dialog.ShowAsync();

        if (result != ContentDialogResult.Primary)
            return;

        try
        {
            // Get a fresh tracked instance from Identity
            var user =
                await _userManager.FindByIdAsync(
                    selectedUser.Id);

            if (user == null)
            {
                await ShowMessageAsync(
                    "User Not Found",
                    "The selected user no longer exists.");

                await LoadUsersAsync();
                return;
            }

            var deleteResult =
                await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                await ShowIdentityErrorsAsync(
                    "Unable to Delete User",
                    deleteResult);

                return;
            }

            await LoadUsersAsync();

            await ShowMessageAsync(
                "User Deleted",
                $"User '{username}' was deleted successfully.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Delete User Error",
                ex.Message);
        }
    }


    // ==========================================
    // REFRESH
    // ==========================================

    private async void RefreshButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await LoadUsersAsync();
    }


    // ==========================================
    // SHOW MESSAGE
    // ==========================================

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


    // ==========================================
    // IDENTITY ERRORS
    // ==========================================

    private async Task ShowIdentityErrorsAsync(
        string title,
        IdentityResult result)
    {
        var errors =
            string.Join(
                "\n",
                result.Errors.Select(
                    error =>
                        $"• {error.Description}"));

        await ShowMessageAsync(
            title,
            errors);
    }
}