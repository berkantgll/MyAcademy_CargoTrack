
using CargoTrack.Business.Abstract;
using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

[Authorize(Roles = "Admin,Manager,User")]
public class DashboardController : Controller
{
    private readonly ICargoService _cargoService;
    private readonly UserManager<AppUser> _userManager;

    public DashboardController(
        ICargoService cargoService,
        UserManager<AppUser> userManager)
    {
        _cargoService = cargoService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        // Admin tüm kargoları görebilir
        if (User.IsInRole("Admin"))
        {
            ViewData["PageTitle"] = "Admin - Tüm Kargolar";

            var cargos = await _cargoService.GetAllCargoAsync();

            return View(cargos);
        }

        // Manager yalnızca kendi şubesinin kargolarını görür
        if (User.IsInRole("Manager"))
        {
            if (!user.BranchId.HasValue)
                return Forbid();

            ViewData["PageTitle"] = "Manager - Şube Kargoları";

            var cargos = await _cargoService
                .GetByBranchIdAsync(user.BranchId.Value);

            return View(cargos);
        }

        // User yalnızca kendi gönderdiği veya aldığı kargoları görür
        if (User.IsInRole("User"))
        {
            if (!user.CustomerId.HasValue)
                return Forbid();

            ViewData["PageTitle"] = "Kargolarım";

            var cargos = await _cargoService
                .GetByCustomerIdAsync(user.CustomerId.Value);

            return View(cargos);
        }

        return Forbid();
    }
}
