namespace ComputerRepairSystem.company.Entities;

public class Inventory
{
    public long InventoryId { get; set; }

    public long BranchId { get; set; }

    public long ItemId { get; set; }

    public decimal QuantityOnHand { get; set; } = 0;

    // Relationships
    public Branch Branch { get; set; } = null!;

    public InventoryItem Item { get; set; } = null!;
}