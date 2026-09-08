using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillingController : ControllerBase
{
    private readonly IDbContextFactory<TenantDbContext> _dbFactory;

    public BillingController(
        IDbContextFactory<TenantDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    // ==========================================
    // GET COMPLETED REPAIRS FOR BILLING
    // GET: api/billing/completed
    // ==========================================

    [HttpGet("completed")]
    public async Task<IActionResult> GetCompletedRepairs()
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var repairs =
            await db.Repairs
                .AsNoTracking()
                .Where(r =>
                    r.Status == "Completed" &&
                    r.Invoice == null)
                .Select(r => new
                {
                    r.RepairId,
                    r.ServiceRequestId,
                    r.Diagnosis,
                    r.RepairDescription,
                    r.Status,

                    Customer =
                        new
                        {
                            r.ServiceRequest.Device.Customer.CustomerId,
                            r.ServiceRequest.Device.Customer.FirstName,
                            r.ServiceRequest.Device.Customer.LastName
                        },

                    Device =
                        new
                        {
                            r.ServiceRequest.Device.DeviceId,
                            r.ServiceRequest.Device.DeviceType,
                            r.ServiceRequest.Device.Brand,
                            r.ServiceRequest.Device.Model
                        },

                    RepairItems =
                        r.RepairItems
                            .Select(ri => new
                            {
                                ri.RepairItemId,
                                ri.ItemId,
                                ri.Quantity,
                                ri.UnitPrice,
                                ri.Discount,

                                ItemName =
                                    ri.Item.ItemName
                            })
                            .ToList()
                })
                .OrderByDescending(r => r.RepairId)
                .ToListAsync();

        return Ok(repairs);
    }


    // ==========================================
    // CREATE INVOICE
    // POST: api/billing/{repairId}/invoice
    // ==========================================

    [HttpPost("{repairId:int}/invoice")]
    public async Task<IActionResult> CreateInvoice(
        int repairId,
        [FromBody] CreateInvoiceRequest request)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var repair =
            await db.Repairs
                .Include(r => r.RepairItems)
                .FirstOrDefaultAsync(
                    r => r.RepairId == repairId);

        if (repair == null)
        {
            return NotFound("Repair not found.");
        }

        if (repair.Status != "Completed")
        {
            return BadRequest(
                "Only completed repairs can be billed.");
        }

        if (repair.Invoice != null)
        {
            return Conflict(
                "An invoice already exists for this repair.");
        }

        var subtotal =
            repair.RepairItems.Sum(
                x => (x.Quantity * x.UnitPrice)
                    - x.Discount);

        var discount =
            request.Discount < 0
                ? 0
                : request.Discount;

        var tax =
            request.Tax < 0
                ? 0
                : request.Tax;

        var total =
            subtotal - discount + tax;

        if (total < 0)
        {
            return BadRequest(
                "Total amount cannot be negative.");
        }

        var invoice =
            new Invoice
            {
                RepairId =
                    repairId,

                InvoiceNumber =
                    $"INV-{DateTime.UtcNow:yyyyMMddHHmmssfff}",

                Subtotal =
                    subtotal,

                Discount =
                    discount,

                Tax =
                    tax,

                TotalAmount =
                    total,

                Status =
                    "Unpaid"
            };

        db.Invoices.Add(invoice);

        await db.SaveChangesAsync();

        return Ok(new
        {
            invoice.InvoiceId,
            invoice.InvoiceNumber,
            invoice.RepairId,
            invoice.Subtotal,
            invoice.Discount,
            invoice.Tax,
            invoice.TotalAmount,
            invoice.Status
        });
    }


    // ==========================================
    // CREATE PAYMENT
    // POST: api/billing/invoice/{invoiceId}/payment
    // ==========================================

    [HttpPost("invoice/{invoiceId:int}/payment")]
    public async Task<IActionResult> CreatePayment(
        int invoiceId,
        [FromBody] CreatePaymentRequest request)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var invoice =
            await db.Invoices
                .FirstOrDefaultAsync(
                    x => x.InvoiceId == invoiceId);

        if (invoice == null)
        {
            return NotFound("Invoice not found.");
        }

        if (invoice.Status == "Paid")
        {
            return BadRequest(
                "This invoice has already been paid.");
        }

        if (request.Amount <= 0)
        {
            return BadRequest(
                "Payment amount must be greater than zero.");
        }

        if (request.Amount < invoice.TotalAmount)
        {
            return BadRequest(
                "Payment amount is less than the invoice total.");
        }

        var payment =
            new Payment
            {
                InvoiceId =
                    invoiceId,

                Amount =
                    request.Amount,

                PaymentMethod =
                    request.PaymentMethod,

                ReferenceNumber =
                    request.ReferenceNumber,

                Status =
                    "Completed"
            };

        invoice.Status = "Paid";

        db.Payments.Add(payment);

        await db.SaveChangesAsync();

        return Ok(new
        {
            payment.PaymentId,
            payment.InvoiceId,
            payment.Amount,
            payment.PaymentMethod,
            payment.ReferenceNumber,
            payment.Status,
            Change =
                request.Amount -
                invoice.TotalAmount
        });
    }
}


public class CreateInvoiceRequest
{
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
}


public class CreatePaymentRequest
{
    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; }
        = string.Empty;

    public string? ReferenceNumber { get; set; }
}