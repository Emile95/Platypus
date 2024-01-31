using PlatypusFramework.Configuration.Application;
using PlatypusUtils;
using PlatypusRepository;
using Core.Exceptions;
using Core.Event;
using PlatypusFramework.Core.Event;
using PlatypusAPI.ServerFunctionParameter;
using Core.Persistance.Entity;
using Core.Persistance;

namespace Core.Application
{
    public class ApplicationsHandler
    {
        private readonly Repository<ApplicationEntity> _applicationRepository;
        private readonly ApplicationInstaller _applicationInstaller;
        private readonly ApplicationResolver _applicationResolver;
        private readonly EventsHandler _eventsHandler;
        private readonly Dictionary<string, PlatypusApplicationBase> _applications;

        public ApplicationsHandler(
            Repository<ApplicationEntity> applicationRepository,
            ApplicationResolver applicationResolver,
            ApplicationInstaller applicationInstaller,
            EventsHandler eventsHandler
        )
        {
            _applicationRepository = applicationRepository;

            _applicationResolver = applicationResolver;

            _applicationInstaller = applicationInstaller;

            _eventsHandler = eventsHandler;

            _applications = new Dictionary<string, PlatypusApplicationBase>();
        }

        public void LoadApplications()
        {
            _applicationRepository.Consume((entity) => {

                PlatypusApplicationBase applicationBase = PluginResolver.InstanciateImplementationFromRawBytes<PlatypusApplicationBase>(entity.AssemblyRaw);
                LoadApplication(applicationBase, entity.Guid);
            });
        }

        public void LoadApplication(PlatypusApplicationBase application, string applicationGuid)
        {
            _applicationResolver.ResolvePlatypusApplication(application, applicationGuid);
            application.ApplicationDirectoryPath = ApplicationPaths.GetApplicationDirectoryPath(applicationGuid);
            _applications.Add(applicationGuid, application);
        }

        public bool InstallApplication(InstallApplicationParameter parameter)
        {
            string newGuid = Utils.GenerateGuidFromEnumerable(_applications.Keys);

            InstallApplicationEventHandlerEnvironment eventEnv = new InstallApplicationEventHandlerEnvironment()
            {
                ApplicationGuid = newGuid
            };

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.BeforeInstallApplication, eventEnv, (exception) => throw exception);

            PlatypusApplicationBase application = _applicationInstaller.InstallApplication(newGuid, parameter.DllFilePath);

            if (application == null) return false;

            LoadApplication(application, newGuid);

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.AfterInstallApplication, eventEnv, (exception) => throw exception);

            return true;
        }

        public void UninstallApplication(UninstallApplicationParameter parameter)
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
