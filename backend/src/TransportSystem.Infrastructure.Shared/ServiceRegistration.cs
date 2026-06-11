using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Infrastructure.Shared.Services;
using TransportSystem.Infrastructure.Shared.Settings;

namespace TransportSystem.Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddScoped<IEmailService, EmailService>();
        }
    }
}
