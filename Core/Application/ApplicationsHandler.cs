using PlatypusFramework.Configuration.Application;
using PlatypusUtils;
using PlatypusRepository;
using Core.Exceptions;
using Core.Event;
using PlatypusFramework.Core.Event;
using PlatypusAPI.ServerFunctionParameter;
using Core.Persistance.Entity;
using Core.Persistance;
using Core.Persistance.Repository;

namespace Core.Application
{
    internal class ApplicationsHandler
    {
        private readonly IRepositoryConsumeOperator<ApplicationEntity> _applicationRepositoryConsumeOperator;
        private readonly ApplicationInstaller _applicationInstaller;
        private readonly ApplicationResolver _applicationResolver;
        private readonly EventsHandler _eventsHandler;
        private readonly Dictionary<string, PlatypusApplicationBase> _applications;

        internal ApplicationsHandler(
            ApplicationRepository applicationRepository,
            ApplicationResolver applicationResolver,
            ApplicationInstaller applicationInstaller,
            EventsHandler eventsHandler
        )
        {
            _applicationRepositoryConsumeOperator = applicationRepository;

            _applicationResolver = applicationResolver;

            _applicationInstaller = applicationInstaller;

            _eventsHandler = eventsHandler;

            _applications = new Dictionary<string, PlatypusApplicationBase>();
        }

        internal void LoadApplications()
        {
            _applicationRepositoryConsumeOperator.Consume((entity) => {

                PlatypusApplicationBase applicationBase = PluginResolver.InstanciateImplementationFromRawBytes<PlatypusApplicationBase>(entity.AssemblyRaw);
                LoadApplication(applicationBase, entity.Guid);
            });
        }

        internal void LoadApplication(PlatypusApplicationBase application, string applicationGuid)
        {
            _applicationResolver.ResolvePlatypusApplication(application, applicationGuid);
            _applications.Add(applicationGuid, application);
        }

        internal bool InstallApplication(InstallApplicationParameter parameter)
        {
            InstallApplicationEventHandlerEnvironment eventEnv = new InstallApplicationEventHandlerEnvironment();

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.BeforeInstallApplication, eventEnv, (exception) => throw exception);

            PlatypusApplicationBase application = _applicationInstaller.InstallApplication(parameter.DllFilePath);

            if (application == null) return false;

            LoadApplication(application, application.ApplicationGuid);

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.AfterInstallApplication, eventEnv, (exception) => throw exception);

            return true;
        }

        internal void UninstallApplication(UninstallApplicationParameter parameter)
        {
            if (_applications.ContainsKey(parameter.ApplicationGuid) == false)
                throw new ApplicationInexistantException(parameter.ApplicationGuid);

            UninstallApplicationEventHandlerEnvironment eventEnv = new UninstallApplicationEventHandlerEnvironment()
            {
                ApplicationGuid = parameter.ApplicationGuid
            };

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.BeforeUninstallApplication, eventEnv, (exception) => throw exception);

            PlatypusApplicationBase application = _applications[parameter.ApplicationGuid];
            _applications.Remove(parameter.ApplicationGuid);

            _applicationInstaller.UninstallApplication(application, parameter.ApplicationGuid);

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.AfterUninstallApplication, eventEnv, (exception) => throw exception);
        }
    }
}
