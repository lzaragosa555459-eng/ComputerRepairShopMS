using ComputerRepairSystem.api.DTOs;
using ComputerRepairSystem.domain.Entities;
using ComputerRepairSystem.infrastructure.Entities;
using ComputerRepairSystem.infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace ComputerRepairSystem.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(
        UserService userService)
    {
        _userService = userService;
    }


    // ==========================================
    // GET ALL USERS
    // GET: api/users
    // ==========================================

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();

        return Ok(users);
    }


    // ==========================================
    // GET USER BY ID
    // GET: api/users/{id}
    // ==========================================

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id)
    {
        var user =
            await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }


    // ==========================================
    // CREATE USER
    // POST: api/users
    // ==========================================

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateUserRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,

            Email = request.Email,

            CompanyId = request.CompanyId,

            IsActive = true
        };


        var result =
            await _userService.CreateAsync(
                user,
                request.Password);


        if (!result.Succeeded)
        {
            return BadRequest(
                result.Errors);
        }


        return CreatedAtAction(
            nameof(GetById),

            new { id = user.Id },

            user);
    }


    // ==========================================
    // UPDATE USER
    // PUT: api/users/{id}
    // ==========================================

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        ApplicationUser updatedUser)
    {
        var user =
            await _userService.GetByIdAsync(id);


        if (user == null)
        {
            return NotFound();
        }


        user.UserName =
            updatedUser.UserName;

        user.Email =
            updatedUser.Email;

        user.CompanyId =
            updatedUser.CompanyId;

        user.IsActive =
            updatedUser.IsActive;


        var result =
            await _userService.UpdateAsync(user);


        if (!result.Succeeded)
        {
            return BadRequest(
                result.Errors);
        }


        return NoContent();
    }


    // ==========================================
    // DELETE USER
    // DELETE: api/users/{id}
    // ==========================================

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        string id)
    {
        var user =
            await _userService.GetByIdAsync(id);


        if (user == null)
        {
            return NotFound();
        }


        var result =
            await _userService.DeleteAsync(user);


        if (!result.Succeeded)
        {
            return BadRequest(
                result.Errors);
        }


        return NoContent();
    }
}