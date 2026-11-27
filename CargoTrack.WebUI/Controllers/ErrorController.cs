using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers;

[AllowAnonymous]
public class ErrorController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        Response.StatusCode = 500;

        return View();
    }
}
