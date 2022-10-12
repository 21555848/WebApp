using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Areas.Identity.Data
{
    public static class AppDbInitializer
    {
        
        public  static async Task SeedRolesAsync(UserManager<WebAppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("SuperUser"));
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("Default"));
            await roleManager.CreateAsync(new IdentityRole("Doctor"));
        }

        public static async Task SeedDefaultUser(UserManager<WebAppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new WebAppUser
            {
                UserName = "superuser",
                Email = "21555848@dut4life.ac.za",
                EmailConfirmed = true
            };

            if(userManager.Users.All(a=>a.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if(user == null)
                {
                    await userManager.CreateAsync(defaultUser, "MediDevs2022!");
                    await userManager.AddToRoleAsync(defaultUser, "SuperUser");
                }
            }
        }
    }
}
