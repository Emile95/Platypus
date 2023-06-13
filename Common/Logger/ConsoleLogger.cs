using Logging;

namespace Common.Logger
{
    public class ConsoleLogger : LoggerBase
    {
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
