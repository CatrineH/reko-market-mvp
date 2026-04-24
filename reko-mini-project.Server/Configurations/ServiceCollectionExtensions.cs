using Microsoft.EntityFrameworkCore;
using reko_mini_project.Server.Data;
using reko_mini_project.Server.Features.ImageProcessing.Services;
using reko_mini_project.Server.Features.ImageProcessing.Validation;
using reko_mini_project.Server.Features.Products;
using Azure.Identity;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel.Primitives;

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

    public static IServiceCollection SetupChatClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton((provider) =>
        {
            BearerTokenPolicy tokenPolicy = new(
                new DefaultAzureCredential(),
                DEFAULT_CHAT_CLIENT_SCOPE);

            string model = AI_MODEL_NAME;
            string? endpoint = configuration[AI_MODEL_ENDPOINT_CONFIG_KEY];

            if (endpoint == null)
            {
                // Absence of endpoint configuration should be handled gracefully 
                // as it might be intentional in certain environments,
                // (e.g., local development without AI resources deployed).
                Console.WriteLine($"Warning: {AI_MODEL_ENDPOINT_CONFIG_KEY} is not configured. ChatClient will not be initialized.");
                return null!;
            }

            // Disable the OPENAI001 warning for this specific instance, 
            // as we are intentionally using the ChatClient constructor that accepts an authentication policy.
#pragma warning disable OPENAI001
            return new ChatClient(
                model: model,
                authenticationPolicy: tokenPolicy,
                options: new OpenAIClientOptions()
                {
                    Endpoint = new Uri(endpoint)
                });
#pragma warning restore OPENAI001
        });

        return services;
    }
}