
using CargoTrack.Business.Abstract;
using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

[Authorize(Roles = "User")]
public class UserCargosController : Controller
{
    private readonly ICargoService _cargoService;
    private readonly UserManager<AppUser> _userManager;

    public UserCargosController(
        ICargoService cargoService,
        UserManager<AppUser> userManager)
    {
        _cargoService = cargoService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user?.CustomerId == null)
            return Forbid();

        var cargos = await _cargoService
            .GetByCustomerIdAsync(user.CustomerId.Value);

        return View(cargos);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user?.CustomerId == null)
            return Forbid();

        var cargo = await _cargoService
            .GetUserCargoDetailsAsync(
                user.CustomerId.Value, id);

        if (cargo == null)
            return NotFound();

        return View(cargo);
    }
}
