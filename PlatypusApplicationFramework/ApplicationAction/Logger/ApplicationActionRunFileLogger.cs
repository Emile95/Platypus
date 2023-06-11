using Persistance;

namespace PlatypusApplicationFramework.ApplicationAction.Logger
{
    public class ApplicationActionRunFileLogger : ApplicationActionRunLoggerBase
    {
        private readonly string _logFilePath;

        public ApplicationActionRunFileLogger(string actionGuid, ApplicationActionRepository applicationActionRepository, int runNumber)
            : base(actionGuid) 
        {
            _logFilePath = applicationActionRepository.GetRunActionLogFilePath(actionGuid, runNumber);
        }

        public override void Log(string message)
        {
            File.AppendAllText(_logFilePath, message + Environment.NewLine);
        }
    }
}
