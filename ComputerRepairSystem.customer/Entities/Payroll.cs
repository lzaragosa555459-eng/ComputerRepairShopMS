using ComputerRepairSystem.company.Entities;

public class Payroll
{
    public int PayrollId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }

    public decimal BasicSalary { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary { get; set; }

    public Employee? Employee { get; set; }
}