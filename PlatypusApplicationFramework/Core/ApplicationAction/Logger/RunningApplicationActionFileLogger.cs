using PlatypusNetwork.Logger.FileLogger;
using PlatypusLogging;
using Persistance;

namespace PlatypusApplicationFramework.Core.ApplicationAction.Logger
{
    public class RunningApplicationActionFileLogger : FileLogger
    {
        private string _lineFormat = "[{dateTime}] - {loggingLevel} : {message}";

        public RunningApplicationActionFileLogger(string actionGuid, int runNumber, int maximumFileSizeInKilobytes)
            : base(Path.Combine(ApplicationPaths.GetActionRunsDirectoryPath(actionGuid), runNumber.ToString()), ApplicationPaths.ACTIONLOGFILENAME, maximumFileSizeInKilobytes)
        { }

        public override void Log(LoggingLevel level, string line)
        {
            string completLine = _lineFormat.Replace("{dateTime}", DateTime.Now.ToString());
            completLine = completLine.Replace("{loggingLevel}", level.ToString());
            completLine = completLine.Replace("{message}", line);
            Log(completLine);
        }
    }
}
