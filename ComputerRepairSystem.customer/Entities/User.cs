namespace ComputerRepairSystem.company.Entities;



public class User
{
    public long UserId { get; set; }

    // Employee Account
    public long? EmployeeId { get; set; }

    // Role
    public long RoleId { get; set; }

    // Login Information
    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    // Account Status
    public bool IsActive { get; set; } = true;

    public DateTime? LastLogin { get; set; }

    // Relationships
    public Employee? Employee { get; set; }

    public Role Role { get; set; } = null!;
}