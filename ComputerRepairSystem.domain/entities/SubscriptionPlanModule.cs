namespace ComputerRepairSystem.domain.Entities;

public class SubscriptionPlanModule
{
    public int SubscriptionPlanModuleId { get; set; }

    public int SubscriptionPlanId { get; set; }

    public int ModuleDefinitionId { get; set; }

    public SubscriptionPlan? SubscriptionPlan { get; set; }

    public ModuleDefinition? ModuleDefinition { get; set; }
}