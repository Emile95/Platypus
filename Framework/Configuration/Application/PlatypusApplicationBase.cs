using Common.Application;

namespace PlatypusApplicationFramework.Configuration.Application
{
    public abstract class PlatypusApplicationBase
    {
        public string ApplicationDirectoryPath { get; set; }
        public virtual void Install(ApplicationInstallEnvironment env) { }
        public virtual void Initialize(ApplicationInitializeEnvironment env) { }
    }
}
