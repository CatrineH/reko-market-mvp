namespace reko_mini_project.Server.Configurations;

public static class ServiceCollectionExtensions
{
    public const string FRONTEND_CORS_POLICY = "FRONTEND_CORS_POLICY";
    public const string ALLOWED_ORIGINS_CONFIG_KEY = "ALLOWED_ORIGINS";

    public static IServiceCollection AppApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddCorsConfiguration(configuration);

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
}