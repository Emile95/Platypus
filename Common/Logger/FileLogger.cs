using Logging;

namespace Common.Logger.FileLogger
{
    public abstract class FileLogger : LoggerBase
    {
        private readonly string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public override void Log(string line)
        {
            File.WriteAllText(_filePath, line);
        }
    }
}
