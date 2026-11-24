using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity;

namespace CargoTrack.WebUI.Seed;

public static class RoleSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<AppRole>>();

        string[] roles = { "Admin", "Manager", "User" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(
                    new AppRole { Name = roleName });

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Rol oluşturulamadı: {roleName}");
                }
            }
        }
    }
}
