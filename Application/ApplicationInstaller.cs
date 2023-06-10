using Application.Action;
using Persistance;
using PlatypusApplicationFramework;
using PlatypusApplicationFramework.Action;
using System.Reflection;
using Utils.GuidGeneratorHelper;

namespace Application
{
    internal class ApplicationInstaller
    {
        private readonly ApplicationRepository _applicationRepository;
        private readonly ApplicationsHandler _applicationsHandler;
        private readonly ApplicationActionRepository _applicationActionRepository;

        public ApplicationInstaller(
            ApplicationRepository applicationRepository,
            ApplicationActionRepository applicationActionRepository,
            ApplicationsHandler applicationsHandler
        )
        {
            _applicationRepository = applicationRepository;
            _applicationActionRepository = applicationActionRepository;
            _applicationsHandler = applicationsHandler;
        }

        public List<string> InstallPlatypusApplication(PlatypusApplicationBase platypusApplication, string dllFilePath)
        {
            List<string> newPaths = new List<string>();
            string newGuid = GuidGenerator.GenerateFromEnumerable(_applicationsHandler.Applications.Keys);
            string newDllFilePath = _applicationRepository.SaveApplication(platypusApplication, newGuid, dllFilePath);

            Type type = platypusApplication.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
                InstallActions(newGuid, method);

            newPaths.Add(newGuid);
            newPaths.Add(newDllFilePath);
            return newPaths;
        }

        private void InstallActions(string applicationGuid, MethodInfo methodInfo)
        {
            ActionDefinitionAttribute actionDefinition = methodInfo.GetCustomAttribute<ActionDefinitionAttribute>();
            if (actionDefinition == null) return;

            _applicationActionRepository.SaveAction(actionDefinition.Name+ applicationGuid);
        }
    }
}
