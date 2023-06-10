using PlatypusApplicationFramework;
using PlatypusApplicationFramework.Action;
using System.Reflection;
using Utils.GuidGeneratorHelper;

namespace Application.Action
{
    public class ApplicationActionsHandler
    {
        public Dictionary<string, ApplicationAction> ApplicationActions { get; private set; }
        public Dictionary<string, RunningApplicationAction> RunningApplicationActions { get; private set; }

        private readonly ApplicationActionRepository _applicationActionRepository;
        

        public ApplicationActionsHandler()
        {
            ApplicationActions = new Dictionary<string, ApplicationAction>();
            _applicationActionRepository = new ApplicationActionRepository();
            RunningApplicationActions = new Dictionary<string, RunningApplicationAction>();
        }

        public void AddAction(PlatypusApplicationBase application, ActionDefinitionAttribute actionDefinition, MethodInfo methodInfo)
        {
            if (ApplicationActions.ContainsKey(actionDefinition.Name)) return;
            ApplicationAction applicationAction = new ApplicationAction(application,actionDefinition,methodInfo);
            ApplicationActions.Add(actionDefinition.Name, applicationAction);
        }

        public ApplicationActionResult RunAction(RunActionParameter runActionParameter, ApplicationActionEnvironmentBase env)
        {
            ApplicationAction action = ApplicationActions[runActionParameter.Name];

            string runningActionGUID = GuidGenerator.GenerateFromEnumerable(RunningApplicationActions.Keys);

            RunningApplicationAction runningApplicationAction = new RunningApplicationAction(runningActionGUID, _applicationActionRepository, action, runActionParameter, env, RunningApplicationActions);

            RunningApplicationActions.Add(
                runningActionGUID,
                runningApplicationAction
            );

            if(runActionParameter.Async)
                return new ApplicationActionResult() { 
                    Message = $"new action of type '{runActionParameter.Name}' has been started",
                    Status = ApplicationActionResultStatus.Started 
                };

            runningApplicationAction.Task.Wait();

            return runningApplicationAction.Result;
        }

        public void CancelRunningAction(string guid)
        {
            if (RunningApplicationActions.ContainsKey(guid) == false)
                return;
            RunningApplicationActions[guid].Cancel();
        }

        public IEnumerable<RunningApplicationAction> GetRunningApplicationActions()
        {
            return RunningApplicationActions.Values;
        }

        public ApplicationActionEnvironmentBase CreateStartActionEnvironment(string actionName)
        {
            return ApplicationActions[actionName].CreateStartActionEnvironment();
        }

        
    }
}
