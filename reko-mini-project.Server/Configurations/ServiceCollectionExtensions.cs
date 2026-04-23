using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using reko_mini_project.Server.Data;
using reko_mini_project.Server.ExceptionHandlers;
using reko_mini_project.Server.Features.ImageProcessing.Services;
using reko_mini_project.Server.Features.ImageProcessing.Validation;
using reko_mini_project.Server.Features.Products;

namespace reko_mini_project.Server.Configurations;

public static class ServiceCollectionExtensions
{
    public const string FRONTEND_CORS_POLICY = "FRONTEND_CORS_POLICY";
    public const string ALLOWED_ORIGINS_CONFIG_KEY = "ALLOWED_ORIGINS";
    public const string DATABASE_CONNECTION = "DefaultConnection";
    public const string DEFAULT_CHAT_CLIENT_SCOPE = "https://ai.azure.com/.default";
    public const string AI_MODEL_NAME = "OpenAI:ModelName";
    public const string AI_MODEL_ENDPOINT_CONFIG_KEY = "OpenAI:Endpoint";

    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenApi();
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

        return services;
    }

    public static IServiceCollection AddCorsConfiguration(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        var allowedOrigin = configuration[ALLOWED_ORIGINS_CONFIG_KEY]
            ?? throw new InvalidOperationException($"{ALLOWED_ORIGINS_CONFIG_KEY} not configured");

        services.AddCors(options =>
        {
            options.AddPolicy(FRONTEND_CORS_POLICY, policy =>
            {
                policy.WithOrigins(allowedOrigin)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }

    public static IServiceCollection AddSqliteDatabaseConfiguration(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString(DATABASE_CONNECTION)));
        return services;
    }
}