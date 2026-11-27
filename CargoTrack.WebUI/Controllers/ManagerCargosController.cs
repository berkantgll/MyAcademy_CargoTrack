
using CargoTrack.Business.Abstract;
using CargoTrack.DTO.CargoDtos;
using CargoTrack.DTO.EmployeeDtos;
using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

[Authorize(Roles = "Manager")]
public class ManagerCargosController : Controller
{
    private readonly ICargoService _cargoService;
    private readonly IEmployeeService _employeeService;
    private readonly UserManager<AppUser> _userManager;

    public ManagerCargosController(
        ICargoService cargoService,
        IEmployeeService employeeService,
        UserManager<AppUser> userManager)
    {
        _cargoService = cargoService;
        _employeeService = employeeService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user?.BranchId == null)
            return Forbid();

        var cargos = await _cargoService
            .GetByBranchIdAsync(user.BranchId.Value);

        return View(cargos);
    }

    [HttpGet]
    public async Task<IActionResult> Status(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user?.BranchId == null)
            return Forbid();

        var cargo = await _cargoService
            .GetManagerCargoAsync(user.BranchId.Value, id);

        if (cargo == null)
            return NotFound();

        var employees = await _employeeService.GetAllAsync();

        ViewBag.TrackingNumber = cargo.TrackingNumber;
        ViewBag.CurrentStatus = cargo.Status;

        ViewBag.Employees = employees
            .Where(x =>
                x.IsActive &&
                x.BranchId == user.BranchId.Value)
            .ToList();

        return View(new ChangeCargoStatusDto
        {
            CargoId = id
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Status(ChangeCargoStatusDto dto)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user?.BranchId == null)
            return Forbid();

        if (dto.CargoId == Guid.Empty)
            return BadRequest();

        var success = await _cargoService
            .ChangeCargoStatusForBranchAsync(
                user.BranchId.Value, dto);

        if (!success)
        {
            TempData["Error"] =
                "Bu kargo için yetkiniz yok veya durum geçişi geçersiz.";
        }
        else
        {
            TempData["Success"] =
                "Kargo durumu başarıyla güncellendi.";
        }

        return RedirectToAction(nameof(Index));
    }
}
