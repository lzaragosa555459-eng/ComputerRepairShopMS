using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDbContextFactory<TenantDbContext> _dbFactory;

    public DevicesController(
        IDbContextFactory<TenantDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }


    // ==========================================
    // GET ALL DEVICES
    // GET: api/devices
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var devices = await db.Devices
            .AsNoTracking()
            .ToListAsync();

        return Ok(devices);
    }


    // ==========================================
    // GET DEVICE BY ID
    // GET: api/devices/{id}
    // ==========================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var device = await db.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.DeviceId == id);

        if (device == null)
        {
            return NotFound();
        }

        return Ok(device);
    }


    // ==========================================
    // GET DEVICES BY CUSTOMER
    // GET: api/devices/customer/{customerId}
    // ==========================================

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomer(
        int customerId)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var devices = await db.Devices
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .ToListAsync();

        return Ok(devices);
    }


    // ==========================================
    // CREATE DEVICE
    // POST: api/devices
    // ==========================================

    [HttpPost]
    public async Task<IActionResult> Create(Device device)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var customerExists =
            await db.Customers.AnyAsync(
                x => x.CustomerId == device.CustomerId);

        if (!customerExists)
        {
            return BadRequest(
                "The specified customer does not exist.");
        }

        db.Devices.Add(device);

        await db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = device.DeviceId },
            device);
    }


    // ==========================================
    // UPDATE DEVICE
    // PUT: api/devices/{id}
    // ==========================================

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        Device updatedDevice)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var device =
            await db.Devices.FindAsync(id);

        if (device == null)
        {
            return NotFound();
        }

        var customerExists =
            await db.Customers.AnyAsync(
                x => x.CustomerId ==
                     updatedDevice.CustomerId);

        if (!customerExists)
        {
            return BadRequest(
                "The specified customer does not exist.");
        }

        device.CustomerId =
            updatedDevice.CustomerId;

        device.DeviceType =
            updatedDevice.DeviceType;

        device.Brand =
            updatedDevice.Brand;

        device.Model =
            updatedDevice.Model;

        device.SerialNumber =
            updatedDevice.SerialNumber;

        device.DeviceCondition =
            updatedDevice.DeviceCondition;

        await db.SaveChangesAsync();

        return NoContent();
    }


    // ==========================================
    // DELETE DEVICE
    // DELETE: api/devices/{id}
    // ==========================================

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var device =
            await db.Devices.FindAsync(id);

        if (device == null)
        {
            return NotFound();
        }

        db.Devices.Remove(device);

        await db.SaveChangesAsync();

        return NoContent();
    }
}