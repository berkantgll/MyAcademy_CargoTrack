using CargoTrack.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

public class CargoController : Controller
{
    private readonly ICargoService _cargoService;

    public CargoController(ICargoService cargoService)
    {
        _cargoService = cargoService;
    }

    public async Task<IActionResult> Index()
    {
        var cargo = await _cargoService.GetAllCargoAsync();
        return View(cargo);
    }
}