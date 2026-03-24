using reko_mini_project.Server.Features.Products;
using Scalar.AspNetCore;

namespace reko_mini_project.Server.Configurations;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection();
        var corsPolicyName = ServiceCollectionExtensions.FRONTEND_CORS_POLICY;
        app.UseCors(corsPolicyName);

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.MapProductEndpoints();

        return app;
    }
}