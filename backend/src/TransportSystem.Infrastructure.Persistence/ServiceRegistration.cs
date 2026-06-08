using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Repositories;
using TransportSystem.Infrastructure.Persistence.Contexts;
using TransportSystem.Infrastructure.Persistence.Repositories;
using TransportSystem.Infrastructure.Persistence.UnitOfWorkk;

namespace TransportSystem.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<ITravelRequestRepository, TravelRequestRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}