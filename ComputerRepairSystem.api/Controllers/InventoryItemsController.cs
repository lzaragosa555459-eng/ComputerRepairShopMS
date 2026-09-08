using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryItemsController : ControllerBase
{
    private readonly IDbContextFactory<TenantDbContext> _dbFactory;

    public InventoryItemsController(
        IDbContextFactory<TenantDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }


    // ==========================================
    // GET ALL ITEMS
    // GET: api/InventoryItems
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var items =
            await db.InventoryItems
                .AsNoTracking()
                .OrderBy(x => x.ItemName)
                .ToListAsync();

        return Ok(items);
    }


    // ==========================================
    // GET ITEM BY ID
    // GET: api/InventoryItems/{id}
    // ==========================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var item =
            await db.InventoryItems
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.ItemId == id);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }


    // ==========================================
    // CREATE ITEM
    // POST: api/InventoryItems
    // ==========================================

    [HttpPost]
    public async Task<IActionResult> Create(
        InventoryItem item)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        db.InventoryItems.Add(item);

        await db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = item.ItemId
            },
            item);
    }


    // ==========================================
    // UPDATE ITEM
    // PUT: api/InventoryItems/{id}
    // ==========================================

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        InventoryItem updatedItem)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var item =
            await db.InventoryItems
                .FirstOrDefaultAsync(
                    x => x.ItemId == id);

        if (item == null)
        {
            return NotFound();
        }

        item.ItemName =
            updatedItem.ItemName;

        item.Category =
            updatedItem.Category;

        item.Description =
            updatedItem.Description;

        item.Brand =
            updatedItem.Brand;

        item.Model =
            updatedItem.Model;

        item.Unit =
            updatedItem.Unit;

        item.UnitCost =
            updatedItem.UnitCost;

        item.UnitPrice =
            updatedItem.UnitPrice;

        item.ReorderLevel =
            updatedItem.ReorderLevel;

        await db.SaveChangesAsync();

        return NoContent();
    }


    // ==========================================
    // DELETE ITEM
    // DELETE: api/InventoryItems/{id}
    // ==========================================

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var item =
            await db.InventoryItems
                .FirstOrDefaultAsync(
                    x => x.ItemId == id);

        if (item == null)
        {
            return NotFound();
        }

        db.InventoryItems.Remove(item);

        await db.SaveChangesAsync();

        return NoContent();
    }
}