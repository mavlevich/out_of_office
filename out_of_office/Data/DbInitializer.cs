using Microsoft.AspNetCore.Identity;
using out_of_office.Models;
using System.Threading.Tasks;


namespace out_of_office.Data;

public class DbInitializer
{
    public static async Task Initialize(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { RoleModel.Employee, RoleModel.HRManager, RoleModel.ProjectManager, RoleModel.Administrator };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                // Create the roles and seed them to the database
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create an admin user for demonstration
        var adminEmail = "admin@example.com";
        var adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail
        };

        string adminPassword = "Admin123!";
        var user = await userManager.FindByEmailAsync(adminEmail);

        if (user == null)
        {
            var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);
            if (createAdminUser.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, RoleModel.Administrator);
            }
        }
    }
}