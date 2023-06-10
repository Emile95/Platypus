using Persistance;
using PlatypusApplicationFramework;
using PlatypusApplicationFramework.ApplicationAction;
using System.Reflection;
using Utils;

namespace Application
{
    internal class ApplicationInstaller
    {
        private readonly ApplicationRepository _applicationRepository;
        private readonly ApplicationActionRepository _applicationActionRepository;
        private readonly ApplicationResolver _applicationResolver;

        public ApplicationInstaller(
            ApplicationRepository applicationRepository,
            ApplicationActionRepository applicationActionRepository,
            ApplicationResolver applicationResolver
        )
        {
            _applicationRepository = applicationRepository;
            _applicationActionRepository = applicationActionRepository;
            _applicationResolver = applicationResolver;
        }

        public void InstallApplication(string newGuid, string dllFilePath)
        {
            PlatypusApplicationBase platypusApplication = PluginResolver.InstanciateImplementationFromDll<PlatypusApplicationBase>(dllFilePath);
            _applicationRepository.SaveApplication(newGuid, dllFilePath);

            Type type = platypusApplication.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
                InstallActions(newGuid, method);

            _applicationResolver.ResolvePlatypusApplication(platypusApplication, newGuid);
        }

        private void InstallActions(string applicationGuid, MethodInfo methodInfo)
        {
            ActionDefinitionAttribute actionDefinition = methodInfo.GetCustomAttribute<ActionDefinitionAttribute>();
            if (actionDefinition == null) return;

            _applicationActionRepository.SaveAction(actionDefinition.Name+ applicationGuid);
        }
    }
}
