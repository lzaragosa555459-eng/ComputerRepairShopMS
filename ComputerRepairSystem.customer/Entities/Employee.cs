namespace ComputerRepairSystem.company.Entities;

public class Employee
{
    public int EmployeeId { get; set; }

    // Reference to the Master DB user
    public string? MasterUserId { get; set; }

    public int? BranchId { get; set; }
    public int? DepartmentId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;

    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public string Position { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Branch? Branch { get; set; } = null!;
    public Department? Department { get; set; }
}