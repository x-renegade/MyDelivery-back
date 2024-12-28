using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Api.Extensions;

public static class UserRoleExtension
{
    public static async Task InitializeRolesAsync(this WebApplication app)

    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        if (!await roleManager.RoleExistsAsync("User"))
        {
            var role = new IdentityRole("User");
            await roleManager.CreateAsync(role);
        }

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            var role = new IdentityRole("Admin");
            await roleManager.CreateAsync(role);
        }

        
        // var user = await userManager.FindByEmailAsync("admin@example.com");
        // if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
        // {
        //     await userManager.AddToRoleAsync(user, "Admin");
        // }
    }
}
