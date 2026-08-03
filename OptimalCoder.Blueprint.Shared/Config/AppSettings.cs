namespace OptimalCoder.Blueprint.Shared.Config
{
    public class AppSettings
    {
        public required ConnectionStrings ConnectionStrings { get; set; }
        public required LoggerAppSettings Logging { get; set; }

        public required Jwt Jwt { get; set; }
    }

    public class ConnectionStrings
    {
        public required string DefaultConnection { get; set; }
    }
}
