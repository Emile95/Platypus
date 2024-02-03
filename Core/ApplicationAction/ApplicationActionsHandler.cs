using Core.ApplicationAction.Run;
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
        IApplicationActionRunner
    {
        private readonly Dictionary<string, ApplicationAction> _applicationActions;
        private readonly IRepositoryConsumeOperator<ApplicationActionEntity> _applicationActionRepositoryConsumeOperator;
        private readonly IRepositoryRemoveOperator<ApplicationActionEntity, string> _applicationActionRepositoryRemoveOperator;
        private readonly IRepositoryAddOperator<ApplicationActionRun> _applicationActionRunAddOperator;
        private readonly IEventHandlerRunner _eventhHandlerRunner;

        public ApplicationActionsHandler(
            IRepositoryConsumeOperator<ApplicationActionEntity> applicationActionRepositoryConsumeOperator,
            IRepositoryRemoveOperator<ApplicationActionEntity, string> applicationActionRepositoryRemoveOperator,
            IRepositoryAddOperator<ApplicationActionRun> applicationActionRunAddOperator,
            IEventHandlerRunner eventhHandlerRunner
        )
        {
            _applicationActions = new Dictionary<string, ApplicationAction>();
            _applicationActionRepositoryConsumeOperator = applicationActionRepositoryConsumeOperator;
            _applicationActionRepositoryRemoveOperator = applicationActionRepositoryRemoveOperator;
            _applicationActionRunAddOperator = applicationActionRunAddOperator;
            _eventhHandlerRunner = eventhHandlerRunner;
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

            _applicationActionRunAddOperator.Add(applicationActionRun);

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
        }

        private ApplicationActionRunResult BeforeApplicationActionRun(ApplicationAction applicationAction, ActionRunEventHandlerEnvironment eventEnv)
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
