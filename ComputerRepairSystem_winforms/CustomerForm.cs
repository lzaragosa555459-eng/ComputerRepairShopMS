using ComputerRepairSystem.company.Entities;
using ComputerRepairSystem.company.Services;

namespace ComputerRepairSystem_winforms;

public class CustomerForm : Form
{
    private readonly CustomerService _customerService;

    private TextBox txtFirstName = null!;
    private TextBox txtMiddleName = null!;
    private TextBox txtLastName = null!;
    private TextBox txtPhone = null!;
    private TextBox txtEmail = null!;
    private TextBox txtAddress = null!;

    private Button btnAdd = null!;
    private Button btnUpdate = null!;
    private Button btnDelete = null!;
    private Button btnClear = null!;

    private DataGridView dgvCustomers = null!;

    private int? selectedCustomerId;

    public CustomerForm(CustomerService customerService)
    {
        _customerService = customerService;

        InitializeForm();
        CreateControls();

        Load += CustomerForm_Load;
    }

    private void InitializeForm()
    {
        Text = "Customer Management";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(1100, 700);
        MinimumSize = new Size(900, 600);

        Font = new Font("Segoe UI", 10F);
    }

    private void CreateControls()
    {
        var mainPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            ColumnCount = 2,
            RowCount = 4
        };

        mainPanel.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        mainPanel.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 220));

        mainPanel.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 50));

        mainPanel.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 40));

        mainPanel.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        Controls.Add(mainPanel);

        // =========================
        // INPUT SECTION
        // =========================

        var inputPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 3,
            Padding = new Padding(5)
        };

        for (int i = 0; i < 4; i++)
        {
            inputPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 25));
        }

        for (int i = 0; i < 3; i++)
        {
            inputPanel.RowStyles.Add(
                new RowStyle(SizeType.Percent, 33.33F));
        }

        txtFirstName = CreateTextBox();
        txtMiddleName = CreateTextBox();
        txtLastName = CreateTextBox();
        txtPhone = CreateTextBox();
        txtEmail = CreateTextBox();
        txtAddress = CreateTextBox();

        inputPanel.Controls.Add(CreateField("First Name", txtFirstName), 0, 0);
        inputPanel.Controls.Add(CreateField("Middle Name", txtMiddleName), 1, 0);
        inputPanel.Controls.Add(CreateField("Last Name", txtLastName), 2, 0);
        inputPanel.Controls.Add(CreateField("Phone", txtPhone), 3, 0);

        inputPanel.Controls.Add(CreateField("Email", txtEmail), 0, 1);
        inputPanel.Controls.Add(CreateField("Address", txtAddress), 1, 1);
        inputPanel.SetColumnSpan(txtAddress, 1);

        mainPanel.Controls.Add(inputPanel, 0, 0);

        // =========================
        // BUTTON SECTION
        // =========================

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(5)
        };

        btnAdd = CreateButton("Add");
        btnUpdate = CreateButton("Update");
        btnDelete = CreateButton("Delete");
        btnClear = CreateButton("Clear");

        buttonPanel.Controls.Add(btnAdd);
        buttonPanel.Controls.Add(btnUpdate);
        buttonPanel.Controls.Add(btnDelete);
        buttonPanel.Controls.Add(btnClear);

        mainPanel.Controls.Add(buttonPanel, 0, 1);

        // =========================
        // TITLE
        // =========================

        var lblCustomers = new Label
        {
            Text = "Customers",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        mainPanel.Controls.Add(lblCustomers, 0, 2);

        // =========================
        // DATA GRID
        // =========================

        dgvCustomers = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.CustomerId),
            HeaderText = "ID",
            FillWeight = 10
        });

        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.FirstName),
            HeaderText = "First Name"
        });

        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.MiddleName),
            HeaderText = "Middle Name"
        });

        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.LastName),
            HeaderText = "Last Name"
        });

        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.Phone),
            HeaderText = "Phone"
        });

        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.Email),
            HeaderText = "Email"
        });

        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.Address),
            HeaderText = "Address"
        });

        mainPanel.Controls.Add(dgvCustomers, 0, 3);

        // =========================
        // EVENTS
        // =========================

        btnAdd.Click += BtnAdd_Click;
        btnUpdate.Click += BtnUpdate_Click;
        btnDelete.Click += BtnDelete_Click;
        btnClear.Click += BtnClear_Click;

        dgvCustomers.SelectionChanged += DgvCustomers_SelectionChanged;
    }

    private static TextBox CreateTextBox()
    {
        return new TextBox
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(5)
        };
    }

    private static Button CreateButton(string text)
    {
        return new Button
        {
            Text = text,
            Width = 110,
            Height = 35,
            Margin = new Padding(5)
        };
    }

    private static Panel CreateField(string labelText, Control control)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(5)
        };

        var label = new Label
        {
            Text = labelText,
            Dock = DockStyle.Top,
            Height = 25
        };

        panel.Controls.Add(control);
        panel.Controls.Add(label);

        return panel;
    }

    // =========================
    // LOAD CUSTOMERS
    // =========================

    private async void CustomerForm_Load(object? sender, EventArgs e)
    {
        await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        try
        {
            var customers = await _customerService.GetAllAsync();

            dgvCustomers.DataSource = customers;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to load customers.\n\n{ex.Message}",
                "Database Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    // =========================
    // ADD
    // =========================

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (!ValidateCustomerInput())
            return;

        try
        {
            var customer = new Customer
            {
                FirstName = txtFirstName.Text.Trim(),
                MiddleName = GetNullableText(txtMiddleName),
                LastName = txtLastName.Text.Trim(),
                Phone = GetNullableText(txtPhone),
                Email = GetNullableText(txtEmail),
                Address = GetNullableText(txtAddress)
            };

            await _customerService.AddAsync(customer);

            MessageBox.Show(
                "Customer added successfully.",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            ClearFields();
            await LoadCustomersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to add customer.\n\n{ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    // =========================
    // UPDATE
    // =========================

    private async void BtnUpdate_Click(object? sender, EventArgs e)
    {
        if (selectedCustomerId == null)
        {
            MessageBox.Show(
                "Select a customer first.",
                "Update Customer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        if (!ValidateCustomerInput())
            return;

        try
        {
            var customer = await _customerService
                .GetByIdAsync(selectedCustomerId.Value);

            if (customer == null)
            {
                MessageBox.Show(
                    "Customer was not found.",
                    "Update Customer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            customer.FirstName = txtFirstName.Text.Trim();
            customer.MiddleName = GetNullableText(txtMiddleName);
            customer.LastName = txtLastName.Text.Trim();
            customer.Phone = GetNullableText(txtPhone);
            customer.Email = GetNullableText(txtEmail);
            customer.Address = GetNullableText(txtAddress);

            await _customerService.UpdateAsync(customer);

            MessageBox.Show(
                "Customer updated successfully.",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            ClearFields();
            await LoadCustomersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to update customer.\n\n{ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    // =========================
    // DELETE
    // =========================

    private async void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (selectedCustomerId == null)
        {
            MessageBox.Show(
                "Select a customer first.",
                "Delete Customer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this customer?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
            return;

        try
        {
            await _customerService.DeleteAsync(
                selectedCustomerId.Value);

            MessageBox.Show(
                "Customer deleted successfully.",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            ClearFields();
            await LoadCustomersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to delete customer.\n\n{ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    // =========================
    // GRID SELECTION
    // =========================

    private void DgvCustomers_SelectionChanged(
        object? sender,
        EventArgs e)
    {
        if (dgvCustomers.CurrentRow?.DataBoundItem is not Customer customer)
        {
            return;
        }

        selectedCustomerId = customer.CustomerId;

        txtFirstName.Text = customer.FirstName;
        txtMiddleName.Text = customer.MiddleName ?? string.Empty;
        txtLastName.Text = customer.LastName;
        txtPhone.Text = customer.Phone ?? string.Empty;
        txtEmail.Text = customer.Email ?? string.Empty;
        txtAddress.Text = customer.Address ?? string.Empty;
    }

    // =========================
    // CLEAR
    // =========================

    private void BtnClear_Click(object? sender, EventArgs e)
    {
        ClearFields();
    }

    private void ClearFields()
    {
        selectedCustomerId = null;

        txtFirstName.Clear();
        txtMiddleName.Clear();
        txtLastName.Clear();
        txtPhone.Clear();
        txtEmail.Clear();
        txtAddress.Clear();

        dgvCustomers.ClearSelection();
    }

    // =========================
    // VALIDATION
    // =========================

    private bool ValidateCustomerInput()
    {
        if (string.IsNullOrWhiteSpace(txtFirstName.Text))
        {
            MessageBox.Show(
                "First name is required.",
                "Validation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            txtFirstName.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtLastName.Text))
        {
            MessageBox.Show(
                "Last name is required.",
                "Validation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            txtLastName.Focus();
            return false;
        }

        return true;
    }

    private static string? GetNullableText(TextBox textBox)
    {
        var value = textBox.Text.Trim();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }
}