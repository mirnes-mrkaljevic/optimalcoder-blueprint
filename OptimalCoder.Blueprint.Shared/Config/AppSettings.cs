namespace OptimalCoder.Blueprint.Shared.Config
{
    public class AppSettings
    {
        public ConnectionStrings? ConnectionStrings { get; set; }
        public LoggerAppSettings? Logging { get; set; }

        public Jwt? Jwt { get; set; }
    }

    public class ConnectionStrings
    {
        public string? DefaultConnection { get; set; }
    }
}
