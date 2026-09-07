using ComputerRepairSystem.domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ComputerRepairSystem.infrastructure.Entities;

public class ApplicationUser : IdentityUser
{
    public int CompanyId { get; set; }

    public Company Company { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public DateTime? LastLogin { get; set; }
}