using System.Threading.RateLimiting;

namespace OptimalCoder.Blueprint.API.RateLimiting
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddOptimalRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                        httpContext =>
                        {
                            var partitionKey = GetPartitionKey(httpContext);

                            return RateLimitPartition.GetFixedWindowLimiter(
                                    partitionKey,
                                    _ => new FixedWindowRateLimiterOptions
                                    {
                                        AutoReplenishment = true,
                                        PermitLimit = 10,
                                        QueueLimit = 0,
                                        Window = TimeSpan.FromMinutes(1)
                                    });
                        });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }

        private static string GetPartitionKey(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                return $"user:{context.User.Identity.Name}";
            }

            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            return $"ip:{ipAddress ?? "unknown"}";
        }
    }
}
