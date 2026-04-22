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
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            await next();
        });
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