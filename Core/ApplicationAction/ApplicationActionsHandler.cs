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
using Persistance.Entity;
using Core.Exceptions;

namespace Core.ApplicationAction
{
    public class ApplicationActionsHandler
    {
        private readonly Dictionary<string, ApplicationAction> _applicationActions;
        private readonly Dictionary<string, ApplicationActionRun> _applicationActionRuns;
        private readonly ApplicationActionRepository _applicationActionRepository;
        private readonly EventsHandler _eventsHandler;

        public ApplicationActionsHandler(
            ApplicationActionRepository applicationActionRepository,
            EventsHandler eventsHandler
        )
        {
            _applicationActions = new Dictionary<string, ApplicationAction>();
            _applicationActionRuns = new Dictionary<string, ApplicationActionRun>();
            _applicationActionRepository = applicationActionRepository;
            _eventsHandler = eventsHandler;
        }

        public void AddAction(PlatypusApplicationBase application, string applicationGuid, ActionDefinitionAttribute actionDefinition, MethodInfo methodInfo)
        {
            string actionGuid = actionDefinition.Name+applicationGuid;
            if (_applicationActions.ContainsKey(actionGuid)) return;

            ApplicationAction applicationAction = new ApplicationAction(application,actionDefinition,methodInfo);
            _applicationActions.Add(actionGuid, applicationAction);
        }

        public bool HasActionWithGuid(string actionGuid)
        {
            return _applicationActions.ContainsKey(actionGuid);
        }

        public ApplicationActionResult RunAction(ApplicationActionRunParameter runActionParameter, ApplicationActionEnvironmentBase env)
        {
            ApplicationActionResult result = BeforeApplicationActionRun();
            if (result is not null) return result;

            ApplicationActionRun applicationActionRun = CreateApplicationActionRun(runActionParameter, env);

            string configFilePath = _applicationActionRepository.GetRunActionLogFilePath(runActionParameter.Guid, applicationActionRun.RunNumber);
            env.ActionLoggers.CreateLogger<ApplicationActionRunFileLogger>(configFilePath);

            ApplicationAction applicationAction = _applicationActions[runActionParameter.Guid];

            applicationActionRun.StartRun(applicationAction, runActionParameter, (guid) => {
                ApplicationActionRunCallBack(applicationActionRun, guid);
            });

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
            if (_applicationActionRuns.ContainsKey(guid) == false)
                return;
            _applicationActionRuns[guid].Cancel();
        }

        public IEnumerable<ApplicationActionRunInfo> GetRunningApplicationActionInfos()
        {
            return _applicationActionRuns.Values.Select(x => x.GetInfo());
                
        }

        public ApplicationActionEnvironmentBase CreateStartActionEnvironment(string actionName)
        {
            return _applicationActions[actionName].CreateStartActionEnvironment();
        }

        private int CreateNewActionRunNumber(string applicationRunGuid)
        {
            string actionRunsFilePath = ApplicationPaths.GetActionRunsDirectoryPath(applicationRunGuid);
            int runNumber = _applicationActionRepository.GetAndIncrementActionRunNumberByBasePath(actionRunsFilePath);
            _applicationActionRepository.SaveActionRunByBasePath(actionRunsFilePath, runNumber);
            return runNumber;
        }

        private ApplicationActionRun CreateApplicationActionRun(ApplicationActionRunParameter runActionParameter, ApplicationActionEnvironmentBase env)
        {
            string applicationActionRunGUID = GuidGenerator.GenerateFromEnumerable(_applicationActionRuns.Keys);
            int runNumber = CreateNewActionRunNumber(runActionParameter.Guid);

            ApplicationActionRun applicationActionRun = new ApplicationActionRun()
            {
                ActionGuid = runActionParameter.Guid,
                Guid = applicationActionRunGUID,
                RunNumber = runNumber,
                Env = env
            };

            _applicationActionRuns.Add(
                applicationActionRunGUID,
                applicationActionRun
            );

            return applicationActionRun;
        }

        private void ApplicationActionRunCallBack(ApplicationActionRun run, string applicationRunGuid)
        {
            _applicationActionRuns.Remove(applicationRunGuid);

            if(run.Result.Status != ApplicationActionResultStatus.Failed &&
               run.Result.Status != ApplicationActionResultStatus.Canceled)
            {
                EventHandlerEnvironment eventEnv = new EventHandlerEnvironment();
                try
                {
                    _eventsHandler.RunEventHandlers(EventHandlerType.AfterApplicationActionRun, eventEnv);
                }
                catch (EventHandlerException ex)
                {
                    run.Result.Status = ApplicationActionResultStatus.Failed;
                    run.Result.Message = ex.Message;
                }
            }

            _applicationActionRepository.SaveActionRunResult(
                run.ActionGuid,
                run.RunNumber,
                new ApplicationActionResultEntity()
                {
                    Status = run.Result.Status.ToString(),
                    Message = run.Result.Message
                }
            );
        }

        private ApplicationActionResult BeforeApplicationActionRun()
        {
            EventHandlerEnvironment eventEnv = new EventHandlerEnvironment();
            try
            {
                _eventsHandler.RunEventHandlers(EventHandlerType.BeforeApplicationActionRun, eventEnv);
            }
            catch (EventHandlerException ex)
            {
                return new ApplicationActionResult()
                {
                    Status = ApplicationActionResultStatus.Failed,
                    Message = ex.Message,
                };
            }

            return null;
        }
    }
}
