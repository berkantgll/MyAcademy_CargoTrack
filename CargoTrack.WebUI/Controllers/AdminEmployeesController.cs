
using CargoTrack.Business.Abstract;
using CargoTrack.DTO.EmployeeDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminEmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IBranchService _branchService;

    public AdminEmployeesController(
        IEmployeeService employeeService,
        IBranchService branchService)
    {
        _employeeService = employeeService;
        _branchService = branchService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.Branches = (await _branchService.GetAllAsync())
            .Where(x => x.IsActive)
            .ToList();

        return View(await _employeeService.GetAllAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeDto dto)
    {
        try
        {
            dto.Id = Guid.Empty;
            dto.IsActive = true;

            await _employeeService.SaveAsync(dto);

            TempData["Success"] = "Çalışan oluşturuldu.";
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
