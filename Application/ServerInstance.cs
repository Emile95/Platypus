using Core.Application;
using Core.ApplicationAction;
using Core.ApplicationAction.Run;
using Persistance;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusApplicationFramework.ApplicationAction;

namespace Core
{
    public class ServerInstance
    {
        private readonly ApplicationsHandler _applicationsHandler;
        private readonly ApplicationActionsHandler _applicationActionsHandler;

        public ServerInstance()
        {
            ApplicationActionRepository applicationActionRepository = new ApplicationActionRepository();

            _applicationActionsHandler = new ApplicationActionsHandler(applicationActionRepository);

            _applicationsHandler = new ApplicationsHandler(
                applicationActionRepository,
                _applicationActionsHandler
            );
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

        public ApplicationActionResult RunAction(RunActionParameter runActionParameter)
        {
            if (_applicationActionsHandler.ApplicationActions.ContainsKey(runActionParameter.Guid) == false)
                return new ApplicationActionResult() { 
                    Message = $"action with guid {runActionParameter.Guid} non existant",
                    Status = ApplicationActionResultStatus.Failed,
                };

            ApplicationActionEnvironmentBase env = _applicationActionsHandler.CreateStartActionEnvironment(runActionParameter.Guid);

            return _applicationActionsHandler.RunAction(runActionParameter, env);
        }

        public void CancelRunningApplicationAction(string guid)
        {
            _applicationActionsHandler.CancelRunningAction(guid);
        }

        public IEnumerable<RunningApplicationActionInfo> GetRunningApplicationActions()
        {
            return _applicationActionsHandler.GetRunningApplicationActionInfos();
        }
    }
}
