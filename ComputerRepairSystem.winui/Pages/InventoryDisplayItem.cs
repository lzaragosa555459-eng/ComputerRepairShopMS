namespace ComputerRepairSystem_winui.Pages;

public class InventoryDisplayItem
{
    public int InventoryId { get; set; }

    public int ItemId { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public string Unit { get; set; } = string.Empty;

    public decimal QuantityOnHand { get; set; }

    public decimal AvailableQuantity { get; set; }
}