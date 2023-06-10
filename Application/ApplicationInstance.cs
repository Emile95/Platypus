using Application.Action;
using Application.Exceptions;
using Persistance;
using PlatypusApplicationFramework;
using PlatypusApplicationFramework.Action;
using Utils;

namespace Application
{
    public class ApplicationInstance
    {
        private readonly ApplicationInstaller _applicationInstaller;
        private readonly ApplicationResolver _applicationResolver;
        
        private readonly ApplicationsHandler _applicationsHandler;
        private readonly ApplicationActionsHandler _applicationActionsHandler;


        public ApplicationInstance()
        {
            ApplicationRepository applicationRepository = new ApplicationRepository();
            ApplicationActionRepository applicationActionRepository = new ApplicationActionRepository();

            _applicationsHandler = new ApplicationsHandler();
            _applicationActionsHandler = new ApplicationActionsHandler(applicationActionRepository);

            _applicationInstaller = new ApplicationInstaller(
                applicationRepository,
                applicationActionRepository,
                _applicationsHandler
            );
            _applicationResolver = new ApplicationResolver(_applicationActionsHandler);
        }

        public void LoadConfiguration()
        {

        }

        public void InstallApplication(string dllFilePath)
        {
            List<string> newPaths = InstallApplicationPluginFromDll(dllFilePath);
            LoadApplicationPluginFromDll(newPaths[0], newPaths[1]);
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
                LoadApplicationPluginFromDll(directoryInfo.Name, dllFilePath);
            }
                
            return _applicationsHandler.Applications.Count;
        }

        private void LoadApplicationPluginFromDll(string directoryPath, string dllFilePath)
        {
            PlatypusApplicationBase applicationFromDll = PluginResolver.InstanciateImplementationFromDll<PlatypusApplicationBase>(dllFilePath);
            _applicationsHandler.AddApplication(directoryPath, applicationFromDll);
            _applicationResolver.ResolvePlatypusApplication(applicationFromDll, directoryPath);
        }

        private List<string> InstallApplicationPluginFromDll(string dllFilePath)
        {
            PlatypusApplicationBase applicationFromDll = PluginResolver.InstanciateImplementationFromDll<PlatypusApplicationBase>(dllFilePath);
            return _applicationInstaller.InstallPlatypusApplication(applicationFromDll, dllFilePath);
        }

        public ApplicationActionResult RunAction(RunActionParameter runActionParameter)
        {
            if (_applicationActionsHandler.ApplicationActions.ContainsKey(runActionParameter.Guid) == false)
                throw new ApplicationActionInexistantException(runActionParameter.Guid);

            ApplicationActionEnvironmentBase env = _applicationActionsHandler.CreateStartActionEnvironment(runActionParameter.Guid);

            return _applicationActionsHandler.RunAction(runActionParameter, env);
        }

        public void CancelRunningApplicationAction(string guid)
        {
            _applicationActionsHandler.CancelRunningAction(guid);
        }

        public IEnumerable<RunningApplicationAction> GetRunningApplicationActions()
        {
            return _applicationActionsHandler.GetRunningApplicationActions();
        }
    }
}
