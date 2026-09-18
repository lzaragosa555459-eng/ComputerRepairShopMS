using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;
using ComputerRepairSystem.infrastructure.data;
using ComputerRepairSystem_winui.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ComputerRepairSystem_winui.Pages;

public sealed partial class SettingsPage : Page
{
    private readonly TenantDbContextFactory
        _tenantDbFactory;

    public SettingsPage(
        TenantDbContextFactory tenantDbFactory)
    {
        InitializeComponent();

        _tenantDbFactory =
            tenantDbFactory;

        Loaded += SettingsPage_Loaded;
    }


    private async void SettingsPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            await LoadSettingsAsync();
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Settings Error",
                ex.Message);

            System.Diagnostics.Debug.WriteLine(ex);
        }
    }


    private async Task LoadSettingsAsync()
    {
        if (CurrentUser.CompanyId == null)
        {
            return;
        }

        await using var db =
            await _tenantDbFactory.CreateAsync(
                CurrentUser.CompanyId.Value);

        var settings =
            await db.SystemSettings
                .FirstOrDefaultAsync();

        if (settings == null)
            return;

        ShopNameBox.Text =
            settings.ShopName;

        ShopAddressBox.Text =
            settings.ShopAddress ?? string.Empty;

        LowLaborRateBox.Value =
            (double)settings.LowLaborRate;

        MediumLaborRateBox.Value =
            (double)settings.MediumLaborRate;

        HighLaborRateBox.Value =
            (double)settings.HighLaborRate;
    }


    private async void SaveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ShopNameBox.Text))
        {
            await ShowMessageAsync(
                "Missing Shop Name",
                "Please enter the shop name.");

            return;
        }

        try
        {
            if (CurrentUser.CompanyId == null)
            {
                return;
            }

            await using var db =
                await _tenantDbFactory.CreateAsync(
                    CurrentUser.CompanyId.Value);

            var settings =
                await db.SystemSettings
                    .FirstOrDefaultAsync();

            if (settings == null)
            {
                settings = new SystemSettings();

                db.SystemSettings.Add(settings);
            }

            settings.ShopName =
                ShopNameBox.Text.Trim();

            settings.ShopAddress =
                string.IsNullOrWhiteSpace(ShopAddressBox.Text)
                    ? null
                    : ShopAddressBox.Text.Trim();

            settings.LowLaborRate =
                double.IsNaN(LowLaborRateBox.Value)
                    ? 0
                    : (decimal)LowLaborRateBox.Value;

            settings.MediumLaborRate =
                double.IsNaN(MediumLaborRateBox.Value)
                    ? 0
                    : (decimal)MediumLaborRateBox.Value;

            settings.HighLaborRate =
                double.IsNaN(HighLaborRateBox.Value)
                    ? 0
                    : (decimal)HighLaborRateBox.Value;

            settings.UpdatedAt =
                DateTime.UtcNow;

            await db.SaveChangesAsync();

            await ShowMessageAsync(
                "Settings Saved",
                "System settings have been updated.");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync(
                "Settings Error",
                ex.Message);

            System.Diagnostics.Debug.WriteLine(ex);
        }
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