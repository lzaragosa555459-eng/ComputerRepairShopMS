namespace ComputerRepairSystem.company.Entities;

public class SystemSettings
{
    public int SystemSettingsId { get; set; }

    public string ShopName { get; set; } = string.Empty;

    public string? ShopAddress { get; set; }

    public decimal LowLaborRate { get; set; }

    public decimal MediumLaborRate { get; set; }

    public decimal HighLaborRate { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}