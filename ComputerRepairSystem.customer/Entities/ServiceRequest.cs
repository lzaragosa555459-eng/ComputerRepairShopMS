namespace ComputerRepairSystem.company.Entities;

public class ServiceRequest
{
    public long ServiceRequestId { get; set; }

    public long DeviceId { get; set; }

    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public string Priority { get; set; } = "Medium";

    // Relationships
    public Device Device { get; set; } = null!;

    public Repair? Repair { get; set; }
}