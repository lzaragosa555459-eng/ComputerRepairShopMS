using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;
using ComputerRepairSystem.company.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem.company.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDbContextFactory<TenantDbContext>
        _contextFactory;

    public CustomerRepository(
        IDbContextFactory<TenantDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }


    // ==========================================
    // GET ALL
    // ==========================================

    public async Task<List<Customer>> GetAllAsync()
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        return await context.Customers
            .AsNoTracking()
            .ToListAsync();
    }


    // ==========================================
    // GET BY ID
    // ==========================================

    public async Task<Customer?> GetByIdAsync(
        int customerId)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        return await context.Customers
            .FirstOrDefaultAsync(
                x => x.CustomerId == customerId);
    }


    // ==========================================
    // ADD
    // ==========================================

    public async Task<Customer> AddAsync(
        Customer customer)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        context.Customers.Add(customer);

        await context.SaveChangesAsync();

        return customer;
    }


    // ==========================================
    // UPDATE
    // ==========================================

    public async Task UpdateAsync(
        Customer customer)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        context.Customers.Update(customer);

        await context.SaveChangesAsync();
    }


    // ==========================================
    // DELETE
    // ==========================================

    public async Task DeleteAsync(
        int customerId)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        var customer =
            await context.Customers
                .FirstOrDefaultAsync(
                    x => x.CustomerId == customerId);

        if (customer == null)
            return;

        context.Customers.Remove(customer);

        await context.SaveChangesAsync();
    }
}