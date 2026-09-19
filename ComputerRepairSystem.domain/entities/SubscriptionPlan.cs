namespace ComputerRepairSystem.domain.Entities;

public class SubscriptionPlan
{
    public int SubscriptionPlanId { get; set; }

    public string PlanName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int DurationInDays { get; set; }

    public bool IsActive { get; set; } = true;
}