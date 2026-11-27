
using CargoTrack.Business.Abstract;
using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.ViewComponents;

public class ManagerDashboardViewComponent : ViewComponent
{
    private readonly IManagerDashboardService _dashboardService;
    private readonly UserManager<AppUser> _userManager;

    public ManagerDashboardViewComponent(
        IManagerDashboardService dashboardService,
        UserManager<AppUser> userManager)
    {
        _dashboardService = dashboardService;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!UserClaimsPrincipal.IsInRole("Manager"))
            return Content(string.Empty);

        var user = await _userManager
            .GetUserAsync(UserClaimsPrincipal);

        if (user?.BranchId == null)
            return Content("Hesabınıza şube atanmamış.");

        var model = await _dashboardService
            .GetAsync(user.BranchId.Value);

        return View(model);
    }
}
