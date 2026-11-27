using CargoTrack.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.ViewComponents;

public class AdminDashboardViewComponent : ViewComponent
{
    private readonly IAdminReportService _reportService;

    public AdminDashboardViewComponent(
        IAdminReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!User.IsInRole("Admin"))
        {
            return Content(string.Empty);
        }

        var model = await _reportService.GetReportAsync();

        return View(model);
    }
}
