using Application.Action;
using Application.Exceptions;
using Persistance;
using PlatypusApplicationFramework.Action;

namespace Application
{
    public class ApplicationInstance
    {
        private readonly ApplicationsHandler _applicationsHandler;
        private readonly ApplicationActionsHandler _applicationActionsHandler;

        public ApplicationInstance()
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

        public IEnumerable<RunningApplicationAction> GetRunningApplicationActions()
        {
            return _applicationActionsHandler.GetRunningApplicationActions();
        }
    }
}
