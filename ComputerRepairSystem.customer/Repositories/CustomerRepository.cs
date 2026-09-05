using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;
using ComputerRepairSystem.company.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem.company.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly TenantDbContext _context;

    public CustomerRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(x => x.CustomerId == customerId);
    }

    public Task<Customer> AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);

        return Task.FromResult(customer);
    }

    public Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);

        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int customerId)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(x => x.CustomerId == customerId);

        if (customer == null)
            return;

        _context.Customers.Remove(customer);
    }
}