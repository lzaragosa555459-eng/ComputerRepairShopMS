using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;
using ComputerRepairSystem.domain.Entities;
using ComputerRepairSystem.infrastructure.data;
using ComputerRepairSystem.infrastructure.Entities;
using ComputerRepairSystem_winui.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Contacts;
namespace ComputerRepairSystem_winui.Pages;

public sealed partial class UserManagementPage : Page
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly MasterErpDbContext _masterDb;
    private readonly TenantDbContextFactory _tenantDbFactory;

    public UserManagementPage(
        UserManager<ApplicationUser> userManager,
        MasterErpDbContext masterDb,
        TenantDbContextFactory tenantDbFactory)
    {
        InitializeComponent();

        _userManager = userManager;
        _masterDb = masterDb;
        _tenantDbFactory = tenantDbFactory;

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
            var usersQuery =
                _userManager.Users
                    .AsNoTracking();

            // ==========================================
            // USER VISIBILITY BY ROLE
            // ==========================================

            if (CurrentUser.Role == "Super Admin")
            {
                // Super Admin can see all users.
            }
            else if (CurrentUser.Role == "Admin")
            {
                // Company Admin can only see users
                // belonging to their own company.
                usersQuery =
                    usersQuery.Where(
                        u => u.CompanyId == CurrentUser.CompanyId);
            }
            else
            {
                // Other roles should not see users.
                usersQuery =
                    usersQuery.Where(u => false);
            }

            var users =
                await usersQuery.ToListAsync();


            // ==========================================
            // LOAD COMPANIES
            // ==========================================

            var companies =
                await _masterDb.Companies
                    .AsNoTracking()
                    .ToListAsync();


            // ==========================================
            // BUILD USER ROWS
            // ==========================================

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
                                ?? "System / No Company"
                        };
                    })
                    .ToList();


            UsersListView.ItemsSource =
                userRows;

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
            // ==========================================
            // USER INFORMATION
            // ==========================================

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

            roleBox.Items.Add("Super Admin");
            roleBox.Items.Add("Admin");
            roleBox.Items.Add("Technician");
            roleBox.Items.Add("Receptionist");



            if (CurrentUser.Role == "Admin")
            {
                roleBox.Items.Remove("Super Admin");
            }

            roleBox.SelectedIndex = 1;


            // ==========================================
            // EMPLOYEE INFORMATION
            // ==========================================

            var firstNameBox = new TextBox
            {
                Header = "First Name",
                PlaceholderText = "Enter first name"
            };

            var middleNameBox = new TextBox
            {
                Header = "Middle Name",
                PlaceholderText = "Enter middle name"
            };

            var lastNameBox = new TextBox
            {
                Header = "Last Name",
                PlaceholderText = "Enter last name"
            };

            var phoneBox = new TextBox
            {
                Header = "Phone",
                PlaceholderText = "Enter phone number"
            };

            var addressBox = new TextBox
            {
                Header = "Address",
                PlaceholderText = "Enter address"
            };

            var positionBox = new TextBox
            {
                Header = "Job Position",
                PlaceholderText = "Enter job position"
            };

            roleBox.SelectionChanged += (_, _) =>
            {
                switch (roleBox.SelectedItem?.ToString())
                {
                    case "Admin":
                        positionBox.Text = "Administrator";
                        break;

                    case "Technician":
                        positionBox.Text = "Computer Technician";
                        break;

                    case "Receptionist":
                        positionBox.Text = "Receptionist";
                        break;

                    case "Super Admin":
                        positionBox.Text = string.Empty;
                        break;
                }
            };

            var hireDatePicker = new DatePicker
            {
                Header = "Hire Date",
                Date = DateTimeOffset.Now
            };


            // ==========================================
            // COMPANY
            // ==========================================

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

            if (CurrentUser.Role == "Admin")
            {
                companyBox.IsEnabled = false;
            }

            foreach (var company in companies)
            {
                companyBox.Items.Add(company);
            }

            if (CurrentUser.Role == "Admin")
            {
                var myCompany =
                    companies.FirstOrDefault(
                        c => c.CompanyId == CurrentUser.CompanyId);

                companyBox.SelectedItem = myCompany;
            }
            else
            {
                if (companies.Count > 0)
                {
                    companyBox.SelectedIndex = 0;
                }
            }


            // ==========================================
            // ACTIVE
            // ==========================================

            var activeCheckBox = new CheckBox
            {
                Content = "Active",
                IsChecked = true
            };


            // ==========================================
            // PANEL
            // ==========================================

            var panel = new StackPanel
            {
                Spacing = 12
            };

            panel.Children.Add(
                new TextBlock
                {
                    Text = "Account Information",
                    FontSize = 18,
                    FontWeight =
                        Microsoft.UI.Text.FontWeights.SemiBold
                });

            panel.Children.Add(usernameBox);
            panel.Children.Add(emailBox);
            panel.Children.Add(passwordBox);
            panel.Children.Add(roleBox);
            panel.Children.Add(companyBox);

            panel.Children.Add(
                new TextBlock
                {
                    Text = "Employee Information",
                    FontSize = 18,
                    FontWeight =
                        Microsoft.UI.Text.FontWeights.SemiBold,
                    Margin = new Thickness(0, 12, 0, 0)
                });

            panel.Children.Add(firstNameBox);
            panel.Children.Add(middleNameBox);
            panel.Children.Add(lastNameBox);
            panel.Children.Add(phoneBox);
            panel.Children.Add(addressBox);
            panel.Children.Add(positionBox);
            panel.Children.Add(hireDatePicker);

            panel.Children.Add(activeCheckBox);


            // ==========================================
            // DIALOG
            // ==========================================

            var scrollViewer = new ScrollViewer
            {
                Content = panel,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                MaxHeight = 550
            };

            var dialog = new ContentDialog
            {
                Title = "Add User",
                Content = scrollViewer,

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

            if (string.IsNullOrWhiteSpace(firstNameBox.Text))
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "First name is required.");

                return;
            }

            if (string.IsNullOrWhiteSpace(lastNameBox.Text))
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Last name is required.");

                return;
            }

            if (string.IsNullOrWhiteSpace(positionBox.Text))
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Position is required.");

                return;
            }


            // ==========================================
            // SELECT ROLE
            // ==========================================

            var selectedRole =
                roleBox.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedRole))
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Please select a role.");

                return;
            }


            // ==========================================
            // DETERMINE COMPANY
            // ==========================================

            int? companyId = null;

            if (selectedRole == "Super Admin")
            {
                // Super Admin is platform-level
                companyId = null;
            }
            else if (CurrentUser.Role == "Admin")
            {
                if (CurrentUser.CompanyId == null)
                {
                    await ShowMessageAsync(
                        "Validation Error",
                        "Unable to determine your company.");

                    return;
                }

                companyId =
                    CurrentUser.CompanyId.Value;
            }
            else
            {
                if (companyBox.SelectedItem is not Company selectedCompany)
                {
                    await ShowMessageAsync(
                        "Validation Error",
                        "Please select a company.");

                    return;
                }

                companyId =
                    selectedCompany.CompanyId;
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
            // CREATE APPLICATION USER
            // ==========================================

            var user = new ApplicationUser
            {
                UserName =
                    usernameBox.Text.Trim(),

                Email =
                    emailBox.Text.Trim(),

                CompanyId =
                    companyId,

                IsActive =
                    activeCheckBox.IsChecked == true
            };


            // ==========================================
            // CREATE USER IN MASTER DB
            // ==========================================

            var createResult =
                await _userManager.CreateAsync(
                    user,
                    passwordBox.Password);

            if (!createResult.Succeeded)
            {
                await ShowIdentityErrorsAsync(
                    "Unable to Add User",
                    createResult);

                return;
            }


            // ==========================================
            // ASSIGN ROLE
            // ==========================================

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


            // ==========================================
            // SUPER ADMIN DOES NOT NEED EMPLOYEE
            // ==========================================

            if (selectedRole == "Super Admin")
            {
                await LoadUsersAsync();

                await ShowMessageAsync(
                    "User Added",
                    $"Super Admin '{user.UserName}' was created successfully.");

                return;
            }


            // ==========================================
            // CREATE EMPLOYEE IN TENANT DB
            // ==========================================

            if (!companyId.HasValue)
            {
                await ShowMessageAsync(
                    "Employee Creation Error",
                    "A company is required for employee users.");

                await _userManager.RemoveFromRoleAsync(
                    user,
                    selectedRole);

                await _userManager.DeleteAsync(user);

                return;
            }

            try
            {
                await using var tenantDb =
                    await _tenantDbFactory.CreateAsync(
                        companyId.Value);

                var employee = new Employee
                {
                    MasterUserId = user.Id,

                    BranchId = null,
                    DepartmentId = null,

                    FirstName =
                        firstNameBox.Text.Trim(),

                    MiddleName =
                        string.IsNullOrWhiteSpace(
                            middleNameBox.Text)
                            ? null
                            : middleNameBox.Text.Trim(),

                    LastName =
                        lastNameBox.Text.Trim(),

                    Phone =
                        string.IsNullOrWhiteSpace(
                            phoneBox.Text)
                            ? null
                            : phoneBox.Text.Trim(),

                    Email =
                        emailBox.Text.Trim(),

                    Address =
                        string.IsNullOrWhiteSpace(
                            addressBox.Text)
                            ? null
                            : addressBox.Text.Trim(),

                    Position =
                        positionBox.Text.Trim(),

                    HireDate =
                        hireDatePicker.Date.Date,

                    IsActive =
                        activeCheckBox.IsChecked == true
                };

                tenantDb.Employees.Add(employee);

                await tenantDb.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Employee creation failed.
                // Remove the Master DB user so we don't
                // leave an account without an employee record.

                await _userManager.RemoveFromRoleAsync(
                    user,
                    selectedRole);

                await _userManager.DeleteAsync(user);

                await ShowMessageAsync(
                    "Employee Creation Error",
                    $"The user could not be created because the employee record failed.\n\n{ex.Message}");

                return;
            }


            // ==========================================
            // REFRESH
            // ==========================================

            await LoadUsersAsync();

            await ShowMessageAsync(
                "User Added",
                $"User '{user.UserName}' and the employee record were created successfully.");
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

            var currentRoles =
                await _userManager.GetRolesAsync(user);

            var currentRole =
                currentRoles.FirstOrDefault();


            if (CurrentUser.Role == "Admin" &&
                user.CompanyId != CurrentUser.CompanyId)
            {
                await ShowMessageAsync(
                    "Access Denied",
                    "You can only edit users from your own company.");

                return;
            }
            var isSuperAdmin =
                await _userManager.IsInRoleAsync(
                    user,
                    "Super Admin");

            if (CurrentUser.Role == "Admin" &&
                isSuperAdmin)
            {
                await ShowMessageAsync(
                    "Access Denied",
                    "You cannot edit a Super Admin.");

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
            var roleBox = new ComboBox
            {
                Header = "Role",
                PlaceholderText = "Select a role"
            };

            roleBox.Items.Add("Admin");
            roleBox.Items.Add("Technician");
            roleBox.Items.Add("Receptionist");

            if (CurrentUser.Role == "Super Admin")
            {
                roleBox.Items.Add("Super Admin");
            }

            roleBox.SelectedItem =
                currentRole;

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

            if (CurrentUser.Role == "Admin")
            {
                var myCompany =
                    companies.FirstOrDefault(
                        c => c.CompanyId == CurrentUser.CompanyId);

                companyBox.SelectedItem = myCompany;
                companyBox.IsEnabled = false;
            }
            else
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
            panel.Children.Add(roleBox);
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

            Company? selectedCompany = null;

            if (CurrentUser.Role == "Admin")
            {
                selectedCompany =
                    companies.FirstOrDefault(
                        c => c.CompanyId == CurrentUser.CompanyId);

                if (selectedCompany == null)
                {
                    await ShowMessageAsync(
                        "Validation Error",
                        "Your company could not be found.");

                    return;
                }
            }
            else
            {
                if (companyBox.SelectedItem is not Company company)
                {
                    await ShowMessageAsync(
                        "Validation Error",
                        "Please select a company.");

                    return;
                }

                selectedCompany = company;
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
            var selectedRole =
                roleBox.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedRole))
            {
                await ShowMessageAsync(
                    "Validation Error",
                    "Please select a role.");

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
            // UPDATE ROLE
            // ==========================================

            if (!string.IsNullOrWhiteSpace(currentRole) &&
                currentRole != selectedRole)
            {
                var removeRoleResult =
                    await _userManager.RemoveFromRoleAsync(
                        user,
                        currentRole);

                if (!removeRoleResult.Succeeded)
                {
                    await ShowIdentityErrorsAsync(
                        "Unable to Remove Previous Role",
                        removeRoleResult);

                    return;
                }

                var addRoleResult =
                    await _userManager.AddToRoleAsync(
                        user,
                        selectedRole);

                if (!addRoleResult.Succeeded)
                {
                    await ShowIdentityErrorsAsync(
                        "Unable to Assign New Role",
                        addRoleResult);

                    // Restore previous role
                    await _userManager.AddToRoleAsync(
                        user,
                        currentRole);

                    return;
                }
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

            var isSuperAdmin =
                await _userManager.IsInRoleAsync(
                    user,
                    "Super Admin");

            if (CurrentUser.Role == "Admin" && isSuperAdmin)
            {
                await ShowMessageAsync(
                    "Access Denied",
                    "You cannot delete a Super Admin.");

                return;
            }

            if (CurrentUser.Role == "Admin" &&
                user.CompanyId != CurrentUser.CompanyId)
            {
                await ShowMessageAsync(
                    "Access Denied",
                    "You can only delete users from your own company.");

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