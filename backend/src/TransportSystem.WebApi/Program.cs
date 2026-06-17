using Microsoft.AspNetCore.Identity;
using TransportSystem.Core.Application;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Infrastructure.Identity;
using TransportSystem.Infrastructure.Identity.Entities;
using TransportSystem.Infrastructure.Identity.Seeds;
using TransportSystem.Infrastructure.Persistence;
using TransportSystem.Infrastructure.Shared;
using TransportSystem.WebApi.Extensions;
using TransportSystem.WebApi.Middlewares;
using TransportSystem.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddApplicationLayer();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);

builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerExtension();
builder.Services.AddCorsExtension();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await DefaultRoles.SeedAsync(roleManager);
        await DefaultAdminUser.SeedAsync(userManager);
        await DefaultSupervisorUser.SeedAsync(userManager);
        await DefaultOperatorUser.SeedAsync(userManager);

        logger.LogInformation("Identity seed completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding Identity data.");
    }
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseSwaggerExtension();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

