
using CargoTrack.Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class CargoController : Controller
{
    private readonly ICargoService _cargoService;

    public CargoController(ICargoService cargoService)
    {
        _cargoService = cargoService;
    }

    public async Task<IActionResult> Index()
    {
        var cargos = await _cargoService.GetAllCargoAsync();

        return View(cargos);
    }
}
