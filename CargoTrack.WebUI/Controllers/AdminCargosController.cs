using CargoTrack.Business.Abstract;
using CargoTrack.DTO.CargoDtos;
using CargoTrack.Entity.Enums;
using CargoTrack.WebUI.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoTrack.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminCargosController : Controller
{
    private readonly ICargoService _cargoService;
    private readonly ICustomerService _customerService;
    private readonly IBranchService _branchService;
    private readonly IAddressService _addressService;
    private readonly IEmployeeService _employeeService;

    public AdminCargosController(
        ICargoService cargoService,
        ICustomerService customerService,
        IBranchService branchService,
        IAddressService addressService,
        IEmployeeService employeeService)
    {
        _cargoService = cargoService;
        _customerService = customerService;
        _branchService = branchService;
        _addressService = addressService;
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new AdminCargoCreateVm();

        await LoadSelectionsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminCargoCreateVm model)
    {
        // Formdan seçilen ek hizmetleri birleştir
        model.Cargo.ExtraServices = ExtraServiceType.None;

        foreach (var service in model.SelectedExtraServices)
        {
            model.Cargo.ExtraServices |= service;
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _cargoService.CreateCargoAsync(model.Cargo);

                TempData["Success"] = "Kargo başarıyla oluşturuldu.";

                return RedirectToAction("Index", "Dashboard");
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(
                        "", error.ErrorMessage);
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        await LoadSelectionsAsync(model);

        return View(model);
    }

    // Formun açılır listelerini doldur
    private async Task LoadSelectionsAsync(AdminCargoCreateVm model)
    {
        var customers = await _customerService.GetAllAsync();
        var branches = await _branchService.GetAllAsync();
        var addresses = await _addressService.GetAllAsync();

        model.Customers = customers
            .OrderBy(x => x.FirstName)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.FirstName} {x.LastName}"
            })
            .ToList();

        model.Branches = branches
            .Where(x => x.IsActive)
            .OrderBy(x => x.BranchName)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.BranchName} ({x.BranchCity})"
            })
            .ToList();

        model.Addresses = addresses
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.City} / {x.District} - {x.FullAddress}"
            })
            .ToList();
    }



    [HttpGet]
    public async Task<IActionResult> Status(Guid id)
    {
        var cargo = await _cargoService.GetCargoAsync(id);

        if (cargo == null)
            return NotFound();

        ViewBag.TrackingNumber = cargo.TrackingNumber;
        ViewBag.CurrentStatus = cargo.Status.ToString();
        ViewBag.Employees = await _employeeService.GetAllAsync();

        return View(new ChangeCargoStatusDto
        {
            CargoId = cargo.Id
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Status(ChangeCargoStatusDto dto)
    {
        if (dto.CargoId == Guid.Empty)
            return BadRequest();

        var success = await _cargoService.ChangeCargoStatusAsync(dto);

        if (success)
        {
            TempData["Success"] = "Kargo durumu güncellendi.";

            return RedirectToAction("Index", "Dashboard");
        }

        var cargo = await _cargoService.GetCargoAsync(dto.CargoId);

        if (cargo == null)
            return NotFound();

        ViewBag.TrackingNumber = cargo.TrackingNumber;
        ViewBag.CurrentStatus = cargo.Status.ToString();
        ViewBag.Employees = await _employeeService.GetAllAsync();

        ModelState.AddModelError(
            "",
            "Durum değiştirilemedi. Geçişi veya teslimat kodunu kontrol edin.");

        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var cargo = await _cargoService.GetAdminCargoDetailsAsync(id);

        if (cargo == null)
            return NotFound();

        return View(cargo);
    }


    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var dto = await _cargoService.GetCargoForEditAsync(id);

        if (dto == null)
            return NotFound();

        var model = new AdminCargoEditVm
        {
            Cargo = dto,
            SelectedExtraServices = Enum
                .GetValues<ExtraServiceType>()
                .Where(x =>
                    x != ExtraServiceType.None &&
                    (dto.ExtraServices & x) == x)
                .ToList()
        };

        await LoadEditOptionsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AdminCargoEditVm model)
    {
        if (model.Cargo.CargoId == Guid.Empty)
            return BadRequest();

        model.Cargo.ExtraServices = ExtraServiceType.None;

        foreach (var service in model.SelectedExtraServices)
        {
            model.Cargo.ExtraServices |= service;
        }

        if (ModelState.IsValid)
        {
            try
            {
                var success = await _cargoService
                    .UpdateCargoAsync(model.Cargo);

                if (success)
                {
                    TempData["Success"] = "Kargo güncellendi.";

                    return RedirectToAction("Index", "Dashboard");
                }

                ModelState.AddModelError(
                    "",
                    "Yalnızca Created durumundaki kargolar güncellenebilir.");
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        await LoadEditOptionsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var success = await _cargoService.DeleteCargoAsync(id);

        if (success)
        {
            TempData["Success"] = "Kargo silindi.";
        }
        else
        {
            TempData["Error"] =
                "Yalnızca Created durumundaki kargolar silinebilir.";
        }

        return RedirectToAction("Index", "Dashboard");
    }

    private async Task LoadEditOptionsAsync(AdminCargoEditVm model)
    {
        var branches = await _branchService.GetAllAsync();
        var addresses = await _addressService.GetAllAsync();

        model.Branches = branches
            .Where(x => x.IsActive)
            .OrderBy(x => x.BranchName)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.BranchName} ({x.BranchCity})"
            })
            .ToList();

        model.Addresses = addresses
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.City} / {x.District} - {x.FullAddress}"
            })
            .ToList();
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? search,
        CargoStatus? status,
        Guid? branchId,
        int page = 1)
    {
        page = Math.Max(page, 1);

        var result = await _cargoService.SearchCargosAsync(
            search, status, branchId, page, 10);

        var branches = await _branchService.GetAllAsync();

        var model = new AdminCargoSearchVm
        {
            Search = search,
            Status = status,
            BranchId = branchId,
            Page = page,
            PageSize = 10,
            TotalCount = result.TotalCount,
            Items = result.Items,

            Branches = branches
                .OrderBy(x => x.BranchName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BranchName,
                    Selected = branchId == x.Id
                })
                .ToList()
        };

        return View(model);
    }

}
