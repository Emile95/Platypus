namespace PlatypusApplicationFramework.ApplicationAction.Logger
{
    public class ApplicationActionRunLoggers
    {
        private readonly string _runningActionGuid;
        private readonly Dictionary<Type, IApplicationActionRunLogger> _loggers;

        public ApplicationActionRunLoggers(string runningActionGuid)
        {
            _runningActionGuid = runningActionGuid;
            _loggers = new Dictionary<Type, IApplicationActionRunLogger>();
        }

        public void LogToAllLoggers(string message)
        {
            foreach (IApplicationActionRunLogger logger in _loggers.Values)
                logger.Log(message);
        }

        public void Log<LoggerType>(string message)
            where LoggerType : IApplicationActionRunLogger
        {
            Type loggerType = typeof(LoggerType);
            _loggers[loggerType].Log(message);
        }

        public void AddLogger<LoggerType>()
            where LoggerType : ApplicationActionRunLoggerBase
        {
            Type loggerType = typeof(LoggerType);
            AddLogger(loggerType);
        }

        public void AddLogger(Type loggerType)
        {
            if(loggerType.IsInstanceOfType(typeof(ApplicationActionRunLoggerBase)))
                _loggers.Add(loggerType, Activator.CreateInstance(loggerType, new object[] { _runningActionGuid }) as ApplicationActionRunLoggerBase);
        }
    }
}
