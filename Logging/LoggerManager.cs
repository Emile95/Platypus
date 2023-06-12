namespace Logging
{
    public class LoggerManager
    {
        protected readonly Dictionary<Type, ILogger> _loggersWithoutSensivity;
        protected readonly Dictionary<Type, ILogger> _loggerWithSensivity;

        public LoggerManager()
        {
            _loggersWithoutSensivity = new Dictionary<Type, ILogger>();
            _loggerWithSensivity = new Dictionary<Type, ILogger>();
        }

        public LoggerType GetLogger<LoggerType>()
            where LoggerType : class, ILogger
        {
            Type type = typeof(LoggerType);
            return (LoggerType)GetLogger(type);
        }

        public ILogger GetLogger(Type type)
        {

            if(_loggersWithoutSensivity.ContainsKey(type))
                return _loggersWithoutSensivity[type];

            if (_loggerWithSensivity.ContainsKey(type))
                return _loggerWithSensivity[type];

            return null;
        }

        public List<ILogger> GetLoggers(LoggingLevel level, bool includeUnsensivitiveLoggers = true)
        {
            List<ILogger> fittingLoggers = new List<ILogger>();

            ConsumeLoggerByLogLevel(level, (logger) => fittingLoggers.Add(logger));

            if (includeUnsensivitiveLoggers)
                fittingLoggers.AddRange(_loggersWithoutSensivity.Values);

            return fittingLoggers;
        }

        public void Log(string line)
        {
            foreach(ILogger logger in _loggersWithoutSensivity.Values)
                logger.Log(line);
            foreach (ILogger logger in _loggerWithSensivity.Values)
                logger.Log(line);
        }

        public void Log(LoggingLevel level, string line, bool includeUnsensivitiveLoggers = true)
        {
            ConsumeLoggerByLogLevel(level, (logger) => logger.Log(line));

            if (includeUnsensivitiveLoggers)
            {
                foreach (ILogger logger in _loggersWithoutSensivity.Values)
                    logger.Log(line);
            }
        }

        public void Log<LoggerType>(string line)
            where LoggerType : class, ILogger
        {
            LoggerType logger = GetLogger<LoggerType>();
            if(logger != null)
                logger.Log(line);
        }

        public void Log(Type type, string line)
        {
            ILogger logger = GetLogger(type);
            if (logger != null)
                logger.Log(line);
        }

        public void AddLogger(ILogger logger)
        {
            Type type = logger.GetType();
            AddLoggerWithTypeObject(logger, type);
        }

        public void CreateLogger<LoggerType>(params object[] parameters)
            where LoggerType : ILogger
        {
            CreateLoggerByType(typeof(LoggerType), parameters);
        }

        public void CreateLogger(Type loggerType, params object[] parameters)
        {
            CreateLoggerByType(loggerType, parameters);
        }

        private void ConsumeLoggerByLogLevel(LoggingLevel level, Action<ILogger> consumer)
        {
            foreach (ILogger logger in _loggerWithSensivity.Values)
            {
                ILoggingSensivity loggingSensivity = logger as ILoggingSensivity;
                if (loggingSensivity.MinimumLoggingLevel <= level && loggingSensivity.MaximumLoggingLevel >= level)
                    consumer.Invoke(logger);
            }
        }

        private void CreateLoggerByType(Type loggerType, params object[] parameters)
        {
            List<object> paramsToPass = new List<object>();

            foreach (object parameter in parameters)
                paramsToPass.Add(parameter);

            ILogger logger = Activator.CreateInstance(loggerType, paramsToPass.ToArray()) as ILogger;

            if (logger == null) return;

            AddLoggerWithTypeObject(
                logger,
                loggerType
            );
        }

        private void AddLoggerWithTypeObject(ILogger logger, Type type)
        {
            if (_loggersWithoutSensivity.ContainsKey(type))
                return;
            if (_loggerWithSensivity.ContainsKey(type))
                return;

            if (typeof(ILogger).IsAssignableFrom(type))
            {
                if (typeof(ILoggingSensivity).IsAssignableFrom(type))
                {
                    _loggerWithSensivity.Add(type, logger);
                    return;
                }
                _loggersWithoutSensivity.Add(type, logger);
            }
        }
    }
}
