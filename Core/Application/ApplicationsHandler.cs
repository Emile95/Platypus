using PlatypusFramework.Configuration.Application;
using PlatypusUtils;
using PlatypusRepository;
using Core.Exceptions;
using Core.Event;
using PlatypusFramework.Core.Event;
using Core.Persistance.Entity;
using Core.Abstract;
using Core.Application.Abstract;
using PlatypusFramework.Core.Application;

namespace Core.Application
{
    internal class ApplicationsHandler : 
        IApplicationResolver<PlatypusApplicationBase>, 
        IApplicationPackageInstaller<PlatypusApplicationBase>,
        IApplicationPackageUninstaller<string>,
        IServerStarter
    {
        private readonly Dictionary<string, PlatypusApplicationBase> _applications;

        private readonly IApplicationPackageInstaller<PlatypusApplicationBase> _applicationPackageInstaller;

        private readonly IApplicationResolver<PlatypusApplicationBase> _applicationResolver;

        private readonly IRepositoryConsumeOperator<ApplicationEntity> _applicationRepositoryConsumeOperator;
        private readonly IRepositoryRemoveOperator<string> _applicationRepositoryRemoveOperator;

        private readonly IRepositoryRemoveOperator<string> _applicationActionRepositoryRemoveOperator;

        private readonly EventsHandler _eventsHandler;

        internal ApplicationsHandler(
            IRepositoryConsumeOperator<ApplicationEntity> applicationRepositoryConsumeOperator,
            IRepositoryRemoveOperator<string> applicationRepositoryRemoveOperator,
            IApplicationResolver<PlatypusApplicationBase> applicationResolver,
            IApplicationPackageInstaller<PlatypusApplicationBase> applicationPackageInstaller,
            IRepositoryRemoveOperator<string> applicationActionRepositoryRemoveOperator,
            EventsHandler eventsHandler
        )
        {
            _applicationRepositoryConsumeOperator = applicationRepositoryConsumeOperator;
            _applicationRepositoryRemoveOperator = applicationRepositoryRemoveOperator;

            _applications = new Dictionary<string, PlatypusApplicationBase>();

            _applicationPackageInstaller = applicationPackageInstaller;

            _applicationResolver = applicationResolver;

            _applicationActionRepositoryRemoveOperator = applicationActionRepositoryRemoveOperator;

            _eventsHandler = eventsHandler;
        }

        public void Resolve(PlatypusApplicationBase application)
        {
            _applicationResolver.Resolve(application);
            _applications.Add(application.ApplicationGuid, application);
        }

        public void Start()
        {
            _applicationRepositoryConsumeOperator.Consume((entity) => {

                PlatypusApplicationBase applicationBase = PluginResolver.InstanciateImplementationFromRawBytes<PlatypusApplicationBase>(entity.AssemblyRaw);
                applicationBase.ApplicationGuid = entity.Guid;
                Resolve(applicationBase);
            });
        }

        public PlatypusApplicationBase Install(string sourcePath)
        {
            InstallApplicationEventHandlerEnvironment eventEnv = new InstallApplicationEventHandlerEnvironment();

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.BeforeInstallApplication, eventEnv, (exception) => throw exception);

            PlatypusApplicationBase application = _applicationPackageInstaller.Install(sourcePath);

            if (application == null) return null;

            Resolve(application);

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.AfterInstallApplication, eventEnv, (exception) => throw exception);

            return application;
        }

        public void Uninstall(string applicationIdentifier)
        {
            if (_applications.ContainsKey(applicationIdentifier) == false)
                throw new ApplicationInexistantException(applicationIdentifier);

            UninstallApplicationEventHandlerEnvironment eventEnv = new UninstallApplicationEventHandlerEnvironment()
            {
                ApplicationGuid = applicationIdentifier
            };

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.BeforeUninstallApplication, eventEnv, (exception) => throw exception);

            PlatypusApplicationBase application = _applications[applicationIdentifier];
            _applications.Remove(applicationIdentifier);

            ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
            env.ApplicationGuid = applicationIdentifier;

            application.Uninstall(env);

            _applicationRepositoryRemoveOperator.Remove(applicationIdentifier);

            string[] applicationActionsNames = application.GetAllApplicationActionNames();
            foreach (string applicationActionsName in applicationActionsNames)
                _applicationActionRepositoryRemoveOperator.Remove(applicationActionsName + applicationIdentifier);

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.AfterUninstallApplication, eventEnv, (exception) => throw exception);
        }
    }
}
