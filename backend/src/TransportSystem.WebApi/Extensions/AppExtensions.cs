using Swashbuckle.AspNetCore.SwaggerUI;

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
    }
}
