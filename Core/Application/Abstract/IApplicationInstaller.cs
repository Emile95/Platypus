using PlatypusFramework.Configuration.Application;

namespace Core.Application.Abstract
{
    public interface IApplicationInstaller
    {
        PlatypusApplicationBase Install(string sourcePath);
    }
}
