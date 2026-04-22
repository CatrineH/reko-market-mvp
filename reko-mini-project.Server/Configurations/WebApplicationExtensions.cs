using reko_mini_project.Server.Features.Products;
using reko_mini_project.Server.Features.ImageProcessing;

namespace reko_mini_project.Server.Configurations;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseExceptionHandler("/error");
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        var corsPolicyName = CorsExtensions.FRONTEND_CORS_POLICY;
        app.UseCors(corsPolicyName);
        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.MapScalarDocs();
        }

        app.MapProductEndpoints();
        app.MapImageProcessingEndpoints();

        return app;
    }
}