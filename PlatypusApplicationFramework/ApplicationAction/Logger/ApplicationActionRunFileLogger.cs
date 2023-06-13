using Logging;
using Common.Logger.FileLogger;

namespace PlatypusApplicationFramework.ApplicationAction.Logger
{
    public class ApplicationActionRunFileLogger : FileLogger
    {
        private readonly string _lineFormat;

        public ApplicationActionRunFileLogger(string filePath)
            : base(filePath)
        {
            _lineFormat = "{0} : {1} : {2}";
        }

        public override void Log(string line)
        {
            Log(LoggingLevel.Info, line);
        }

        public override void Log(LoggingLevel level, string line)
        {
            string expandedLine = string.Format(_lineFormat, level.ToString(), DateTime.Now, line);
            base.Log(expandedLine);
        }
    }
}
