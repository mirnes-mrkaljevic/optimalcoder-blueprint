using OptimalCoder.Blueprint.Shared.Config;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace OptimalCoder.Blueprint.Infra.Logger
{
    public static class LoggerBuilder
    {
        public static Serilog.ILogger Build(AppSettings appSettings)
        {
            return new LoggerConfiguration()
                .ConfigureDbTarget(appSettings)
                .ConfigureFileTarget(appSettings)
                .ConfigureDatadog(appSettings)
                .ConfigureElasticSearch(appSettings)
                .CreateLogger();
        }

        private static LoggerConfiguration ConfigureDbTarget(this LoggerConfiguration loggerConfiguration, AppSettings appSettings)
        {
            loggerConfiguration.WriteTo
                    .MSSqlServer(
                        connectionString: appSettings.ConnectionStrings.DefaultConnection,
                        sinkOptions: new MSSqlServerSinkOptions { TableName = "ErrorLog", AutoCreateSqlTable = true },
                        restrictedToMinimumLevel: LogEventLevel.Error);

            if (appSettings.Logging.Database.Enabled)
            {

                loggerConfiguration.WriteTo
                    .MSSqlServer(
                        connectionString: appSettings.ConnectionStrings.DefaultConnection,
                        sinkOptions: new MSSqlServerSinkOptions { TableName = "Log", AutoCreateSqlTable = true },
                        restrictedToMinimumLevel: GetLevel(appSettings.Logging.Database.Level));
            }

            return loggerConfiguration;
        }

        private static LogEventLevel GetLevel(string level)
        {
            return level switch
            {
                "Error" => LogEventLevel.Error,
                "Warn" => LogEventLevel.Warning,
                "Debug" => LogEventLevel.Debug,
                "Verbose" => LogEventLevel.Verbose,
                _ => LogEventLevel.Information,
            };
        }

        private static LoggerConfiguration ConfigureFileTarget(this LoggerConfiguration loggerConfiguration, AppSettings appSettings)
        {
            return loggerConfiguration;
        }

        private static LoggerConfiguration ConfigureDatadog(this LoggerConfiguration loggerConfiguration, AppSettings appSettings)
        {
            return loggerConfiguration;
        }
        private static LoggerConfiguration ConfigureElasticSearch(this LoggerConfiguration loggerConfiguration, AppSettings appSettings)
        {
            return loggerConfiguration;
        }
    }
}
