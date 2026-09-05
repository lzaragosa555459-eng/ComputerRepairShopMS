namespace ComputerRepairSystem.company.Entities;

public class Device
{
    public int DeviceId { get; set; }

    public int CustomerId { get; set; }

    public string DeviceType { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string? SerialNumber { get; set; }

    public string? DeviceCondition { get; set; }

    // Relationships
    public Customer Customer { get; set; } = null!;

    public ICollection<ServiceRequest> ServiceRequests { get; set; }
        = new List<ServiceRequest>();
}