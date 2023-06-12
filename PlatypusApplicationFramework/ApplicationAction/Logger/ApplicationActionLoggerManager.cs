using Logging;

namespace PlatypusApplicationFramework.ApplicationAction.Logger
{
    public class ApplicationActionLoggerManager : LoggerManager
    {
        public ApplicationActionLoggerManager(string runningActionGuid)
        {

        }

        public void Trace(string message)
        {
            Log(LoggingLevel.Trace, message);
        }

        public void Debug(string message)
        {
            Log(LoggingLevel.Debug, message);
        }

        public void Info(string message)
        {
            Log(LoggingLevel.Info, message);
        }

        public void Warn(string message)
        {
            Log(LoggingLevel.Warn, message);
        }

        public void Error(string message)
        {
            Log(LoggingLevel.Error, message);
        }

        public void Fatal(string message)
        {
            Log(LoggingLevel.Fatal, message);
        }

    }
}
