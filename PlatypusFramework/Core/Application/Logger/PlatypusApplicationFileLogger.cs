using PlatypusNetwork.Logger.FileLogger;
using PlatypusLogging;

namespace PlatypusFramework.Core.Application.Logger
{
    public class PlatypusApplicationFileLogger : FileLogger, ILoggingSensivity
    {
        private readonly string _format;

        public LoggingLevel MinimumLoggingLevel { get; set; }
        public LoggingLevel MaximumLoggingLevel { get; set; }

        public PlatypusApplicationFileLogger(string directoryPath, string fileName, int maximumFileSizeInKiloBytes, string format, LoggingLevel minimumLoggingLevel, LoggingLevel maximumLoggingLevel) 
            : base(directoryPath, fileName, maximumFileSizeInKiloBytes)
        {
            _format = format;
            MinimumLoggingLevel = minimumLoggingLevel;
            MaximumLoggingLevel = maximumLoggingLevel;
        }

        public override void Log(LoggingLevel level, string line)
        {
            string completLine = _format.Replace("{dateTime}", DateTime.Now.ToString());
            completLine = completLine.Replace("{loggingLevel}", level.ToString());
            completLine = completLine.Replace("{message}", line);
            Log(completLine);
        }
    }
}
