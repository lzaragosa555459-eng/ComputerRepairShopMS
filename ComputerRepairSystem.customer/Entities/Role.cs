

namespace ComputerRepairSystem.company.Entities;

public class Role
{
    public long RoleId { get; set; }

    public string RoleName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public ICollection<User> Users { get; set; }
        = new List<User>();
}