
using CargoTrack.Business.Abstract;
using CargoTrack.DTO.BranchDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminBranchesController : Controller
{
    private readonly IBranchService _branchService;

    public AdminBranchesController(IBranchService branchService)
    {
        _branchService = branchService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var branches = await _branchService.GetAllAsync();

        return View(branches);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BranchDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Şube bilgilerini kontrol edin.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            dto.Id = Guid.Empty;
            dto.IsActive = true;

            await _branchService.SaveAsync(dto);

            TempData["Success"] = "Şube başarıyla oluşturuldu.";
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
