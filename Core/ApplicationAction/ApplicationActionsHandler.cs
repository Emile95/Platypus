using Core.ApplicationAction.Run;
using Logging;
using Persistance;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusApplicationFramework.ApplicationAction;
using PlatypusApplicationFramework.ApplicationAction.Logger;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;
using Utils.GuidGeneratorHelper;

namespace Core.ApplicationAction
{
    public class ApplicationActionsHandler
    {
        public Dictionary<string, ApplicationAction> ApplicationActions { get; private set; }
        public Dictionary<string, RunningApplicationAction> RunningApplicationActions { get; private set; }

        private readonly ApplicationActionRepository _applicationActionRepository;

        public ApplicationActionsHandler(
            ApplicationActionRepository applicationActionRepository
        )
        {
            ApplicationActions = new Dictionary<string, ApplicationAction>();
            _applicationActionRepository = applicationActionRepository;
            RunningApplicationActions = new Dictionary<string, RunningApplicationAction>();
        }

        public void AddAction(PlatypusApplicationBase application, string applicationGuid, ActionDefinitionAttribute actionDefinition, MethodInfo methodInfo)
        {
            string actionGuid = actionDefinition.Name+applicationGuid;
            if (ApplicationActions.ContainsKey(actionGuid)) return;
            ApplicationAction applicationAction = new ApplicationAction(application,actionDefinition,methodInfo);
            ApplicationActions.Add(actionGuid, applicationAction);
        }

        public ApplicationActionResult RunAction(RunActionParameter runActionParameter, ApplicationActionEnvironmentBase env)
        {
            string actionRunsFilePath = ApplicationPaths.GetActionRunsDirectoryPath(runActionParameter.Guid);
            int runNumber = _applicationActionRepository.GetAndIncrementActionRunNumberByBasePath(actionRunsFilePath);
            _applicationActionRepository.SaveActionRunByBasePath(actionRunsFilePath, runNumber);

            ApplicationAction action = ApplicationActions[runActionParameter.Guid];

            string runningActionGUID = GuidGenerator.GenerateFromEnumerable(RunningApplicationActions.Keys);

            string configFilePath = _applicationActionRepository.GetRunActionLogFilePath(runActionParameter.Guid, runNumber);
            env.ActionLoggers.CreateLogger<ApplicationActionRunFileLogger>(configFilePath);

            RunningApplicationAction runningApplicationAction = new RunningApplicationAction(runActionParameter.Guid, runNumber, _applicationActionRepository, action, runActionParameter, env, RunningApplicationActions);

            RunningApplicationActions.Add(
                runningActionGUID,
                runningApplicationAction
            );

            if(runActionParameter.Async)
                return new ApplicationActionResult() { 
                    Message = $"new action has been started",
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

        public IEnumerable<RunningApplicationActionInfo> GetRunningApplicationActionInfos()
        {
            return RunningApplicationActions.Values.Select(x => x.GetInfo());
                
        }

        public ApplicationActionEnvironmentBase CreateStartActionEnvironment(string actionName)
        {
            return ApplicationActions[actionName].CreateStartActionEnvironment();
        }
        
    }
}
