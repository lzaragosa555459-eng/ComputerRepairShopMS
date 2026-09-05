namespace ComputerRepairSystem.company.Entities;

public class Inventory
{
    public int InventoryId { get; set; }

    public int BranchId { get; set; }

    public int ItemId { get; set; }

    public decimal QuantityOnHand { get; set; } = 0;

    // Relationships
    public Branch Branch { get; set; } = null!;

    public InventoryItem Item { get; set; } = null!;
}