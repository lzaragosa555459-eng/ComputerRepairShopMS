using ComputerRepairSystem.customer.Entities;

namespace ComputerRepairSystem.company.Entities;

public class RepairItem
{
    public int RepairItemId { get; set; }

    public int RepairId { get; set; }

    public int ItemId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }

    // Relationships
    public Repair Repair { get; set; } = null!;

    public InventoryItem Item { get; set; } = null!;
}