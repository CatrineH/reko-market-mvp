using Microsoft.EntityFrameworkCore;
using reko_mini_project.Server.Data;

namespace reko_mini_project.Server.Configurations;

public static class ServiceCollectionExtensions
{
    public const string FRONTEND_CORS_POLICY = "FRONTEND_CORS_POLICY";
    public const string ALLOWED_ORIGINS_CONFIG_KEY = "ALLOWED_ORIGINS";
    public const string DATABASE_CONNECTION = "DefaultConnection";

    public static IServiceCollection AppApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddCorsConfiguration(configuration);
        services.AddDatabaseConfiguration(configuration);

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

    public static IServiceCollection AddDatabaseConfiguration(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString(DATABASE_CONNECTION)));
        return services;
    }
}