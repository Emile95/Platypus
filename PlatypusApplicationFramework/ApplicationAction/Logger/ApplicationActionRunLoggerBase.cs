namespace PlatypusApplicationFramework.ApplicationAction.Logger
{
    public abstract class ApplicationActionRunLoggerBase : IApplicationActionRunLogger
    {
        protected string ActionGuid;

        public ApplicationActionRunLoggerBase(string actionGuid)
        {
            ActionGuid = actionGuid;
        }

        public abstract void Log(string message);
    }
}
