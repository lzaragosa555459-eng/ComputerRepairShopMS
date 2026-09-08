using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceRequestsController : ControllerBase
{
    private readonly IDbContextFactory<TenantDbContext> _dbFactory;

    public ServiceRequestsController(
        IDbContextFactory<TenantDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }


    // ==========================================
    // GET ALL SERVICE REQUESTS
    // GET: api/servicerequests
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var requests = await db.ServiceRequests
            .AsNoTracking()
            .ToListAsync();

        return Ok(requests);
    }


    // ==========================================
    // GET SERVICE REQUEST BY ID
    // GET: api/servicerequests/{id}
    // ==========================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var request = await db.ServiceRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ServiceRequestId == id);

        if (request == null)
        {
            return NotFound();
        }

        return Ok(request);
    }


    // ==========================================
    // GET REQUESTS BY DEVICE
    // GET: api/servicerequests/device/{deviceId}
    // ==========================================

    [HttpGet("device/{deviceId:int}")]
    public async Task<IActionResult> GetByDevice(
        int deviceId)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var requests = await db.ServiceRequests
            .AsNoTracking()
            .Where(x => x.DeviceId == deviceId)
            .ToListAsync();

        return Ok(requests);
    }


    // ==========================================
    // CREATE SERVICE REQUEST
    // POST: api/servicerequests
    // ==========================================

    [HttpPost]
    public async Task<IActionResult> Create(
        ServiceRequest request)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var deviceExists =
            await db.Devices.AnyAsync(
                x => x.DeviceId == request.DeviceId);

        if (!deviceExists)
        {
            return BadRequest(
                "The specified device does not exist.");
        }

        request.RequestDate =
            DateTime.UtcNow;

        db.ServiceRequests.Add(request);

        await db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = request.ServiceRequestId },
            request);
    }


    // ==========================================
    // UPDATE SERVICE REQUEST
    // PUT: api/servicerequests/{id}
    // ==========================================

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        ServiceRequest updatedRequest)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var request =
            await db.ServiceRequests.FindAsync(id);

        if (request == null)
        {
            return NotFound();
        }

        var deviceExists =
            await db.Devices.AnyAsync(
                x => x.DeviceId ==
                     updatedRequest.DeviceId);

        if (!deviceExists)
        {
            return BadRequest(
                "The specified device does not exist.");
        }

        request.DeviceId =
            updatedRequest.DeviceId;

        request.Description =
            updatedRequest.Description;

        request.Status =
            updatedRequest.Status;

        request.Priority =
            updatedRequest.Priority;

        await db.SaveChangesAsync();

        return NoContent();
    }


    // ==========================================
    // DELETE SERVICE REQUEST
    // DELETE: api/servicerequests/{id}
    // ==========================================

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var request =
            await db.ServiceRequests.FindAsync(id);

        if (request == null)
        {
            return NotFound();
        }

        db.ServiceRequests.Remove(request);

        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("all")]
    public async Task<IActionResult> DeleteAll()
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        db.ServiceRequests.RemoveRange(
            db.ServiceRequests);

        db.Devices.RemoveRange(
            db.Devices);

        db.Customers.RemoveRange(
            db.Customers);

        await db.SaveChangesAsync();

        return Ok("All customers, devices, and service requests have been deleted.");
    }
}