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

        public void AddLogger<LoggerType>(params object[] parameters)
            where LoggerType : IApplicationActionRunLogger
        {
            Type loggerType = typeof(LoggerType);
            AddLogger(loggerType, parameters);
        }

        public void AddLogger(Type loggerType, params object[] parameters)
        {
            List<object> paramsToPass = new List<object>();

            paramsToPass.Add(_runningActionGuid);

            foreach (object parameter in parameters)
                paramsToPass.Add(parameter);

            if (typeof(IApplicationActionRunLogger).IsAssignableFrom(loggerType))
                _loggers.Add(loggerType, Activator.CreateInstance(loggerType, paramsToPass.ToArray()) as IApplicationActionRunLogger);
        }
    }
}
