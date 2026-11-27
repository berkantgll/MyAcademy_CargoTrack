
using CargoTrack.DataAccess.Context;
using CargoTrack.DTO.AuthDtos;
using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.WebUI.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly CargoTrackDbContext _context;

    public AccountController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        CargoTrackDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        string email = dto.Email.Trim();
        string phone = dto.PhoneNumber.Trim();

        // Kullanıcı ve müşteri kayıtları birlikte oluşturulsun
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var customer = new Customer
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            PhoneNumber = phone,
            Email = email,
            CreatedDate = DateTime.UtcNow
        };

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            PhoneNumber = phone,
            CustomerId = customer.Id,
            Customer = customer
        };

        var result = await _userManager
            .CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            await transaction.RollbackAsync();

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(dto);
        }

        // Herkese açık kayıt yalnızca User rolü verir
        var roleResult = await _userManager
            .AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
        {
            await transaction.RollbackAsync();

            ModelState.AddModelError(
                "", "Kullanıcı rolü atanamadı.");

            return View(dto);
        }

        await transaction.CommitAsync();

        await _signInManager.SignInAsync(
            user, isPersistent: false);

        return RedirectToAction("Index", "Dashboard");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _signInManager.PasswordSignInAsync(
            dto.Email.Trim(),
            dto.Password,
            dto.RememberMe,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(
                "", "Giriş başarısız. Bilgilerinizi kontrol edin.");

            return View(dto);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login");
    }
}
