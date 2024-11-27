using LaptopStore.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace LaptopStore.API.Infrastructure
{
    public class SeedData
    {
        public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = serviceProvider.GetRequiredService<ILogger<SeedData>>();

            // 1. Tạo vai trò Admin
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                logger.LogInformation("Admin role created.");
            }

            // 2. Tạo tài khoản Admin
            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    Name = "Admin User",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                    logger.LogInformation("Admin account created successfully.");
                }
                else
                {
                    logger.LogError("Failed to create admin account: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger.LogInformation("Admin account already exists.");
            }
        }
    }
}
