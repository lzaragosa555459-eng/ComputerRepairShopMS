namespace ComputerRepairSystem.company.Entities;

public class Payment
{
    public long PaymentId { get; set; }

    public long InvoiceId { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string? ReferenceNumber { get; set; }

    public string Status { get; set; } = "Completed";

    // Relationship
    public Invoice Invoice { get; set; } = null!;
}