using Core.ApplicationAction.Run;
using Persistance.Repository;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusApplicationFramework.Core.ApplicationAction;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;
using Utils.GuidGeneratorHelper;
using Core.Event;
using PlatypusApplicationFramework.Core.Event;
using Persistance.Entity;
using Persistance;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ServerFunctionParameter;

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

            ApplicationAction applicationAction = new ApplicationAction(application,actionDefinition,methodInfo,actionGuid);
            _applicationActions.Add(actionGuid, applicationAction);
        }

        public void RemoveAction(string actionGuid)
        {
            foreach(ApplicationActionRun applicationActionRun in _applicationActionRuns.Values)
            {
                if (applicationActionRun.ActionGuid == actionGuid)
                    CancelRunningActionByGuid(applicationActionRun.Guid);
            }
            _applicationActions.Remove(actionGuid);
        }

        public bool HasActionWithGuid(string actionGuid)
        {
            return _applicationActions.ContainsKey(actionGuid);
        }

        public void ReRunStopedApplicationActions(ApplicationRepository applicationRepository)
        {
            List<RunningApplicationActionEntity> runningActions = _applicationActionRepository.LoadRunningActions();
            foreach(RunningApplicationActionEntity runningAction in runningActions)
            {
                ApplicationActionEnvironmentBase env = CreateStartActionEnvironment(runningAction.ActionGuid);
                env.ApplicationRepository = applicationRepository;

                RunAction(new ApplicationActionRunParameter() { 
                    Async = true,
                    ActionParameters = runningAction.ActionParameters,
                    Guid = runningAction.ActionGuid,
                }, env, runningAction);
            }
        }

        public ApplicationActionRunResult RunAction(ApplicationActionRunParameter runActionParameter, ApplicationActionEnvironmentBase env, RunningApplicationActionEntity savedRunningAction = null)
        {
            ApplicationAction applicationAction = _applicationActions[runActionParameter.Guid];

            try
            {
                applicationAction.ResolveActionParameter(env, runActionParameter.ActionParameters);
            } catch (Exception ex)
            {
                return new ApplicationActionRunResult
                {
                    Status = ApplicationActionRunResultStatus.Failed,
                    Message = ex.Message,
                };
            }

            ActionRunEventHandlerEnvironment actionRunEventHandlerEnvironment = new ActionRunEventHandlerEnvironment();
            actionRunEventHandlerEnvironment.ApplicationActionInfo = applicationAction.GetInfo();

            ApplicationActionRunResult result = BeforeApplicationActionRun(applicationAction, actionRunEventHandlerEnvironment);
            if (result is not null) return result;

            ApplicationActionRun applicationActionRun = CreateApplicationActionRun(runActionParameter, env, savedRunningAction);

            string configFilePath = _applicationActionRepository.GetRunActionLogFilePath(runActionParameter.Guid, applicationActionRun.RunNumber);

            applicationActionRun.StartRun(applicationAction, runActionParameter, () => {
                ApplicationActionRunCallBack(applicationActionRun, actionRunEventHandlerEnvironment);
            });

            if (runActionParameter.Async)
            {
                
                string message = Common.Utils.GetString("NewApplicationActionStarted");
                return new ApplicationActionRunResult()
                {
                    Message = message,
                    Status = ApplicationActionRunResultStatus.Started
                };
            }
                
            applicationActionRun.Task.Wait();

            return applicationActionRun.Result;
        }

        public void CancelRunningAction(CancelRunningActionParameter parameter)
        {
            CancelRunningActionByGuid(parameter.Guid);
        }

        private void CancelRunningActionByGuid(string guid)
        {
            if (_applicationActionRuns.ContainsKey(guid) == false) throw new Exception($"No action with guid '{guid}' is runiing");

            CancelRunningActionEventHandlerEnvironment eventEnv = new CancelRunningActionEventHandlerEnvironment()
            {
                RunningActionGuid = guid
            };

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.BeforeCancelApplicationRun, eventEnv, (exception) => throw exception);

            ApplicationActionRun run = _applicationActionRuns[guid];
            _applicationActionRuns.Remove(guid);

            _applicationActionRepository.RemoveRunningAction(guid);

            run.Cancel();

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.AfterCancelApplicationRun, eventEnv, (exception) => throw exception);
        }

        public IEnumerable<ApplicationActionRunInfo> GetRunningApplicationActionInfos()
        {
            return _applicationActionRuns.Values.Select(x => x.GetInfo());
                
        }

        public IEnumerable<ApplicationActionInfo> GetApplicationActionInfos()
        {
            return _applicationActions.Values.Select(x => x.GetInfo());
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

        private ApplicationActionRun CreateApplicationActionRun(ApplicationActionRunParameter runActionParameter, ApplicationActionEnvironmentBase env, RunningApplicationActionEntity savedRunningAction = null)
        {

            string applicationActionRunGUID;
            int runNumber;

            if(savedRunningAction == null)
            {
                applicationActionRunGUID = GuidGenerator.GenerateFromEnumerable(_applicationActionRuns.Keys);
                runNumber = CreateNewActionRunNumber(runActionParameter.Guid);

                _applicationActionRepository.SaveRunningAction(new RunningApplicationActionEntity()
                {
                    ActionGuid = runActionParameter.Guid,
                    Guid = applicationActionRunGUID,
                    ActionParameters = runActionParameter.ActionParameters,
                    RunNumber = runNumber
                });
            } else
            {
                applicationActionRunGUID = savedRunningAction.Guid;
                runNumber = savedRunningAction.RunNumber;
            }

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

        private void ApplicationActionRunCallBack(ApplicationActionRun run, ActionRunEventHandlerEnvironment eventEnv)
        {
            if (run.Env.ActionCancelled)
                return;

            _applicationActionRuns.Remove(run.Guid);
            _applicationActionRepository.RemoveRunningAction(run.Guid);

            eventEnv.ApplicationActionResult = run.Result;
            eventEnv.ApplicationActionRunInfo = run.GetInfo();

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.AfterApplicationActionRun, eventEnv, (exception) => {
                if (run.Result.Status != ApplicationActionRunResultStatus.Failed)
                {
                    run.Result.Status = ApplicationActionRunResultStatus.Failed;
                    run.Result.Message = exception.Message;
                }
                return null;
            });

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

        private ApplicationActionRunResult BeforeApplicationActionRun(ApplicationAction applicationAction, ActionRunEventHandlerEnvironment eventEnv)
        {
            eventEnv.ApplicationActionInfo = applicationAction.GetInfo();

            _eventsHandler.RunEventHandlers(EventHandlerType.BeforeApplicationActionRun, eventEnv, (exception) => {
                return new ApplicationActionRunResult()
                {
                    Status = ApplicationActionRunResultStatus.Failed,
                    Message = exception.Message,
                };
            });

            return null;
        }
    }
}
