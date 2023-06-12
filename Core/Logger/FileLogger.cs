using Logging;

namespace Core.Logger
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Log(string line)
        {
            File.WriteAllText(_filePath, line);
        }
    }
}
