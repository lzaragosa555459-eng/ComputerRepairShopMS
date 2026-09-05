using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;

namespace ComputerRepairSystem.company.Services;

public class SyncQueueService
{
    private readonly TenantDbContext _context;

    public SyncQueueService(TenantDbContext context)
    {
        _context = context;
    }

    public async Task QueueAsync(
        string tableName,
        int recordId,
        string operation)
    {
        var syncQueue = new SyncQueue
        {
            TableName = tableName,
            RecordId = recordId,
            Operation = operation,
            CreatedAt = DateTime.UtcNow,
            IsSynced = false
        };

        _context.SyncQueues.Add(syncQueue);

        await _context.SaveChangesAsync();
    }
}