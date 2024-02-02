using Core.ApplicationAction.Run;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusFramework.Core.ApplicationAction;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.ApplicationAction;
using System.Reflection;
using Core.Event;
using PlatypusFramework.Core.Event;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusUtils;
using Core.Ressource;
using PlatypusRepository;
using Core.Persistance.Entity;
using Core.Persistance.Repository;
using Core.Abstract;

namespace Core.ApplicationAction
{
    internal class ApplicationActionsHandler : 
        IApplicationAttributeMethodResolver<ActionDefinitionAttribute>
    {
        private readonly Dictionary<string, ApplicationAction> _applicationActions;
        private readonly Dictionary<string, ApplicationActionRun> _applicationActionRuns;
       
        private readonly IRepositoryAddOperator<RunningApplicationActionEntity> _runningApplicationActionRepositoryAddOperator;
        private readonly IRepositoryRemoveOperator<string> _runningApplicationActionRepositoryRemoveOperator;
        private readonly IRepositoryConsumeOperator<RunningApplicationActionEntity> _runningApplicationActionRepositoryConsumeOperator;

        private readonly EventsHandler _eventsHandler;

        internal ApplicationActionsHandler(
            RunningApplicationActionRepository runningApplicationActionRepository,
            EventsHandler eventsHandler
        )
        {
            _applicationActions = new Dictionary<string, ApplicationAction>();
            _applicationActionRuns = new Dictionary<string, ApplicationActionRun>();

            _runningApplicationActionRepositoryAddOperator = runningApplicationActionRepository;
            _runningApplicationActionRepositoryRemoveOperator = runningApplicationActionRepository;
            _runningApplicationActionRepositoryConsumeOperator = runningApplicationActionRepository;

            _eventsHandler = eventsHandler;
        }

        public void Resolve(PlatypusApplicationBase application, ActionDefinitionAttribute attribute, MethodInfo method)
        {
            string actionGuid = attribute.Name + application.ApplicationGuid;
            if (_applicationActions.ContainsKey(actionGuid)) return;

            ApplicationAction applicationAction = new ApplicationAction(application, attribute, method, actionGuid);
            _applicationActions.Add(actionGuid, applicationAction);
        }

        internal void RemoveAction(string actionGuid)
        {
            foreach(ApplicationActionRun applicationActionRun in _applicationActionRuns.Values)
            {
                if (applicationActionRun.ActionGuid == actionGuid)
                    CancelRunningActionByGuid(applicationActionRun.Guid);
            }
            _applicationActions.Remove(actionGuid);
        }

        internal bool HasActionWithGuid(string actionGuid)
        {
            return _applicationActions.ContainsKey(actionGuid);
        }

        internal ApplicationActionRunResult RunAction(ApplicationActionRunParameter runActionParameter, ApplicationActionEnvironmentBase env)
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

            ApplicationActionRun applicationActionRun = applicationAction.CreateApplicationActionRun(runActionParameter, env);

            applicationActionRun.StartRun(applicationAction, runActionParameter, () => {
                ApplicationActionRunCallBack(applicationActionRun, actionRunEventHandlerEnvironment);
            });

            _applicationActionRuns.Add(
                applicationActionRun.Guid,
                applicationActionRun
            );

            _runningApplicationActionRepositoryAddOperator.Add(new RunningApplicationActionEntity()
            {
                Guid = applicationActionRun.Guid,
                ActionGuid = applicationActionRun.ActionGuid,
                Parameters = runActionParameter.ActionParameters
            });

            if (runActionParameter.Async)
            {
                
                string message = Utils.GetString(Strings.ResourceManager,"NewApplicationActionStarted");
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

            _runningApplicationActionRepositoryRemoveOperator.Remove(guid);

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

        private void ApplicationActionRunCallBack(ApplicationActionRun run, ActionRunEventHandlerEnvironment eventEnv)
        {
            if (run.Env.ActionCancelled)
                return;

            _applicationActionRuns.Remove(run.Guid);
            _runningApplicationActionRepositoryRemoveOperator.Remove(run.Guid);

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
