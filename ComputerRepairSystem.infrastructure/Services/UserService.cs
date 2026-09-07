using ComputerRepairSystem.domain.Entities;
using ComputerRepairSystem.infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace ComputerRepairSystem.infrastructure.Services;

public class UserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }


    // ==========================================
    // GET ALL USERS
    // ==========================================

    public List<ApplicationUser> GetAll()
    {
        return _userManager.Users.ToList();
    }


    // ==========================================
    // GET USER BY ID
    // ==========================================

    public async Task<ApplicationUser?> GetByIdAsync(
        string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }


    // ==========================================
    // CREATE USER
    // ==========================================

    public async Task<IdentityResult> CreateAsync(
        ApplicationUser user,
        string password)
    {
        return await _userManager.CreateAsync(
            user,
            password);
    }


    // ==========================================
    // UPDATE USER
    // ==========================================

    public async Task<IdentityResult> UpdateAsync(
        ApplicationUser user)
    {
        return await _userManager.UpdateAsync(user);
    }


    // ==========================================
    // DELETE USER
    // ==========================================

    public async Task<IdentityResult> DeleteAsync(
        ApplicationUser user)
    {
        return await _userManager.DeleteAsync(user);
    }
}