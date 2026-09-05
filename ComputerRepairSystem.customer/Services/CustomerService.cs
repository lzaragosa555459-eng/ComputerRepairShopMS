using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;
using ComputerRepairSystem.company.Interfaces;

namespace ComputerRepairSystem.company.Services;

public class CustomerService
{
    private readonly TenantDbContext _context;
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(
        TenantDbContext context,
        ICustomerRepository customerRepository)
    {
        _context = context;
        _customerRepository = customerRepository;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _customerRepository.GetAllAsync();
    }

    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        return await _customerRepository.GetByIdAsync(customerId);
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            _context.SyncQueues.Add(new SyncQueue
            {
                TableName = "Customers",
                RecordId = customer.CustomerId,
                Operation = "INSERT",
                CreatedAt = DateTime.UtcNow,
                IsSynced = false
            });

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return customer;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateAsync(Customer customer)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Customers.Update(customer);

            await _context.SaveChangesAsync();

            _context.SyncQueues.Add(new SyncQueue
            {
                TableName = "Customers",
                RecordId = customer.CustomerId,
                Operation = "UPDATE",
                CreatedAt = DateTime.UtcNow,
                IsSynced = false
            });

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteAsync(int customerId)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
                return;

            _context.Customers.Remove(customer);

            // Save deletion before adding queue record
            await _context.SaveChangesAsync();

            _context.SyncQueues.Add(new SyncQueue
            {
                TableName = "Customers",
                RecordId = customerId,
                Operation = "DELETE",
                CreatedAt = DateTime.UtcNow,
                IsSynced = false
            });

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}