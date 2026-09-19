namespace ComputerRepairSystem.domain.Entities;

public class Subscription
{
    public int SubscriptionId { get; set; }

    public int CompanyId { get; set; }

    public int SubscriptionPlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? TrialEndsAt { get; set; }

    public string Status { get; set; } = "Pending";

    public Company? Company { get; set; }

    public SubscriptionPlan? SubscriptionPlan { get; set; }
}