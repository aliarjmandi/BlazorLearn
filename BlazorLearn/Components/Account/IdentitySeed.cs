using Microsoft.AspNetCore.Identity;

namespace BlazorLearn.Components.Account;

public static class IdentitySeed
{
    private static readonly string[] DefaultRoles = { "Admin", "Customer", "Seller" };

    public static async Task EnsureRolesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleMgr.RoleExistsAsync("Admin"))
        {

            foreach (var r in DefaultRoles)
                if (!await roleMgr.RoleExistsAsync(r))
                    await roleMgr.CreateAsync(new IdentityRole(r));
        }
    }


    public static async Task EnsureUserInRoleAsync(IServiceProvider services, string email, string role)
    {
        /*
        var user = await userManager.FindByEmailAsync("aliarjmandi@yahoo.com");
        if (user == null)
        {
            user = new IdentityUser { UserName = "aliarjmandi@yahoo.com", Email = "aliarjmandi@yahoo.com" };
            await userManager.CreateAsync(user, "YourStrongPassword123!");
            await userManager.AddToRoleAsync(user, "Admin");
        }
        

        using var scope = services.CreateScope();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleMgr.RoleExistsAsync(role))
            throw new InvalidOperationException($"Role '{role}' does not exist. Seed roles first.");

        var userRole = await userMgr.FindByEmailAsync(email);
        if (userRole is null) throw new InvalidOperationException($"User '{email}' not found.");

        if (!await userMgr.IsInRoleAsync(userRole, role))
            await userMgr.AddToRoleAsync(userRole, role);
        */
    }

}
