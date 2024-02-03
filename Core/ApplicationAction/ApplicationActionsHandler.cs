﻿using Core.ApplicationAction.Run;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusFramework.Core.ApplicationAction;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.ApplicationAction;
using System.Reflection;
using Core.Event;
using PlatypusFramework.Core.Event;
using PlatypusUtils;
using Core.Ressource;
using PlatypusRepository;
using Core.Persistance.Entity;
using Core.Abstract;
using Core.ApplicationAction.Abstract;

namespace Core.ApplicationAction
{
    internal class ApplicationActionsHandler : 
        IApplicationAttributeMethodResolver<ActionDefinitionAttribute>,
        IRepositoryConsumeOperator<ApplicationActionEntity>,
        IRepositoryRemoveOperator<ApplicationActionEntity>,
        IRepositoryRemoveOperator<RunningApplicationActionEntity>,
        IRepositoryConsumeOperator<RunningApplicationActionEntity>,
        IApplicationActionRunner,
        IServerStarter
    {
        private readonly Dictionary<string, ApplicationAction> _applicationActions;
        private readonly Dictionary<string, ApplicationActionRun> _applicationActionRuns;
        private readonly IRepositoryAddOperator<RunningApplicationActionEntity> _runningApplicationActionRepositoryAddOperator;
        private readonly IRepositoryRemoveOperator<RunningApplicationActionEntity> _runningApplicationActionRepositoryRemoveOperator;
        private readonly IRepositoryConsumeOperator<RunningApplicationActionEntity> _runningApplicationActionRepositoryConsumeOperator;
        private readonly IRepositoryConsumeOperator<ApplicationActionEntity> _applicationActionRepositoryConsumeOperator;
        private readonly IRepositoryRemoveOperator<ApplicationActionEntity> _applicationActionRepositoryRemoveOperator;
        private readonly EventsHandler _eventsHandler;

        internal ApplicationActionsHandler(
            IRepositoryAddOperator<RunningApplicationActionEntity> runningApplicationActionRepositoryAddOperator,
            IRepositoryRemoveOperator<RunningApplicationActionEntity> runningApplicationActionRepositoryRemoveOperator,
            IRepositoryConsumeOperator<RunningApplicationActionEntity> runningApplicationActionRepositoryConsumeOperator,
            IRepositoryConsumeOperator<ApplicationActionEntity> applicationActionRepositoryConsumeOperator,
            IRepositoryRemoveOperator<ApplicationActionEntity> applicationActionRepositoryRemoveOperator,
            EventsHandler eventsHandler
        )
        {
            _applicationActions = new Dictionary<string, ApplicationAction>();
            _applicationActionRuns = new Dictionary<string, ApplicationActionRun>();
            _runningApplicationActionRepositoryAddOperator = runningApplicationActionRepositoryAddOperator;
            _runningApplicationActionRepositoryRemoveOperator = runningApplicationActionRepositoryRemoveOperator;
            _runningApplicationActionRepositoryConsumeOperator = runningApplicationActionRepositoryConsumeOperator;
            _applicationActionRepositoryConsumeOperator = applicationActionRepositoryConsumeOperator;
            _applicationActionRepositoryRemoveOperator = applicationActionRepositoryRemoveOperator;
            _eventsHandler = eventsHandler;
        }

        public void Resolve(PlatypusApplicationBase application, ActionDefinitionAttribute attribute, MethodInfo method)
        {
            string actionGuid = attribute.Name + application.ApplicationGuid;
            if (_applicationActions.ContainsKey(actionGuid)) return;

            ApplicationAction applicationAction = new ApplicationAction(application, attribute, method, actionGuid);
            _applicationActions.Add(actionGuid, applicationAction);
        }

        public ApplicationActionRunResult Run(ApplicationActionRunParameter runActionParameter)
        {
            if (_applicationActions.ContainsKey(runActionParameter.Guid) == false)
            {
                string message = Utils.GetString(Strings.ResourceManager, "ApplicationActionNotFound", runActionParameter.Guid);
                return new ApplicationActionRunResult()
                {
                    Message = message,
                    Status = ApplicationActionRunResultStatus.Failed,
                };
            }

            ApplicationActionEnvironmentBase env = _applicationActions[runActionParameter.Guid].CreateStartActionEnvironment();

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

        public void Remove(ApplicationActionEntity entity)
        {
            foreach (ApplicationActionRun applicationActionRun in _applicationActionRuns.Values)
            {
                if (applicationActionRun.ActionGuid == entity.Guid)
                    Remove(new RunningApplicationActionEntity() { Guid = entity.Guid });
            }
            _applicationActions.Remove(entity.Guid);
            _applicationActionRepositoryRemoveOperator.Remove(entity);
        }

        public void Remove(RunningApplicationActionEntity entity)
        {
            if (_applicationActionRuns.ContainsKey(entity.Guid) == false) throw new Exception($"No action with guid '{entity.Guid}' is runiing");

            CancelRunningActionEventHandlerEnvironment eventEnv = new CancelRunningActionEventHandlerEnvironment()
            {
                RunningActionGuid = entity.Guid
            };

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.BeforeCancelApplicationRun, eventEnv, (exception) => throw exception);

            ApplicationActionRun run = _applicationActionRuns[entity.Guid];
            _applicationActionRuns.Remove(entity.Guid);

            _runningApplicationActionRepositoryRemoveOperator.Remove(new RunningApplicationActionEntity() { Guid = entity.Guid });

            run.Cancel();

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.AfterCancelApplicationRun, eventEnv, (exception) => throw exception);
        }

        public void Consume(Action<ApplicationActionEntity> consumer, Predicate<ApplicationActionEntity> condition = null)
        {
            _applicationActionRepositoryConsumeOperator.Consume(consumer, condition);
        }

        public void Consume(Action<RunningApplicationActionEntity> consumer, Predicate<RunningApplicationActionEntity> condition = null)
        {
            _runningApplicationActionRepositoryConsumeOperator.Consume(consumer, condition);
        }

        private void ApplicationActionRunCallBack(ApplicationActionRun run, ActionRunEventHandlerEnvironment eventEnv)
        {
            if (run.Env.ActionCancelled)
                return;

            _applicationActionRuns.Remove(run.Guid);
            _runningApplicationActionRepositoryRemoveOperator.Remove(new RunningApplicationActionEntity() { Guid = run.Guid });

            eventEnv.ApplicationActionResult = run.Result;

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
            _eventsHandler.RunEventHandlers(EventHandlerType.BeforeApplicationActionRun, eventEnv, (exception) => {
                return new ApplicationActionRunResult()
                {
                    Status = ApplicationActionRunResultStatus.Failed,
                    Message = exception.Message,
                };
            });

            return null;
        }

        public void Start()
        {
            
        }
    }
}
