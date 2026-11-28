
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


    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var branch = await _branchService.GetByIdAsync(id);

        if (branch == null)
            return NotFound();

        return View(branch);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BranchDto dto)
    {
        if (dto.Id == Guid.Empty)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            var branch = await _branchService.GetByIdAsync(dto.Id);

            if (branch == null)
                return NotFound();

            await _branchService.SaveAsync(dto);

            TempData["Success"] =
                "Şube bilgileri başarıyla güncellendi.";

            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
        }

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            bool deleted = await _branchService.DeleteAsync(id);

            TempData[deleted ? "Success" : "Error"] =
                deleted
                    ? "Şube başarıyla silindi."
                    : "Şube bulunamadı.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

}
