

namespace ComputerRepairSystem.company.Entities;

public class Employee
{
    public long EmployeeId { get; set; }

    // Organization
    public long BranchId { get; set; }
    public long? DepartmentId { get; set; }

    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;

    // Contact Information
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    // Employment Information
    public string Position { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Relationships
    public Branch Branch { get; set; } = null!;
    public Department? Department { get; set; }
    public User? User { get; set; }
}