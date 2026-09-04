

namespace ComputerRepairSystem.company.Entities;

public class Repair
{
    public long RepairId { get; set; }

    public long ServiceRequestId { get; set; }

    public long TechnicianId { get; set; }

    public long BranchId { get; set; }

    public string? Diagnosis { get; set; }

    public string? RepairDescription { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    // Relationships
    public ServiceRequest ServiceRequest { get; set; } = null!;

    public Employee Technician { get; set; } = null!;

    public ICollection<RepairItem> RepairItems { get; set; }
        = new List<RepairItem>();

    public Invoice? Invoice { get; set; }
}