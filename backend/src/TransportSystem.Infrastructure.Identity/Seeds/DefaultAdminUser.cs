using Microsoft.AspNetCore.Identity;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Infrastructure.Identity.Entities;

namespace TransportSystem.Infrastructure.Identity.Seeds
{
    public static class DefaultAdminUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "admin@transport.com",
                Email = "admin@transport.com",
                FirstName = "Admin",
                LastName = "Transport",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true,
            };

            var existingUser = await userManager.FindByEmailAsync(defaultUser.Email);

            if (existingUser is null)
            {
                await userManager.CreateAsync(defaultUser, "Admin123!");
                await userManager.AddToRoleAsync(defaultUser, UserRole.Admin.ToString());
            }
        }
    }
}
