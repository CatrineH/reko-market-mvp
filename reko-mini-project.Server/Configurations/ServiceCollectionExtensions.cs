using Microsoft.AspNetCore.Http.Features;
using reko_mini_project.Server.ExceptionHandlers;
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
        services.AddAuthorizationConfiguration();
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<CustomOpenApiTransformer>();
        });
        var appInsightsConnectionString = configuration["ApplicationInsights:ConnectionString"];
        if (!string.IsNullOrEmpty(appInsightsConnectionString))
        {
            services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = appInsightsConnectionString;
            });
        }
        services.AddProblemDetails();
        services.AddExceptionHandler<ArgumentExceptionHandler>();
        services.AddExceptionHandler<InvalidDataExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = ImageValidator.MaxFileSizeBytes;
        });
        services.AddCorsConfiguration(configuration);
        services.AddSqliteDatabaseConfiguration(configuration);
        services.Configure<BlobStorageOptions>(configuration.GetSection(BlobStorageOptions.SectionName));
        services.AddScoped<IImageValidator, ImageValidator>();
        services.AddScoped<IImageUploadService, ImageUploadService>();
        services.AddScoped<ImageAnalysisService>();
        services.AddScoped<IProductService, ProductService>();
        services.SetupChatClient(configuration);
        services.AddRateLimiterConfiguration();

        return services;
    }
}