namespace PlatypusLogging.Loggers
{
    public class ConsoleLogger : LoggerBase, ILoggingSensivity
    {
        public LoggingLevel MinimumLoggingLevel { get; set; }
        public LoggingLevel MaximumLoggingLevel { get; set; }

        public override void Log(string line)
        {
            Console.WriteLine(line);
        }

        public override void Log(LoggingLevel level, string line)
        {
            Console.WriteLine(line);
        }
    }
}
