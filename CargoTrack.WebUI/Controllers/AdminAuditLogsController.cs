
using CargoTrack.Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminAuditLogsController : Controller
{
    private readonly IAuditLogReadService _auditLogService;

    public AdminAuditLogsController(
        IAuditLogReadService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        var model = await _auditLogService
            .GetPageAsync(page, 20);

        return View(model);
    }
}
