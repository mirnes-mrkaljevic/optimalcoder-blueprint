using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OptimalCoder.Blueprint.Infra.Logger
{
    public static class LoggingBuilderExtensions
    {
        public static IServiceCollection AddOptimalLogger(this ILoggingBuilder logBuilder)
        {
            return logBuilder.Services.AddScoped<IOptimalLogger, OptimalLogger>();
        }
    }
}
