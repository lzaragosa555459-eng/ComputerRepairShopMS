namespace ComputerRepairSystem.company.Entities;

public class SyncQueue
{
    public int SyncId { get; set; }

    public string TableName { get; set; } = string.Empty;

    public int RecordId { get; set; }

    public string Operation { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsSynced { get; set; } = false;

    public DateTime? SyncedAt { get; set; }
}