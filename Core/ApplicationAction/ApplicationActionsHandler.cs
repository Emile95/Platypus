using Core.ApplicationAction.Run;
using Persistance;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusApplicationFramework.Core.ApplicationAction;
using PlatypusApplicationFramework.Core.ApplicationAction.Logger;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;
using Utils.GuidGeneratorHelper;
using Core.Event;
using PlatypusApplicationFramework.Core.Event;

namespace Core.ApplicationAction
{
    public class ApplicationActionsHandler
    {
        public Dictionary<string, ApplicationAction> ApplicationActions { get; private set; }
        public Dictionary<string, ApplicationActionRun> ApplicationActionRuns { get; private set; }

        private readonly ApplicationActionRepository _applicationActionRepository;
        private readonly EventsHandler _eventHandlers;

        public ApplicationActionsHandler(
            ApplicationActionRepository applicationActionRepository,
            EventsHandler eventHandlers
        )
        {
            ApplicationActions = new Dictionary<string, ApplicationAction>();
            ApplicationActionRuns = new Dictionary<string, ApplicationActionRun>();

            _applicationActionRepository = applicationActionRepository;
            _eventHandlers = eventHandlers;
        }

        public void AddAction(PlatypusApplicationBase application, string applicationGuid, ActionDefinitionAttribute actionDefinition, MethodInfo methodInfo)
        {
            string actionGuid = actionDefinition.Name+applicationGuid;
            if (ApplicationActions.ContainsKey(actionGuid)) return;

            ApplicationAction applicationAction = new ApplicationAction(application,actionDefinition,methodInfo);
            ApplicationActions.Add(actionGuid, applicationAction);
        }

        public ApplicationActionResult RunAction(ApplicationActionRunParameter runActionParameter, ApplicationActionEnvironmentBase env)
        {
            string actionRunsFilePath = ApplicationPaths.GetActionRunsDirectoryPath(runActionParameter.Guid);
            int runNumber = _applicationActionRepository.GetAndIncrementActionRunNumberByBasePath(actionRunsFilePath);
            _applicationActionRepository.SaveActionRunByBasePath(actionRunsFilePath, runNumber);

            ApplicationAction action = ApplicationActions[runActionParameter.Guid];

            string applicationActionRunGUID = GuidGenerator.GenerateFromEnumerable(ApplicationActionRuns.Keys);

            string configFilePath = _applicationActionRepository.GetRunActionLogFilePath(runActionParameter.Guid, runNumber);
            env.ActionLoggers.CreateLogger<ApplicationActionRunFileLogger>(configFilePath);

            ApplicationActionRun applicationActionRun = new ApplicationActionRun(_applicationActionRepository, (guid) => {
                ApplicationActionRuns.Remove(guid);
                EventHandlerEnvironment eventEnv = new EventHandlerEnvironment();
                _eventHandlers.RunEventHandlers(EventHandlerType.AfterApplicationActionRun, eventEnv);
            });

            applicationActionRun.ActionGuid = runActionParameter.Guid;
            applicationActionRun.Guid = applicationActionRunGUID;
            applicationActionRun.RunNumber = runNumber;
            applicationActionRun.Env = env;

            ApplicationActionRuns.Add(
                applicationActionRunGUID,
                applicationActionRun
            );

            applicationActionRun.StartRun(action, runActionParameter);

            if (runActionParameter.Async)
                return new ApplicationActionResult() { 
                    Message = $"new action has been started",
                    Status = ApplicationActionResultStatus.Started 
                };

            applicationActionRun.Task.Wait();

            return applicationActionRun.Result;
        }

        public void CancelRunningAction(string guid)
        {
            if (ApplicationActionRuns.ContainsKey(guid) == false)
                return;
            ApplicationActionRuns[guid].Cancel();
        }

        public IEnumerable<ApplicationActionRunInfo> GetRunningApplicationActionInfos()
        {
            return ApplicationActionRuns.Values.Select(x => x.GetInfo());
                
        }

        public ApplicationActionEnvironmentBase CreateStartActionEnvironment(string actionName)
        {
            return ApplicationActions[actionName].CreateStartActionEnvironment();
        }
        
    }
}
