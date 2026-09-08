using ComputerRepairSystem.company.Data;
using ComputerRepairSystem.company.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerRepairSystem.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IDbContextFactory<TenantDbContext> _dbFactory;

    public InventoryController(
        IDbContextFactory<TenantDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }


    // ==========================================
    // GET ALL INVENTORY
    // GET: api/inventory
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var inventory =
            await db.Inventories
                .AsNoTracking()
                .OrderBy(x => x.InventoryId)
                .Select(x => new
                {
                    x.InventoryId,
                    x.BranchId,
                    x.ItemId,
                    x.QuantityOnHand,

                    Item = x.Item == null
                        ? null
                        : new
                        {
                            x.Item.ItemId,
                            x.Item.ItemName,
                            x.Item.Category,
                            x.Item.Brand,
                            x.Item.Model,
                            x.Item.Unit,
                            x.Item.UnitPrice
                        }
                })
                .ToListAsync();

        return Ok(inventory);
    }


    // ==========================================
    // GET INVENTORY BY ID
    // GET: api/inventory/{id}
    // ==========================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var inventory =
            await db.Inventories
                .AsNoTracking()
                .Where(x => x.InventoryId == id)
                .Select(x => new
                {
                    x.InventoryId,
                    x.BranchId,
                    x.ItemId,
                    x.QuantityOnHand,

                    Item = x.Item == null
                        ? null
                        : new
                        {
                            x.Item.ItemId,
                            x.Item.ItemName,
                            x.Item.Category,
                            x.Item.Brand,
                            x.Item.Model,
                            x.Item.Unit,
                            x.Item.UnitPrice
                        }
                })
                .FirstOrDefaultAsync();

        if (inventory == null)
        {
            return NotFound();
        }

        return Ok(inventory);
    }


    // ==========================================
    // GET INVENTORY BY BRANCH
    // GET: api/inventory/branch/{branchId}
    // ==========================================

    [HttpGet("branch/{branchId:int}")]
    public async Task<IActionResult> GetByBranch(
        int branchId)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var inventory =
            await db.Inventories
                .AsNoTracking()
                .Include(x => x.Item)
                .Include(x => x.Branch)
                .Where(x => x.BranchId == branchId)
                .OrderBy(x => x.InventoryId)
                .ToListAsync();

        return Ok(inventory);
    }


    // ==========================================
    // CREATE INVENTORY
    // POST: api/inventory
    // ==========================================

    [HttpPost]
    public async Task<IActionResult> Create(
        Inventory inventory)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        if (inventory.BranchId.HasValue)
        {
            var branchExists =
                await db.Branches.AnyAsync(
                    x => x.BranchId == inventory.BranchId.Value);

            if (!branchExists)
            {
                return BadRequest(
                    "The specified branch does not exist.");
            }
        }

        var itemExists =
            await db.InventoryItems.AnyAsync(
                x => x.ItemId == inventory.ItemId);

        if (!itemExists)
        {
            return BadRequest(
                "The specified inventory item does not exist.");
        }

        var duplicate =
            await db.Inventories.AnyAsync(
                x =>
                    x.BranchId == inventory.BranchId &&
                    x.ItemId == inventory.ItemId);

        if (duplicate)
        {
            return Conflict(
                "Inventory for this item already exists in the specified branch.");
        }

        db.Inventories.Add(inventory);

        await db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = inventory.InventoryId
            },
            inventory);
    }


    // ==========================================
    // UPDATE INVENTORY
    // PUT: api/inventory/{id}
    // ==========================================

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        Inventory updatedInventory)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var inventory =
            await db.Inventories
                .FirstOrDefaultAsync(
                    x => x.InventoryId == id);

        if (inventory == null)
        {
            return NotFound();
        }

        if (updatedInventory.BranchId.HasValue)
        {
            var branchExists =
                await db.Branches.AnyAsync(
                    x => x.BranchId == updatedInventory.BranchId.Value);

            if (!branchExists)
            {
                return BadRequest(
                    "The specified branch does not exist.");
            }
        }

        var itemExists =
            await db.InventoryItems.AnyAsync(
                x => x.ItemId == updatedInventory.ItemId);

        if (!itemExists)
        {
            return BadRequest(
                "The specified inventory item does not exist.");
        }

        var duplicate =
            await db.Inventories.AnyAsync(
                x =>
                    x.InventoryId != id &&
                    x.BranchId == updatedInventory.BranchId &&
                    x.ItemId == updatedInventory.ItemId);

        if (duplicate)
        {
            return Conflict(
                "Inventory for this item already exists in the specified branch.");
        }

        inventory.BranchId =
            updatedInventory.BranchId;

        inventory.ItemId =
            updatedInventory.ItemId;

        inventory.QuantityOnHand =
            updatedInventory.QuantityOnHand;

        await db.SaveChangesAsync();

        return NoContent();
    }


    // ==========================================
    // DELETE INVENTORY
    // DELETE: api/inventory/{id}
    // ==========================================

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var inventory =
            await db.Inventories
                .FirstOrDefaultAsync(
                    x => x.InventoryId == id);

        if (inventory == null)
        {
            return NotFound();
        }

        db.Inventories.Remove(inventory);

        await db.SaveChangesAsync();

        return NoContent();
    }
}