using Microsoft.EntityFrameworkCore;
using reko_mini_project.Server.Data;

namespace reko_mini_project.Server.Configurations;

public static class DatabaseExtensions
{
    public const string DATABASE_CONNECTION = "DefaultConnection";

    public static IServiceCollection AddAzureSqlDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString(DATABASE_CONNECTION)));
        return services;
    }
}
