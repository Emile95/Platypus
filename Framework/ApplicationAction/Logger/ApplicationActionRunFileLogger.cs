namespace PlatypusApplicationFramework.ApplicationAction.Logger
{
    public class ApplicationActionRunFileLogger : ApplicationActionRunLoggerBase
    {
        public ApplicationActionRunFileLogger(string runningActionGuid)
            : base(runningActionGuid) {}

        public override void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
