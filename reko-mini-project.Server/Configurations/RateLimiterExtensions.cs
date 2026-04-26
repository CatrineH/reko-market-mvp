using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace reko_mini_project.Server.Configurations;

public static class RateLimiterExtensions
{
    public const string WritePolicyName = "write";

    public static IServiceCollection AddRateLimiterConfiguration(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter(WritePolicyName, o =>
            {
                o.PermitLimit = 20;
                o.Window = TimeSpan.FromMinutes(1);
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                o.QueueLimit = 0;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        return services;
    }
}
