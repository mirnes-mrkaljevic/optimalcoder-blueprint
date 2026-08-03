using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OptimalCoder.Blueprint.Infra.Logger
{
    public static class LoggerExtensions
    {
        public static IServiceCollection AddOptimalLogger(this ILoggingBuilder logBuilder)
        {
            return logBuilder.Services.AddSingleton<IOptimalLogger, OptimalLogger>();
        }

    }
}
