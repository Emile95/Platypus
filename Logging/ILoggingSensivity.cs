namespace PlatypusLogging
{
    public interface ILoggingSensivity
    {
        LoggingLevel MinimumLoggingLevel { get; set; }
        LoggingLevel MaximumLoggingLevel { get; set; }
    }
}
