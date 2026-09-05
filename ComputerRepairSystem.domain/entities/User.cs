
using ComputerRepairSystem.domain.entities;

namespace ComputerRepairSystem.domain.Entities;

public class User
{
    public int UserId { get; set; }

    public int CompanyId { get; set; }
    public int RoleId { get; set; }

    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public DateTime? LastLogin { get; set; }

    public Company Company { get; set; } = null!;
    public Role Role { get; set; } = null!;
}