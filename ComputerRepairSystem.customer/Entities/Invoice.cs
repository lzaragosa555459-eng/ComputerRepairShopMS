namespace ComputerRepairSystem.company.Entities;

public class Invoice
{
    public int InvoiceId { get; set; }

    public int RepairId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public decimal Subtotal { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Unpaid";

    // Relationships
    public Repair Repair { get; set; } = null!;

    public ICollection<Payment> Payments { get; set; }
        = new List<Payment>();
}