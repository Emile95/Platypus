using Common;
using Common.Application;

namespace PlatypusApplicationFramework.Configuration.Application
{
    public abstract class PlatypusApplicationBase
    {
        public virtual void Install(ApplicationInstallEnvironment env) { }
        public virtual void Initialize(ApplicationInitializeEnvironment env) { }
    }
}
