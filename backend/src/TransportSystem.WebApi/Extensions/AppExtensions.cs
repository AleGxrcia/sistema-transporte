using Swashbuckle.AspNetCore.SwaggerUI;
using TransportSystem.WebApi.Middlewares;

namespace TransportSystem.WebApi.Extensions
{
    public static class AppExtensions
    {
        public static WebApplication UseSwaggerExtension(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Transport System API v1");
                    options.DisplayRequestDuration();
                    options.DocExpansion(DocExpansion.None);
                    options.DefaultModelRendering(ModelRendering.Model);
                });
            }

            return app;
        }

        public static WebApplication UseErrorHandlingMiddleware(this WebApplication app)
        {
            app.UseMiddleware<GlobalExceptionHandler>();
            return app;
        }
    }
}
