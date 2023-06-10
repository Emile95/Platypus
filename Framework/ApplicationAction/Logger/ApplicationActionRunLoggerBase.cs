namespace PlatypusApplicationFramework.ApplicationAction.Logger
{
    public abstract class ApplicationActionRunLoggerBase : IApplicationActionRunLogger
    {
        protected string RunningActionGuid;

        public ApplicationActionRunLoggerBase(string runningActionGuid)
        {
            RunningActionGuid = runningActionGuid;
        }

        public abstract void Log(string message);
    }
}
