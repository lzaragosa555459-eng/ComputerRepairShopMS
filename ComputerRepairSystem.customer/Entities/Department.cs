namespace ComputerRepairSystem.company.Entities;

public class Department
{
    public long DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public ICollection<Employee> Employees { get; set; }
        = new List<Employee>();
}