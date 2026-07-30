using OptimalCoder.Blueprint.Shared.Config;

namespace OptimalCoder.Blueprint.Infra
{
    public class AppSettings
    {
        public required ConnectionStrings ConnectionStrings { get; set; }
        public required LoggerAppSettings Logging { get; set; }
    }

    public class ConnectionStrings
    {
        public required string DefaultConnection { get; set; }
    }
}
