using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;
using ComputerRepairSystem.company.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem.company.Services;

public class CustomerService
{
    private readonly
        IDbContextFactory<TenantDbContext>
        _contextFactory;

    private readonly
        ICustomerRepository
        _customerRepository;


    public CustomerService(
        IDbContextFactory<TenantDbContext>
            contextFactory,

        ICustomerRepository
            customerRepository)
    {
        _contextFactory =
            contextFactory;

        _customerRepository =
            customerRepository;
    }


    // ==========================================
    // GET ALL
    // ==========================================

    public async Task<List<Customer>>
        GetAllAsync()
    {
        return await _customerRepository
            .GetAllAsync();
    }


    // ==========================================
    // GET BY ID
    // ==========================================

    public async Task<Customer?>
        GetByIdAsync(int customerId)
    {
        return await _customerRepository
            .GetByIdAsync(customerId);
    }


    // ==========================================
    // ADD
    // ==========================================

    public async Task<Customer>
        AddAsync(Customer customer)
    {
        await using var context =
            await _contextFactory
                .CreateDbContextAsync();

        await using var transaction =
            await context.Database
                .BeginTransactionAsync();

        try
        {
            // Add customer
            context.Customers.Add(customer);

            await context.SaveChangesAsync();


            // Add sync queue
            context.SyncQueues.Add(
                new SyncQueue
                {
                    TableName = "Customers",

                    RecordId =
                        customer.CustomerId,

                    Operation = "INSERT",

                    CreatedAt =
                        DateTime.UtcNow,

                    IsSynced = false
                });


            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return customer;
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }


    // ==========================================
    // UPDATE
    // ==========================================

    public async Task UpdateAsync(
        Customer customer)
    {
        await using var context =
            await _contextFactory
                .CreateDbContextAsync();

        await using var transaction =
            await context.Database
                .BeginTransactionAsync();

        try
        {
            // Attach and update customer
            context.Customers.Update(customer);

            await context.SaveChangesAsync();


            // Add sync queue
            context.SyncQueues.Add(
                new SyncQueue
                {
                    TableName = "Customers",

                    RecordId =
                        customer.CustomerId,

                    Operation = "UPDATE",

                    CreatedAt =
                        DateTime.UtcNow,

                    IsSynced = false
                });


            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }


    // ==========================================
    // DELETE
    // ==========================================

    public async Task DeleteAsync(
        int customerId)
    {
        await using var context =
            await _contextFactory
                .CreateDbContextAsync();

        await using var transaction =
            await context.Database
                .BeginTransactionAsync();

        try
        {
            var customer =
                await context.Customers
                    .FirstOrDefaultAsync(
                        x =>
                            x.CustomerId ==
                            customerId);


            if (customer == null)
                return;


            // Delete customer
            context.Customers.Remove(customer);

            await context.SaveChangesAsync();


            // Add sync queue
            context.SyncQueues.Add(
                new SyncQueue
                {
                    TableName = "Customers",

                    RecordId =
                        customerId,

                    Operation = "DELETE",

                    CreatedAt =
                        DateTime.UtcNow,

                    IsSynced = false
                });


            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }
}