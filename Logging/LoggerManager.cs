namespace Logging
{
    public class LoggerManager
    {
        protected readonly Dictionary<Type, List<LoggerBase>> _loggersWithoutSensivity;
        protected readonly Dictionary<Type, List<LoggerBase>> _loggerWithSensivity;

        public LoggerManager()
        {
            _loggersWithoutSensivity = new Dictionary<Type, List<LoggerBase>>();
            _loggerWithSensivity = new Dictionary<Type, List<LoggerBase>>();
        }

        public List<LoggerBase> GetLoggers<LoggerType>()
            where LoggerType : LoggerBase
        {
            Type type = typeof(LoggerType);
            return GetLoggers(type);
        }

        public List<LoggerBase> GetLoggers(Type type)
        {

            if(_loggersWithoutSensivity.ContainsKey(type))
                return _loggersWithoutSensivity[type];

            if (_loggerWithSensivity.ContainsKey(type))
                return _loggerWithSensivity[type];

            return null;
        }

        public List<LoggerBase> GetLoggers(LoggingLevel level, bool includeUnsensivitiveLoggers = true)
        {
            List<LoggerBase> fittingLoggers = new List<LoggerBase>();

            ConsumeLoggerByLogLevel(level, (logger) => fittingLoggers.Add(logger));

            if (includeUnsensivitiveLoggers)
                foreach(List<LoggerBase> loggerWithoutSensivityList in _loggersWithoutSensivity.Values)
                    fittingLoggers.AddRange(loggerWithoutSensivityList);

            return fittingLoggers;
        }

        public void Log(string line)
        {
            foreach(List<LoggerBase> loggers in _loggersWithoutSensivity.Values)
                foreach(LoggerBase logger in loggers)
                    logger.Log(line);

            foreach (List<LoggerBase> loggers in _loggerWithSensivity.Values)
                foreach (LoggerBase logger in loggers)
                    logger.Log(line);
        }

        public void Log(LoggingLevel level, string line, bool includeUnsensivitiveLoggers = true)
        {
            ConsumeLoggerByLogLevel(level, (logger) => logger.Log(line));

            if (includeUnsensivitiveLoggers)
            {
                foreach(List<LoggerBase> loggers in _loggersWithoutSensivity.Values)
                    foreach (LoggerBase logger in loggers)
                        logger.Log(line);
            }
        }

        public void Log<LoggerType>(string line)
            where LoggerType : LoggerBase
        {
            Log(typeof(LoggerType), line);
        }

        public void Log(Type type, string line)
        {
            List<LoggerBase> loggers = GetLoggers(type);
            if (loggers != null)
            {
                foreach(LoggerBase logger in loggers)
                    logger.Log(line);
            }
        }

        public void AddLogger(LoggerBase logger)
        {
            Type type = logger.GetType();
            AddLoggerWithTypeObject(logger, type);
        }

        public void CreateLogger<LoggerType>(params object[] parameters)
            where LoggerType : LoggerBase
        {
            CreateLoggerByType(typeof(LoggerType), parameters);
        }

        public void CreateLogger(Type loggerType, params object[] parameters)
        {
            CreateLoggerByType(loggerType, parameters);
        }

        private void ConsumeLoggerByLogLevel(LoggingLevel level, Action<LoggerBase> consumer)
        {
            foreach (List<LoggerBase> loggers in _loggerWithSensivity.Values)
            {
                foreach(LoggerBase logger in loggers)
                {
                    ILoggingSensivity loggingSensivity = logger as ILoggingSensivity;
                    if (loggingSensivity.MinimumLoggingLevel <= level && loggingSensivity.MaximumLoggingLevel >= level)
                        consumer.Invoke(logger);
                }
            }
        }

        private void CreateLoggerByType(Type loggerType, params object[] parameters)
        {
            List<object> paramsToPass = new List<object>();

            foreach (object parameter in parameters)
                paramsToPass.Add(parameter);

            LoggerBase logger;
            if (parameters.Length == 0)
                logger = Activator.CreateInstance(loggerType) as LoggerBase;
            else
                logger = Activator.CreateInstance(loggerType, paramsToPass.ToArray()) as LoggerBase;

            if (logger == null) return;

            AddLoggerWithTypeObject(
                logger,
                loggerType
            );
        }

        private void AddLoggerWithTypeObject(LoggerBase logger, Type type)
        {
            if (typeof(LoggerBase).IsAssignableFrom(type))
            {
                if (typeof(ILoggingSensivity).IsAssignableFrom(type))
                {
                     if (_loggerWithSensivity.ContainsKey(type) == false)
                        _loggerWithSensivity.Add(type, new List<LoggerBase>());

                    _loggerWithSensivity[type].Add(logger);
                    return;
                }

                if (_loggersWithoutSensivity.ContainsKey(type) == false)
                    _loggersWithoutSensivity.Add(type, new List<LoggerBase>());

                _loggersWithoutSensivity[type].Add(logger);
            }
        }
    }
}
