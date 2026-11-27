
using CargoTrack.Business.Abstract;
using CargoTrack.DTO.CustomerDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminCustomersController : Controller
{
    private readonly ICustomerService _customerService;

    public AdminCustomersController(
        ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var customers = await _customerService.GetAllAsync();

        return View(customers);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerDto dto)
    {
        try
        {
            dto.Id = Guid.Empty;

            await _customerService.SaveAsync(dto);

            TempData["Success"] = "Müşteri başarıyla oluşturuldu.";
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
