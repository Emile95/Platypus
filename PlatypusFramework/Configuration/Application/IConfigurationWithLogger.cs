namespace PlatypusFramework.Configuration.Application
{
    public interface IConfigurationWithLogger
    {
        PlatypusApplicationLoggerConfiguration GetLogger();
    }
}
