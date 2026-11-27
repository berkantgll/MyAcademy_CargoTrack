
using CargoTrack.Business.Abstract;
using CargoTrack.DTO.CargoPriceDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminCargoPricesController : Controller
{
    private readonly ICargoPriceService _priceService;

    public AdminCargoPricesController(
        ICargoPriceService priceService)
    {
        _priceService = priceService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var prices = await _priceService.GetAllAsync();

        return View(prices);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CargoPriceDto dto)
    {
        dto.Id = Guid.Empty;
        dto.IsActive = true;

        try
        {
            await _priceService.SaveAsync(dto);

            TempData["Success"] =
                "Fiyat tarifesi başarıyla oluşturuldu.";
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
