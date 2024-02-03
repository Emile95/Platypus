using Core.Abstract;
using Core.Application.Abstract;
using Core.ApplicationAction.Run;
using Core.Event.Abstract;
using Core.Exceptions;
using Core.Persistance.Entity;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Core.Application;
using PlatypusFramework.Core.Event;
using PlatypusRepository;

namespace Core.Application
{
    public class ApplicationUninstaller : IApplicationUninstaller
    {
        private readonly IGuidValidator<PlatypusApplicationBase> _applicationGuidValidator;
        private readonly IGetterByGuid<PlatypusApplicationBase> _applicationGetter;
        private readonly IRepositoryRemoveOperator<PlatypusApplicationBase, string> _applicationRemoveOperator;
        private readonly IRepositoryRemoveOperator<ApplicationEntity, string> _applicationEntityRemoveOperator;
        private readonly IRepositoryRemoveOperator<ApplicationAction.ApplicationAction, string> _applicationActionRemoveOperator;
        private readonly IRepositoryRemoveOperator<ApplicationActionRun, string> _applicationActionRunRemoveOperator;
        private readonly IRepositoryConsumeOperator<ApplicationActionRun> _applicationActionRunConsumeOperator;
        private readonly IRepositoryRemoveOperator<RemoveUserParameter, string> _userRemoveOperator;
        private readonly IEventHandlerRunner _eventhHandlerRunner;

        public ApplicationUninstaller(
            IGuidValidator<PlatypusApplicationBase> applicationGuidValidator,
            IGetterByGuid<PlatypusApplicationBase> applicationGetter,
            IRepositoryRemoveOperator<PlatypusApplicationBase, string> applicationRemoveOperator,
            IRepositoryRemoveOperator<ApplicationEntity, string> applicationEntityRemoveOperator,
            IRepositoryRemoveOperator<ApplicationAction.ApplicationAction, string> applicationActionRemoveOperator,
            IRepositoryRemoveOperator<ApplicationActionRun, string> applicationActionRunRemoveOperator,
            IRepositoryConsumeOperator<ApplicationActionRun> applicationActionRunConsumeOperator,
            IRepositoryRemoveOperator<RemoveUserParameter, string> userRemoveOperator,
            IEventHandlerRunner eventhHandlerRunner
        ) 
        {
            _applicationGuidValidator = applicationGuidValidator;
            _applicationGetter = applicationGetter;
            _applicationRemoveOperator = applicationRemoveOperator;
            _applicationEntityRemoveOperator = applicationEntityRemoveOperator;
            _applicationActionRemoveOperator = applicationActionRemoveOperator;
            _applicationActionRunRemoveOperator = applicationActionRunRemoveOperator;
            _applicationActionRunConsumeOperator = applicationActionRunConsumeOperator;
            _userRemoveOperator = userRemoveOperator;
            _eventhHandlerRunner = eventhHandlerRunner;
        }

        public void Uninstall(string applicationIdentifier)
        {
            if (_applicationGuidValidator.Validate(applicationIdentifier) == false)
                throw new ApplicationInexistantException(applicationIdentifier);

            UninstallApplicationEventHandlerEnvironment eventEnv = new UninstallApplicationEventHandlerEnvironment()
            {
                ApplicationGuid = applicationIdentifier
            };

            _eventhHandlerRunner.Run<object>(EventHandlerType.BeforeUninstallApplication, eventEnv, (exception) => throw exception);

            PlatypusApplicationBase application = _applicationGetter.Get(applicationIdentifier);
            _applicationRemoveOperator.Remove(applicationIdentifier);

            ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
            env.ApplicationGuid = applicationIdentifier;

            application.Uninstall(env);

            string[] applicationActionsNames = application.GetAllApplicationActionNames();
            HashSet<string> applicationActionGuids = new HashSet<string>();

            foreach (string applicationActionsName in applicationActionsNames)
            {
                string applicationActionGuid = applicationActionsName + applicationIdentifier;
                applicationActionGuids.Add(applicationActionGuid);
                _applicationActionRemoveOperator.Remove(applicationActionGuid);
            }

            _applicationEntityRemoveOperator.Remove(applicationIdentifier);
            _applicationRemoveOperator.Remove(applicationIdentifier);

            RemoveApplicationActionRun(applicationActionGuids);
            RemoveUsers(applicationIdentifier);

            _eventhHandlerRunner.Run<object>(EventHandlerType.AfterUninstallApplication, eventEnv, (exception) => throw exception);
        }

        private void RemoveApplicationActionRun(HashSet<string> applicationActionGuids)
        {
            _applicationActionRunConsumeOperator.Consume(
                (applicationActionRun) => _applicationActionRunRemoveOperator.Remove(applicationActionRun.Guid),
                (applicationActionRun) => applicationActionGuids.Contains(applicationActionRun.Guid)
            );
        }

        private void RemoveUsers(string applicationGuid)
        {
            
        }
    }
}
