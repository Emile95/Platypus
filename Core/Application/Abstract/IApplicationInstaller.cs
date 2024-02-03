using PlatypusFramework.Configuration.Application;

namespace Core.Application.Abstract
{
    public interface IApplicationInstaller
    {
        void Install(string sourcePath);
    }
}
