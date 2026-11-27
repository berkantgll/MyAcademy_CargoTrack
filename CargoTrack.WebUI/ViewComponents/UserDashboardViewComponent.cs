using CargoTrack.Business.Abstract;
using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.ViewComponents;

public class UserDashboardViewComponent : ViewComponent
{
    private readonly IUserDashboardService _dashboardService;
    private readonly UserManager<AppUser> _userManager;

    public UserDashboardViewComponent(
        IUserDashboardService dashboardService,
        UserManager<AppUser> userManager)
    {
        _dashboardService = dashboardService;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!UserClaimsPrincipal.IsInRole("User"))
            return Content(string.Empty);

        var user = await _userManager
            .GetUserAsync(UserClaimsPrincipal);

        if (user?.CustomerId == null)
        {
            return Content(
                "Hesabınıza bağlı müşteri kaydı bulunamadı.");
        }

        var model = await _dashboardService
            .GetAsync(user.CustomerId.Value);

        return View(model);
    }
}
