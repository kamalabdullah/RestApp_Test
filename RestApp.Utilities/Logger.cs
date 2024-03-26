using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;


namespace RestApp.Utilities
{
    [ExcludeFromCodeCoverageAttribute]
    public class CustomLogger : ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public CustomLogger(ILogger<CustomLogger> logger)
        {
            this._logger = logger;
        }
        public void Error(Exception exception)
        {
            this._logger.LogError(exception, exception.Message);
        }
    }
}
