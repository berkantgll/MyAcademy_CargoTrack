
using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Context;
using CargoTrack.Entity.Entities;
using CargoTrack.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminUsersController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IBranchService _branchService;
    private readonly CargoTrackDbContext _context;

    public AdminUsersController(
        UserManager<AppUser> userManager,
        IBranchService branchService,
        CargoTrackDbContext context)
    {
        _userManager = userManager;
        _branchService = branchService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .OrderBy(x => x.Email)
            .ToListAsync();

        var branches = await _branchService.GetAllAsync();

        var model = new AdminUsersVm
        {
            Branches = branches
                .Where(x => x.IsActive)
                .ToList()
        };

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            model.Users.Add(new UserRoleRowVm
            {
                Id = user.Id,
                Email = user.Email ?? "",
                BranchId = user.BranchId,
                CustomerId = user.CustomerId,
                Role = roles.Contains("Admin") ? "Admin"
                    : roles.Contains("Manager") ? "Manager"
                    : roles.Contains("User") ? "User"
                    : "Rol yok"
            });
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(
        Guid userId,
        string role,
        Guid? branchId)
    {
        // Panelden yalnızca User veya Manager atanabilir
        if (role != "User" && role != "Manager")
            return BadRequest();

        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        // Admin hesaplarının rolü bu ekrandan değiştirilemez
        if (roles.Contains("Admin"))
            return Forbid();

        if (role == "Manager")
        {
            if (!branchId.HasValue)
                return BadRequest("Şube seçilmelidir.");

            var branch = await _branchService
                .GetByIdAsync(branchId.Value);

            if (branch == null || !branch.IsActive)
                return BadRequest("Aktif şube bulunamadı.");
        }

        if (role == "User" && !user.CustomerId.HasValue)
        {
            return BadRequest(
                "User rolü için müşteri bağlantısı gereklidir.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var oldRoles = roles
            .Where(x => x == "User" || x == "Manager")
            .ToArray();

        if (oldRoles.Length > 0)
        {
            var removeResult = await _userManager
                .RemoveFromRolesAsync(user, oldRoles);

            if (!removeResult.Succeeded)
                throw new InvalidOperationException(
                    "Önceki roller kaldırılamadı.");
        }

        user.BranchId = role == "Manager" ? branchId : null;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
            throw new InvalidOperationException(
                "Kullanıcı güncellenemedi.");

        var addResult = await _userManager
            .AddToRoleAsync(user, role);

        if (!addResult.Succeeded)
            throw new InvalidOperationException(
                "Yeni rol atanamadı.");

        await transaction.CommitAsync();

        TempData["Success"] = "Kullanıcının rolü güncellendi.";

        return RedirectToAction(nameof(Index));
    }
}
