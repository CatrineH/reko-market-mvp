namespace reko_mini_project.Server.Configurations;

public static class AuthorizationExtensions
{
    public static class Roles
    {
        public const string SUPPLIER = "Supplier";
        public const string ADMIN = "Admin";
    }

    public static class Policies
    {
        public const string READ_GLOBAL_PRODUCTS = "ReadGlobalProducts";
        public const string WRITE_GLOBAL_PRODUCTS = "WriteGlobalProducts";
        public const string ANALYZE_IMAGE_WITH_AI = "AnalyzeImageWithAI";
    }

    public static IServiceCollection AddAuthorizationConfiguration(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.READ_GLOBAL_PRODUCTS, policy =>
                policy.RequireRole(Roles.SUPPLIER, Roles.ADMIN));

            options.AddPolicy(Policies.WRITE_GLOBAL_PRODUCTS, policy =>
                policy.RequireRole(Roles.ADMIN));

            options.AddPolicy(Policies.ANALYZE_IMAGE_WITH_AI, policy =>
                policy.RequireRole(Roles.ADMIN, Roles.SUPPLIER));
        });

        return services;
    }
}
