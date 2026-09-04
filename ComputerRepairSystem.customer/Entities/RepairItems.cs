using ComputerRepairSystem.customer.Entities;

namespace ComputerRepairSystem.company.Entities;

public class RepairItem
{
    public long RepairItemId { get; set; }

    public long RepairId { get; set; }

    public long ItemId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }

    // Relationships
    public Repair Repair { get; set; } = null!;

    public InventoryItem Item { get; set; } = null!;
}