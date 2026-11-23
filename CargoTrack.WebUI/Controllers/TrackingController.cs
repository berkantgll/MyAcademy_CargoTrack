
using CargoTrack.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

public class TrackingController : Controller
{
    private readonly ICargoService _cargoService;

    public TrackingController(ICargoService cargoService)
    {
        _cargoService = cargoService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Result(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
        {
            TempData["Error"] = "Takip numarası giriniz.";
            return RedirectToAction("Index");
        }

        trackingNumber = trackingNumber.Trim().ToUpperInvariant();

        var cargo = await _cargoService.TrackCargoAsync(trackingNumber);

        if (cargo == null)
        {
            TempData["Error"] = "Kargo bulunamadı.";
            return RedirectToAction("Index");
        }

        return View(cargo);
    }
}
