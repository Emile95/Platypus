using PlatypusFramework.Configuration.ApplicationAction;
using PlatypusFramework.Core.Application;
using System.Reflection;

namespace PlatypusFramework.Configuration.Application
{
    public abstract class PlatypusApplicationBase
    {
        public string ApplicationDirectoryPath { get; set; }
        public virtual void Install(ApplicationInstallEnvironment env) { }
        public virtual void Uninstall(ApplicationInstallEnvironment env) { }
        public virtual void Initialize(ApplicationInitializeEnvironment env) { }
        public virtual string GetApplicationName() { return GetType().Name; }

        public string[] GetAllApplicationActionNames()
        {
            List<string> applicationGuids = new List<string>();

            MethodInfo[] methodInfos = GetType().GetMethods();

            foreach (MethodInfo methodInfo in methodInfos)
            {
                ActionDefinitionAttribute actionDefinitionAttribute = methodInfo.GetCustomAttribute<ActionDefinitionAttribute>();
                if (actionDefinitionAttribute != null)
                    applicationGuids.Add(actionDefinitionAttribute.Name);
            }

            return applicationGuids.ToArray();
        }
    }
}
