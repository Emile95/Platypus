using PlatypusApplicationFramework.Core.Application;

namespace PlatypusApplicationFramework.Configuration.Application
{
    public abstract class PlatypusApplicationBase
    {
        public string ApplicationDirectoryPath { get; set; }
        public virtual void Install(ApplicationInstallEnvironment env) { }
        public virtual void Uninstall(ApplicationInstallEnvironment env) { }
        public virtual void Initialize(ApplicationInitializeEnvironment env) { }
        public virtual string GetApplicationName() { return GetType().Name; }
    }
}
