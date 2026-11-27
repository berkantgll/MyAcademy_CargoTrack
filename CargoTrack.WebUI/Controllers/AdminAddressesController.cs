
using CargoTrack.Business.Abstract;
using CargoTrack.DTO.AddressDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminAddressesController : Controller
{
    private readonly IAddressService _addressService;

    public AdminAddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var addresses = await _addressService.GetAllAsync();
        return View(addresses);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddressDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Adres bilgilerini kontrol edin.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            dto.Id = Guid.Empty;

            await _addressService.SaveAsync(dto);

            TempData["Success"] = "Adres başarıyla eklendi.";
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
