using ComputerRepairSystem.company.Services;

namespace ComputerRepairSystem_winforms;

public partial class Form1 : Form
{
    private readonly CustomerService _customerService;

    public Form1(CustomerService customerService)
    {
        InitializeComponent();

        _customerService = customerService;

        Load += Form1_Load;
    }

    public Form1()
    {
        InitializeComponent();

        _customerService = null!;
    }

    private void Form1_Load(object? sender, EventArgs e)
    {
        var customerForm = new CustomerForm(_customerService);

        customerForm.Show(this);
    }
}