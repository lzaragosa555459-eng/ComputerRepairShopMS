using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ComputerRepairSystem_winui.Pages;

public sealed partial class InventoryManagementPage : Page
{
    private readonly IDbContextFactory<TenantDbContext> _dbFactory;

    private List<InventoryDisplayItem> _inventory = new();

    public InventoryManagementPage(
        IDbContextFactory<TenantDbContext> dbFactory)
    {
        InitializeComponent();

        _dbFactory = dbFactory;

        Loaded += InventoryManagementPage_Loaded;
    }


    // ==============================
    // PAGE LOADED
    // ==============================

    private async void InventoryManagementPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await LoadInventoryAsync();
    }


    // ==============================
    // LOAD INVENTORY
    // ==============================

    private async Task LoadInventoryAsync()
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var inventory = await db.Inventories
            .Include(x => x.Item)
            .Where(x => x.Item != null)
            .OrderBy(x => x.Item!.ItemName)
            .ToListAsync();

        var itemIds = inventory
            .Select(x => x.ItemId)
            .ToList();

        var usedQuantities = await db.RepairItems
            .Where(x => itemIds.Contains(x.ItemId))
            .GroupBy(x => x.ItemId)
            .Select(g => new
            {
                ItemId = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToDictionaryAsync(
                x => x.ItemId,
                x => x.Quantity);

        _inventory = inventory
            .Select(x =>
            {
                var usedQuantity =
                    usedQuantities.TryGetValue(
                        x.ItemId,
                        out var quantity)
                        ? quantity
                        : 0;

                return new InventoryDisplayItem
                {
                    InventoryId = x.InventoryId,
                    ItemId = x.ItemId,
                    ItemName = x.Item!.ItemName,
                    Category = x.Item.Category,
                    Brand = x.Item.Brand,
                    Model = x.Item.Model,
                    Unit = x.Item.Unit,
                    QuantityOnHand = x.QuantityOnHand,
                    AvailableQuantity = x.QuantityOnHand - usedQuantity
                };
            })
            .ToList();

        InventoryList.ItemsSource = _inventory;
    }



    // ==============================
    // SEARCH
    // ==============================

private void SearchBox_TextChanged(
    object sender,
    TextChangedEventArgs e)
    {
        var search =
            SearchBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(search))
        {
            InventoryList.ItemsSource = _inventory;
            return;
        }

        var filtered =
            _inventory
                .Where(x =>
                    x.ItemName.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                    || x.Category.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                    || (x.Brand ?? "")
                        .Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase)

                    || (x.Model ?? "")
                        .Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

        InventoryList.ItemsSource = filtered;
    }


    // ==============================
    // NEW ITEM
    // ==============================

    private async void NewItemButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var panel = new StackPanel
        {
            Spacing = 10
        };


        var itemNameBox = new TextBox
        {
            Header = "Item Name",
            PlaceholderText = "Example: Laptop RAM"
        };


        var categoryBox = new TextBox
        {
            Header = "Category",
            PlaceholderText = "Example: Memory"
        };


        var brandBox = new TextBox
        {
            Header = "Brand"
        };


        var modelBox = new TextBox
        {
            Header = "Model"
        };


        var unitBox = new TextBox
        {
            Header = "Unit",
            Text = "Piece"
        };


        var unitCostBox = new NumberBox
        {
            Header = "Unit Cost",
            Minimum = 0
        };


        var unitPriceBox = new NumberBox
        {
            Header = "Unit Price",
            Minimum = 0
        };


        var reorderLevelBox = new NumberBox
        {
            Header = "Reorder Level",
            Minimum = 0
        };


        panel.Children.Add(itemNameBox);
        panel.Children.Add(categoryBox);
        panel.Children.Add(brandBox);
        panel.Children.Add(modelBox);
        panel.Children.Add(unitBox);
        panel.Children.Add(unitCostBox);
        panel.Children.Add(unitPriceBox);
        panel.Children.Add(reorderLevelBox);


        var scrollViewer = new ScrollViewer
        {
            Content = panel,
            MaxHeight = 500
        };


        var dialog = new ContentDialog
        {
            Title = "New Inventory Item",
            Content = scrollViewer,
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            XamlRoot = XamlRoot
        };


        var result =
            await dialog.ShowAsync();

        if (result != ContentDialogResult.Primary)
            return;


        if (string.IsNullOrWhiteSpace(itemNameBox.Text))
        {
            await ShowMessageAsync(
                "Missing Item Name",
                "Please enter an item name.");

            return;
        }


        if (string.IsNullOrWhiteSpace(categoryBox.Text))
        {
            await ShowMessageAsync(
                "Missing Category",
                "Please enter a category.");

            return;
        }


        await using var db =
            await _dbFactory.CreateDbContextAsync();


        var item = new InventoryItem
        {
            ItemName = itemNameBox.Text.Trim(),
            Category = categoryBox.Text.Trim(),
            Brand = string.IsNullOrWhiteSpace(brandBox.Text)
                ? null
                : brandBox.Text.Trim(),
            Model = string.IsNullOrWhiteSpace(modelBox.Text)
                ? null
                : modelBox.Text.Trim(),
            Unit = string.IsNullOrWhiteSpace(unitBox.Text)
                ? "Piece"
                : unitBox.Text.Trim(),
            UnitCost = (decimal)unitCostBox.Value,
            UnitPrice = (decimal)unitPriceBox.Value,
            ReorderLevel = (decimal)reorderLevelBox.Value,
            IsActive = true
        };


        db.InventoryItems.Add(item);

        await db.SaveChangesAsync();


        await LoadInventoryAsync();


        await ShowMessageAsync(
            "Item Added",
            "The inventory item has been added successfully.");
    }


    // ==============================
    // ADD STOCK
    // ==============================

    private async void AddStockButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();


        var items = await db.InventoryItems
            .Where(x => x.IsActive)
            .OrderBy(x => x.ItemName)
            .ToListAsync();


        if (items.Count == 0)
        {
            await ShowMessageAsync(
                "No Inventory Items",
                "Add an inventory item first.");

            return;
        }


        var itemComboBox = new ComboBox
        {
            Header = "Inventory Item",
            PlaceholderText = "Select an item",
            ItemsSource = items,
            DisplayMemberPath = "ItemName"
        };


        var quantityBox = new NumberBox
        {
            Header = "Quantity",
            Minimum = 1,
            Value = 1
        };


        var panel = new StackPanel
        {
            Spacing = 12
        };

        panel.Children.Add(itemComboBox);
        panel.Children.Add(quantityBox);


        var dialog = new ContentDialog
        {
            Title = "Add Stock",
            Content = panel,
            PrimaryButtonText = "Add Stock",
            CloseButtonText = "Cancel",
            XamlRoot = XamlRoot
        };


        var result =
            await dialog.ShowAsync();


        if (result != ContentDialogResult.Primary)
            return;


        if (itemComboBox.SelectedItem is not InventoryItem selectedItem)
        {
            await ShowMessageAsync(
                "No Item Selected",
                "Please select an inventory item.");

            return;
        }


        var quantity =
            (decimal)quantityBox.Value;


        var inventory =
            await db.Inventories
                .FirstOrDefaultAsync(
                    x =>
                        x.ItemId == selectedItem.ItemId &&
                        x.BranchId == null);


        if (inventory == null)
        {
            inventory = new Inventory
            {
                ItemId = selectedItem.ItemId,
                BranchId = null,
                QuantityOnHand = quantity
            };

            db.Inventories.Add(inventory);
        }
        else
        {
            inventory.QuantityOnHand += quantity;
        }


        await db.SaveChangesAsync();


        await LoadInventoryAsync();


        await ShowMessageAsync(
            "Stock Added",
            $"{selectedItem.ItemName} stock has been updated.");
    }


    // ==============================
    // EDIT ITEM
    // ==============================

    private async void EditItemButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Tag is not InventoryDisplayItem displayItem)
        {
            return;
        }

        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var item =
            await db.InventoryItems
                .FirstOrDefaultAsync(
                    x => x.ItemId == displayItem.ItemId);

        if (item == null)
        {
            await ShowMessageAsync(
                "Item Not Found",
                "The inventory item could not be found.");

            return;
        }


        // ==============================
        // INPUT FIELDS
        // ==============================

        var itemNameBox = new TextBox
        {
            Header = "Item Name",
            Text = item.ItemName
        };

        var categoryBox = new TextBox
        {
            Header = "Category",
            Text = item.Category
        };

        var descriptionBox = new TextBox
        {
            Header = "Description",
            Text = item.Description ?? "",
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap
        };

        var brandBox = new TextBox
        {
            Header = "Brand",
            Text = item.Brand ?? ""
        };

        var modelBox = new TextBox
        {
            Header = "Model",
            Text = item.Model ?? ""
        };

        var unitBox = new TextBox
        {
            Header = "Unit",
            Text = item.Unit
        };

        var unitCostBox = new NumberBox
        {
            Header = "Unit Cost",
            Value = (double)item.UnitCost,
            Minimum = 0
        };

        var unitPriceBox = new NumberBox
        {
            Header = "Unit Price",
            Value = (double)item.UnitPrice,
            Minimum = 0
        };

        var reorderLevelBox = new NumberBox
        {
            Header = "Reorder Level",
            Value = (double)item.ReorderLevel,
            Minimum = 0
        };


        // ==============================
        // FORM
        // ==============================

        var panel = new StackPanel
        {
            Spacing = 10
        };

        panel.Children.Add(itemNameBox);
        panel.Children.Add(categoryBox);
        panel.Children.Add(descriptionBox);
        panel.Children.Add(brandBox);
        panel.Children.Add(modelBox);
        panel.Children.Add(unitBox);
        panel.Children.Add(unitCostBox);
        panel.Children.Add(unitPriceBox);
        panel.Children.Add(reorderLevelBox);


        var scrollViewer = new ScrollViewer
        {
            Content = panel,
            MaxHeight = 500
        };


        // ==============================
        // DIALOG
        // ==============================

        var dialog = new ContentDialog
        {
            Title = "Edit Inventory Item",
            Content = scrollViewer,
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            XamlRoot = XamlRoot
        };

        var result =
            await dialog.ShowAsync();

        if (result != ContentDialogResult.Primary)
            return;


        // ==============================
        // VALIDATION
        // ==============================

        if (string.IsNullOrWhiteSpace(itemNameBox.Text))
        {
            await ShowMessageAsync(
                "Missing Item Name",
                "Please enter an item name.");

            return;
        }

        if (string.IsNullOrWhiteSpace(categoryBox.Text))
        {
            await ShowMessageAsync(
                "Missing Category",
                "Please enter a category.");

            return;
        }


        // ==============================
        // UPDATE ITEM
        // ==============================

        item.ItemName =
            itemNameBox.Text.Trim();

        item.Category =
            categoryBox.Text.Trim();

        item.Description =
            string.IsNullOrWhiteSpace(descriptionBox.Text)
                ? null
                : descriptionBox.Text.Trim();

        item.Brand =
            string.IsNullOrWhiteSpace(brandBox.Text)
                ? null
                : brandBox.Text.Trim();

        item.Model =
            string.IsNullOrWhiteSpace(modelBox.Text)
                ? null
                : modelBox.Text.Trim();

        item.Unit =
            string.IsNullOrWhiteSpace(unitBox.Text)
                ? "Piece"
                : unitBox.Text.Trim();

        item.UnitCost =
            (decimal)unitCostBox.Value;

        item.UnitPrice =
            (decimal)unitPriceBox.Value;

        item.ReorderLevel =
            (decimal)reorderLevelBox.Value;


        await db.SaveChangesAsync();

        await LoadInventoryAsync();

        await ShowMessageAsync(
            "Item Updated",
            "The inventory item has been updated successfully.");
    }


    // ==============================
    // MESSAGE
    // ==============================

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