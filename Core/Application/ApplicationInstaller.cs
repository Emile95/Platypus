using Persistance;
using Persistance.Entity;
using PlatypusApplicationFramework.Core.Application;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;
using Utils;

namespace Core.Application
{
    internal class ApplicationInstaller
    {
        private readonly ApplicationRepository _applicationRepository;
        private readonly ApplicationActionRepository _applicationActionRepository;

        public ApplicationInstaller(
            ApplicationRepository applicationRepository,
            ApplicationActionRepository applicationActionRepository
        )
        {
            _applicationRepository = applicationRepository;
            _applicationActionRepository = applicationActionRepository;
        }

        public PlatypusApplicationBase InstallApplication(string newGuid, string dllFilePath)
        {
            PlatypusApplicationBase platypusApplication = PluginResolver.InstanciateImplementationFromDll<PlatypusApplicationBase>(dllFilePath);
            _applicationRepository.SaveApplication(new ApplicationEntity()
            {
                Guid = newGuid,
                DllFilePath = dllFilePath
            });

            Type type = platypusApplication.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
                InstallActions(newGuid, method);

            ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
            env.ApplicationRepository = _applicationRepository;
            env.ApplicationGuid = newGuid;

            platypusApplication.Install(env);

            return platypusApplication;
        }

        private void InstallActions(string applicationGuid, MethodInfo methodInfo)
        {
            ActionDefinitionAttribute actionDefinition = methodInfo.GetCustomAttribute<ActionDefinitionAttribute>();
            if (actionDefinition == null) return;

            _applicationActionRepository.SaveAction(actionDefinition.Name+ applicationGuid);
        }
    }
}
