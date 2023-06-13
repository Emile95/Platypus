﻿namespace Logging
{
    public class LoggerManager
    {
        protected readonly Dictionary<Type, LoggerBase> _loggersWithoutSensivity;
        protected readonly Dictionary<Type, LoggerBase> _loggerWithSensivity;

        public LoggerManager()
        {
            _loggersWithoutSensivity = new Dictionary<Type, LoggerBase>();
            _loggerWithSensivity = new Dictionary<Type, LoggerBase>();
        }

        public LoggerType GetLogger<LoggerType>()
            where LoggerType : LoggerBase
        {
            Type type = typeof(LoggerType);
            return (LoggerType)GetLogger(type);
        }

        public LoggerBase GetLogger(Type type)
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
                fittingLoggers.AddRange(_loggersWithoutSensivity.Values);

            return fittingLoggers;
        }

        public void Log(string line)
        {
            foreach(LoggerBase logger in _loggersWithoutSensivity.Values)
                logger.Log(line);
            foreach (LoggerBase logger in _loggerWithSensivity.Values)
                logger.Log(line);
        }

        public void Log(LoggingLevel level, string line, bool includeUnsensivitiveLoggers = true)
        {
            ConsumeLoggerByLogLevel(level, (logger) => logger.Log(line));

            if (includeUnsensivitiveLoggers)
            {
                foreach (LoggerBase logger in _loggersWithoutSensivity.Values)
                    logger.Log(line);
            }
        }

        public void Log<LoggerType>(string line)
            where LoggerType : LoggerBase
        {
            LoggerType logger = GetLogger<LoggerType>();
            if(logger != null)
                logger.Log(line);
        }

        public void Log(Type type, string line)
        {
            LoggerBase logger = GetLogger(type);
            if (logger != null)
                logger.Log(line);
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
            foreach (LoggerBase logger in _loggerWithSensivity.Values)
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
            if (_loggersWithoutSensivity.ContainsKey(type))
                return;
            if (_loggerWithSensivity.ContainsKey(type))
                return;

            if (typeof(LoggerBase).IsAssignableFrom(type))
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
