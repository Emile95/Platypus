using PlatypusLogging;

namespace PlatypusNetwork.Logger.FileLogger
{
    public abstract class FileLogger : LoggerBase
    {
        private readonly string _directoryPath;
        private readonly string _currentfilePath;
        private readonly int maximumFileSizeInBytes;
        private readonly string _logFileExtension;
        
        public FileLogger(string directoryPath, string fileName, int maximumFileSizeInKiloBytes)
        {
            _directoryPath = directoryPath;
            maximumFileSizeInBytes = maximumFileSizeInKiloBytes * 1000;
            _currentfilePath = Path.Combine(directoryPath, fileName);
            FileInfo fileInfo = new FileInfo(_currentfilePath);
            _logFileExtension = fileInfo.Extension;
        }

        public override void Log(string line)
        {
            if (File.Exists(_currentfilePath) == false)
            {
                File.WriteAllText(_currentfilePath, line + Environment.NewLine);
                return;
            }

            FileInfo fileInfo = new FileInfo(_currentfilePath);
            if((fileInfo.Length + line.Length) >= maximumFileSizeInBytes)
                ArchiveLogFile();

            File.AppendAllText(_currentfilePath, line + Environment.NewLine);
        }

        private void ArchiveLogFile()
        {
            string newFileName = $"{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}{_logFileExtension}";
            File.Copy(_currentfilePath, Path.Combine(_directoryPath, newFileName));
            File.Delete(_currentfilePath);
        }
    }
}
