namespace OptimalCoder.Blueprint.Shared.Config
{
    public class LoggerAppSettings
    {
        public required LoggerTarget Database {  get; set; }
        public required LoggerTarget DataDog { get; set; }
        public required LoggerTarget Elastic { get; set; }
        public required LoggerTarget File { get; set; }
        
    }

    public class LoggerTarget
    {
        public required bool Enabled { get; set; }
        public required string Level { get; set; }
    }
}
