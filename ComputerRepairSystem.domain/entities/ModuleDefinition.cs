namespace ComputerRepairSystem.domain.Entities;

public class ModuleDefinition
{
    public int ModuleDefinitionId { get; set; }

    public string ModuleCode { get; set; } = string.Empty;

    public string ModuleName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }
}