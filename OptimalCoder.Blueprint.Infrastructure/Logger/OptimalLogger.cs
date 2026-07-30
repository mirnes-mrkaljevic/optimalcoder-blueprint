using Microsoft.Extensions.Options;

namespace OptimalCoder.Blueprint.Infra.Logger
{
    public interface IOptimalLogger
    {
        public void Info(string message);
        public void Error(string message);
        public void Warn(string message);
        public void Debug(string message);
        public void Verbose(string message);
        public void Error(Exception ex, string message);
    }

    public class OptimalLogger : IOptimalLogger, IDisposable
    {

        private Serilog.ILogger _logger;
        public OptimalLogger(IOptionsSnapshot<AppSettings> appSettings)
        {
            _logger = LoggerBuilder.Build(appSettings.Value);
        }
        public void Build(AppSettings appSettings) 
        {
            _logger = LoggerBuilder.Build(appSettings);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Dispose()
        {
            
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(Exception ex, string message)
        {
            _logger.Error(ex, message);
        }

        public void Info(string message)
        {
            _logger.Information(message);
        }

        public void Verbose(string message)
        {
            _logger.Verbose(message);
        }

        public void Warn(string message)
        {
            _logger.Warning(message);
        }
    }
}
