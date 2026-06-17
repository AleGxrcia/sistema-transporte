using Microsoft.AspNetCore.Identity;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Infrastructure.Identity.Entities;

namespace TransportSystem.Infrastructure.Identity.Seeds
{
    public static class DefaultOperatorUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "operador@transport.com",
                Email = "operador@transport.com",
                FirstName = "Operador",
                LastName = "Transport",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true,
            };

            var existingUser = await userManager.FindByEmailAsync(defaultUser.Email);

            if (existingUser is null)
            {
                await userManager.CreateAsync(defaultUser, "Oper123!");
                await userManager.AddToRoleAsync(defaultUser, UserRole.Operator.ToString());
            }
        }
    }
}
