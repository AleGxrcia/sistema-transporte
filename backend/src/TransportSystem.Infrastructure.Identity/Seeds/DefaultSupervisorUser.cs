using Microsoft.AspNetCore.Identity;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Infrastructure.Identity.Entities;

namespace TransportSystem.Infrastructure.Identity.Seeds
{
    public static class DefaultSupervisorUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "supervisor@transport.com",
                Email = "supervisor@transport.com",
                FirstName = "Supervisor",
                LastName = "Transport",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true,
            };

            var existingUser = await userManager.FindByEmailAsync(defaultUser.Email);

            if (existingUser is null)
            {
                await userManager.CreateAsync(defaultUser, "Superv123!");
                await userManager.AddToRoleAsync(defaultUser, UserRole.Supervisor.ToString());
            }
        }
    }
}
