
using CargoTrack.Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace CargoTrack.WebUI.Controllers;

[AllowAnonymous]
public class CargoQrController : Controller
{
    private readonly ICargoService _cargoService;

    public CargoQrController(ICargoService cargoService)
    {
        _cargoService = cargoService;
    }

    // Kargo için QR kod görselini oluştur.
    [HttpGet]
    public async Task<IActionResult> Image(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber) ||
            trackingNumber.Length > 50)
        {
            return BadRequest();
        }

        trackingNumber = trackingNumber.Trim().ToUpperInvariant();

        var cargo = await _cargoService
            .TrackCargoAsync(trackingNumber);

        if (cargo == null)
            return NotFound();

        // QR kodun açacağı herkese açık takip adresi.
        var url = Url.Action(
            nameof(Result),
            "CargoQr",
            new { trackingNumber },
            Request.Scheme);

        if (string.IsNullOrWhiteSpace(url))
            return StatusCode(500);

        // QR görselini PNG olarak üret.
        byte[] qrImage = PngByteQRCodeHelper.GetQRCode(
            url,
            QRCodeGenerator.ECCLevel.Q,
            8);

        return File(qrImage, "image/png");
    }

    // QR okutulduğunda açılan takip sayfası.
    [HttpGet]
    public async Task<IActionResult> Result(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber) ||
            trackingNumber.Length > 50)
        {
            return BadRequest();
        }

        var cargo = await _cargoService
            .TrackCargoAsync(trackingNumber);

        if (cargo == null)
            return NotFound();

        return View(cargo);
    }
}
