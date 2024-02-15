using PlatypusAPI.ApplicationAction.Run;
using PlatypusFramework.Core.ApplicationAction;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.ApplicationAction;
using System.Reflection;
using PlatypusFramework.Core.Event;
using PlatypusUtils;
using Core.Ressource;
using PlatypusRepository;
using Core.Persistance.Entity;
using Core.Abstract;
using Core.ApplicationAction.Abstract;
using PlatypusAPI.ApplicationAction;
using Core.Event.Abstract;

namespace Core.ApplicationAction
{
    public class ApplicationActionsHandler : 
        IApplicationAttributeMethodResolver<ActionDefinitionAttribute>,
        IRepositoryConsumeOperator<ApplicationActionInfo>,
        IRepositoryRemoveOperator<ApplicationAction, string>,
        IServerStarter<ApplicationActionsHandler>,
        IApplicationActionStarter
    {
        private readonly Dictionary<string, ApplicationAction> _applicationActions;
        private readonly IRepositoryConsumeOperator<ApplicationActionEntity> _applicationActionRepositoryConsumeOperator;
        private readonly IRepositoryConsumeOperator<RunningApplicationActionEntity> _runningApplicationActionOperatorOperator;
        private readonly IRepositoryRemoveOperator<ApplicationActionEntity, string> _applicationActionRepositoryRemoveOperator;
        private readonly IRepositoryAddOperator<ApplicationActionRun> _applicationActionRunAddOperator;
        private readonly IRepositoryAddOperator<RunningApplicationActionEntity> _runningApplicationActionAddOperator;
        private readonly IRepositoryRemoveOperator<ApplicationActionRun,string> _applicationActionRunRemoveOperator;
        private readonly IEventHandlerRunner _eventhHandlerRunner;

        public ApplicationActionsHandler(
            IRepositoryConsumeOperator<ApplicationActionEntity> applicationActionRepositoryConsumeOperator,
            IRepositoryConsumeOperator<RunningApplicationActionEntity> runningApplicationActionOperatorOperator,
            IRepositoryRemoveOperator<ApplicationActionEntity, string> applicationActionRepositoryRemoveOperator,
            IRepositoryAddOperator<ApplicationActionRun> applicationActionRunAddOperator,
            IRepositoryAddOperator<RunningApplicationActionEntity> runningApplicationActionAddOperator,
            IRepositoryRemoveOperator<ApplicationActionRun, string> applicationActionRunRemoveOperator,
            IEventHandlerRunner eventhHandlerRunner
        )
        {
            _applicationActions = new Dictionary<string, ApplicationAction>();
            _applicationActionRepositoryConsumeOperator = applicationActionRepositoryConsumeOperator;
            _runningApplicationActionOperatorOperator = runningApplicationActionOperatorOperator;
            _applicationActionRepositoryRemoveOperator = applicationActionRepositoryRemoveOperator;
            _applicationActionRunAddOperator = applicationActionRunAddOperator;
            _runningApplicationActionAddOperator = runningApplicationActionAddOperator;
            _applicationActionRunRemoveOperator = applicationActionRunRemoveOperator;
            _eventhHandlerRunner = eventhHandlerRunner;
        }

        public void Resolve(PlatypusApplicationBase application, ActionDefinitionAttribute attribute, MethodInfo method)
        {
            string actionGuid = attribute.Name + application.ApplicationGuid;
            if (_applicationActions.ContainsKey(actionGuid)) return;

            ApplicationAction applicationAction = new ApplicationAction(application, attribute, method, actionGuid);
            _applicationActions.Add(actionGuid, applicationAction);
        }

        public void Start()
        {
            _runningApplicationActionOperatorOperator.Consume((entity) => {
                ApplicationActionRun applicationActionRun = BuildApplicationActionRun(entity.ActionGuid, entity.Parameters, true);
                applicationActionRun.Guid = entity.ActionRunGuid;

                _applicationActionRunAddOperator.Add(applicationActionRun);

                ActionRunEventHandlerEnvironment actionRunEventHandlerEnvironment = new ActionRunEventHandlerEnvironment();

                ApplicationActionRunResult result = BeforeApplicationActionRun(actionRunEventHandlerEnvironment);

                applicationActionRun.StartRun(() => {
                    ApplicationActionRunCallBack(applicationActionRun, actionRunEventHandlerEnvironment);
                });
            });
        }

        public ApplicationActionRunResult Start(StartApplicationActionParameter parameter)
        {
            try
            {
                ApplicationActionRun applicationActionRun = BuildApplicationActionRun(parameter.Guid, parameter.ActionParameters, false);

                _applicationActionRunAddOperator.Add(applicationActionRun);
                _runningApplicationActionAddOperator.Add(new RunningApplicationActionEntity()
                {
                    Guid = applicationActionRun.GetRunningActionGuid(),
                    ActionGuid = applicationActionRun.ActionGuid,
                    ActionRunGuid = applicationActionRun.Guid,
                    Parameters = parameter.ActionParameters
                });

                ActionRunEventHandlerEnvironment actionRunEventHandlerEnvironment = new ActionRunEventHandlerEnvironment();

                ApplicationActionRunResult result = BeforeApplicationActionRun(actionRunEventHandlerEnvironment);

                applicationActionRun.StartRun(() => {
                    ApplicationActionRunCallBack(applicationActionRun, actionRunEventHandlerEnvironment);
                });

                if (parameter.Async)
                {
                    string message = Utils.GetString(Strings.ResourceManager, "NewApplicationActionStarted");
                    return new ApplicationActionRunResult()
                    {
                        Message = message,
                        Status = ApplicationActionRunResultStatus.Started
                    };
                }

                applicationActionRun.Task.Wait();

                return applicationActionRun.Result;
            }
            catch (Exception e)
            {
                return new ApplicationActionRunResult
                {
                    Status = ApplicationActionRunResultStatus.Failed,
                    Message = e.Message,
                };
            }
        }

        public void Remove(string id)
        {
            _applicationActions.Remove(id);
            _applicationActionRepositoryRemoveOperator.Remove(id);
        }

        public void Consume(Action<ApplicationActionInfo> consumer, Predicate<ApplicationActionInfo> condition = null)
        {
            _applicationActionRepositoryConsumeOperator.Consume((entity) => {
                ApplicationActionInfo applicationActionInfo = new ApplicationActionInfo()
                {
                    Guid = entity.Guid,
                };
                if (condition == null) consumer(applicationActionInfo);
                else
                    if(condition(applicationActionInfo))
                        consumer(applicationActionInfo);
            });
        }

        private ApplicationActionRun BuildApplicationActionRun(string actionGuid, Dictionary<string, object> actionParameters, bool runExist)
        {
            if (_applicationActions.ContainsKey(actionGuid) == false)
            {
                string message = Utils.GetString(Strings.ResourceManager, "ApplicationActionNotFound", actionGuid);
                throw new Exception(message);
            }

            ApplicationAction applicationAction = _applicationActions[actionGuid];
            ApplicationActionEnvironmentBase env = applicationAction.CreateStartActionEnvironment();

            applicationAction.ResolveActionParameter(env, actionParameters);

            ApplicationActionRun applicationActionRun = applicationAction.BuildApplicationActionRun(actionGuid, env, runExist);

            return applicationActionRun;
        }


        private void ApplicationActionRunCallBack(ApplicationActionRun run, ActionRunEventHandlerEnvironment eventEnv)
        {
            if (run.Env.ActionCancelled)
                return;

            eventEnv.ApplicationActionResult = run.Result;

            _eventhHandlerRunner.Run<object>(EventHandlerType.AfterApplicationActionRun, eventEnv, (exception) => {
                if (run.Result.Status != ApplicationActionRunResultStatus.Failed)
                {
                    run.Result.Status = ApplicationActionRunResultStatus.Failed;
                    run.Result.Message = exception.Message;
                }
                return null;
            });

            _applicationActionRunRemoveOperator.Remove(run.GetRunningActionGuid());
        }

        private ApplicationActionRunResult BeforeApplicationActionRun(ActionRunEventHandlerEnvironment eventEnv)
        {
            _eventhHandlerRunner.Run(EventHandlerType.BeforeApplicationActionRun, eventEnv, (exception) => {
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
