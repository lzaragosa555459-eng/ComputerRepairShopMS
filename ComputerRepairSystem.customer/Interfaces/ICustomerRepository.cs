using ComputerRepairSystem.company.Entities;

namespace ComputerRepairSystem.company.Interfaces;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync();

    Task<Customer?> GetByIdAsync(int customerId);

    Task<Customer> AddAsync(Customer customer);

    Task UpdateAsync(Customer customer);

    Task DeleteAsync(int customerId);
}