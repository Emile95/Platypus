﻿namespace PlatypusLogging
{
    public abstract class LoggerBase
    {
        public abstract void Log(string line);

        public abstract void Log(LoggingLevel level, string line);

        public void Trace(string line)
        {
            Log(LoggingLevel.Trace, line);
        }

        public void Debug(string line)
        {
            Log(LoggingLevel.Debug, line);
        }

        public void Info(string line)
        {
            Log(LoggingLevel.Info, line);
        }

        public void Warn(string line)
        {
            Log(LoggingLevel.Warn, line);
        }

        public void Error(string line)
        {
            Log(LoggingLevel.Error, line);
        }

        public void Fatal(string line)
        {
            Log(LoggingLevel.Fatal, line);
        }
    }
}
