using reko_mini_project.Server.Features.ImageProcessing.Services;
using reko_mini_project.Server.Features.ImageProcessing.Validation;
using reko_mini_project.Server.Features.Products;
using Microsoft.Identity.Web;

namespace reko_mini_project.Server.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMicrosoftIdentityWebApiAuthentication(configuration);
        services.AddAuthorization(options =>
            {
                options.AddPolicy("SupplierPolicy", policy => policy.RequireRole("Supplier"));
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            });
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<CustomOpenApiTransformer>();
        });
        services.AddProblemDetails();
        services.AddCorsConfiguration(configuration);
        services.AddSqliteDatabaseConfiguration(configuration);
        services.Configure<BlobStorageOptions>(configuration.GetSection(BlobStorageOptions.SectionName));
        services.AddScoped<IImageValidator, ImageValidator>();
        services.AddScoped<IImageUploadService, ImageUploadService>();
        services.AddScoped<ImageAnalysisService>();
        services.AddScoped<IProductService, ProductService>();
        services.SetupChatClient(configuration);

        return services;
    }
}