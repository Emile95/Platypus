using Logging;
using Common.Logger.FileLogger;

namespace PlatypusApplicationFramework.ApplicationAction.Logger
{
    public class ApplicationActionRunFileLogger : FileLogger
    {
        public ApplicationActionRunFileLogger(string filePath)
            : base(filePath)
        {
            LineFormat = "{0} : {1} : {2}";
        }

        public override void Log(string line)
        {
            Log(LoggingLevel.Info, line);
        }

        public override void Log(LoggingLevel level, string line)
        {
            string expandedLine = string.Format(LineFormat, DateTime.Now, level.ToString(), line);
            base.Log(expandedLine);
        }
    }
}
