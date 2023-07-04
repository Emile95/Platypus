using Persistance.Repository;
using Persistance.Entity;
using PlatypusApplicationFramework.Configuration.Application;
using Utils;
using Utils.GuidGeneratorHelper;
using Persistance;
using Core.Exceptions;
using Core.Event;
using PlatypusApplicationFramework.Core.Event;

namespace Core.Application
{
    public class ApplicationsHandler
    {
        private readonly ApplicationRepository _applicationRepository;
        private readonly ApplicationInstaller _applicationInstaller;
        private readonly ApplicationResolver _applicationResolver;
        private readonly EventsHandler _eventsHandler;
        private readonly Dictionary<string, PlatypusApplicationBase> _applications;

        public ApplicationsHandler(
            ApplicationRepository applicationRepository,
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
            List<ApplicationEntity> applications = _applicationRepository.LoadApplications();
            foreach(ApplicationEntity application in applications)
            {
                PlatypusApplicationBase applicationBase = PluginResolver.InstanciateImplementationFromDll<PlatypusApplicationBase>(application.DllFilePath);
                LoadApplication(applicationBase, application.Guid);
            }
        }

        public void LoadApplication(PlatypusApplicationBase application, string applicationGuid)
        {
            _applicationResolver.ResolvePlatypusApplication(application, applicationGuid);
            application.ApplicationDirectoryPath = ApplicationPaths.GetApplicationDirectoryPath(applicationGuid);
            _applications.Add(applicationGuid, application);
        }

        public void InstallApplication(string applicationPath)
        {
            string newGuid = GuidGenerator.GenerateFromEnumerable(_applications.Keys);

            InstallApplicationEventHandlerEnvironment eventEnv = new InstallApplicationEventHandlerEnvironment()
            {
                ApplicationGuid = newGuid
            };

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.BeforeInstallApplication, eventEnv, (exception) => throw exception);

            PlatypusApplicationBase application = _applicationInstaller.InstallApplication(newGuid, applicationPath);
            LoadApplication(application, newGuid);

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.AfterInstallApplication, eventEnv, (exception) => throw exception);
        }

        public UninstallApplicationDetails UninstallApplication(string applicationGuid)
        {
            if (_applications.ContainsKey(applicationGuid) == false)
                throw new ApplicationInexistantException(applicationGuid);

            UninstallApplicationEventHandlerEnvironment eventEnv = new UninstallApplicationEventHandlerEnvironment()
            {
                ApplicationGuid = applicationGuid
            };

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.BeforeUninstallApplication, eventEnv, (exception) => throw exception);

            PlatypusApplicationBase application = _applications[applicationGuid];
            _applications.Remove(applicationGuid);

            UninstallApplicationDetails details = _applicationInstaller.UninstallApplication(application, applicationGuid);

            details.EventEnv = eventEnv;

            return details;
        }
    }
}
