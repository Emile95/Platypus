using Common.Logger;
using Core.Application;
using Core.ApplicationAction;
using Core.ApplicationAction.Run;
using Logging;
using Persistance;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusApplicationFramework.Core.ApplicationAction;

namespace Core
{
    public class ServerInstance
    {
        private readonly ApplicationsHandler _applicationsHandler;
        private readonly ApplicationActionsHandler _applicationActionsHandler;
        private readonly ApplicationRepository _applicationRepository;
        private readonly LoggerManager _loggerManager;

        public ServerInstance()
        {
            ApplicationActionRepository applicationActionRepository = new ApplicationActionRepository();
            
            _applicationActionsHandler = new ApplicationActionsHandler(
                applicationActionRepository
            );

            _applicationRepository = new ApplicationRepository();

            _applicationsHandler = new ApplicationsHandler(
                _applicationRepository,
                applicationActionRepository,
                _applicationActionsHandler
            );

            _loggerManager = new LoggerManager();
            _loggerManager.CreateLogger<PlatypusServerConsoleLogger>();
        }

        public void LoadConfiguration()
        {

        }

        public void LoadApplications()
        {
            _applicationsHandler.LoadApplications();
        }

        public void InstallApplication(string dllFilePath)
        {
            _applicationsHandler.InstallApplication(dllFilePath);
        }

        public ApplicationActionResult RunAction(ApplicationActionRunParameter runActionParameter)
        {
            if (_applicationActionsHandler.ApplicationActions.ContainsKey(runActionParameter.Guid) == false)
                return new ApplicationActionResult() { 
                    Message = $"action with guid {runActionParameter.Guid} non existant",
                    Status = ApplicationActionResultStatus.Failed,
                };

            ApplicationActionEnvironmentBase env = _applicationActionsHandler.CreateStartActionEnvironment(runActionParameter.Guid);
            env.ApplicationRepository = _applicationRepository;
            env.ActionLoggers = new LoggerManager();

            return _applicationActionsHandler.RunAction(runActionParameter, env);
        }

        public void CancelRunningApplicationAction(string guid)
        {
            _applicationActionsHandler.CancelRunningAction(guid);
        }

        public IEnumerable<ApplicationActionRunInfo> GetRunningApplicationActions()
        {
            return _applicationActionsHandler.GetRunningApplicationActionInfos();
        }
    }
}
