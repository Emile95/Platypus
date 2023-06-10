using Application.Action;
using Application.Exceptions;
using PlatypusApplicationFramework;
using PlatypusApplicationFramework.Action;
using System.Reflection;
using Utils;

namespace Application
{
    public class ApplicationInstance
    {
        private readonly ApplicationInstaller _applicationInstaller;
        private readonly ApplicationResolver _applicationResolver;
        private readonly List<PlatypusApplicationBase> _applications;
        private readonly ApplicationActionsHandler _applicationActionsHandler;

        public ApplicationInstance()
        {
            _applicationInstaller = new ApplicationInstaller();
            _applicationResolver = new ApplicationResolver(this);
            _applications = new List<PlatypusApplicationBase>();
            _applicationActionsHandler = new ApplicationActionsHandler();
        }

        public void LoadConfiguration()
        {

        }

        public void InstallApplication(string dllFilePath)
        {

        }

        public void InstallApplication(byte[] dll)
        {

        }

        public int LoadApplications()
        {
            if (Directory.Exists(ApplicationPaths.APPLICATIONSDIRECTORYPATHS) == false) return 0;

            string[] applicationDirectoriesPath = Directory.GetDirectories(ApplicationPaths.APPLICATIONSDIRECTORYPATHS);
            foreach(string applicationDirectoryPath in applicationDirectoriesPath)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(applicationDirectoryPath);
                string dllFilePath = ApplicationPaths.GetApplicationDllFilePath(directoryInfo.Name);
                LoadPluginsFromDll(dllFilePath);
            }
                
            return _applications.Count;
        }

        private void LoadPluginsFromDll(string dllFilePath)
        {
            PlatypusApplicationBase applicationFromDll = PluginResolver.InstanciateImplementationFromDll<PlatypusApplicationBase>(dllFilePath);
            _applications.Add(applicationFromDll);
            _applicationResolver.ResolvePlatypusApplication(applicationFromDll);
        }

        public ApplicationActionResult RunAction(RunActionParameter runActionParameter)
        {
            if (_applicationActionsHandler.ApplicationActions.ContainsKey(runActionParameter.Name) == false)
                throw new ApplicationActionInexistantException(runActionParameter.Name);

            ApplicationActionEnvironmentBase env = _applicationActionsHandler.CreateStartActionEnvironment(runActionParameter.Name);

            return _applicationActionsHandler.RunAction(runActionParameter, env); ;
        }

        public void CancelRunningApplicationAction(string guid)
        {
            _applicationActionsHandler.CancelRunningAction(guid);
        }

        public IEnumerable<RunningApplicationAction> GetRunningApplicationActions()
        {
            return _applicationActionsHandler.GetRunningApplicationActions();
        }

        public void AddAction(PlatypusApplicationBase platypusApplication, ActionDefinitionAttribute actionDefinition, MethodInfo methodInfo)
        {
            _applicationActionsHandler.AddAction(platypusApplication, actionDefinition, methodInfo);
        }
    }
}
