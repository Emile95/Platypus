﻿using Persistance.Repository;
using Persistance.Entity;
using PlatypusFramework.Configuration.Application;
using PlatypusUtils;
using Persistance;
using Core.Exceptions;
using Core.Event;
using PlatypusFramework.Core.Event;
using PlatypusAPI.ServerFunctionParameter;

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

        public void InstallApplication(InstallApplicationParameter parameter)
        {
            string newGuid = Utils.GenerateGuidFromEnumerable(_applications.Keys);

            InstallApplicationEventHandlerEnvironment eventEnv = new InstallApplicationEventHandlerEnvironment()
            {
                ApplicationGuid = newGuid
            };

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.BeforeInstallApplication, eventEnv, (exception) => throw exception);

            PlatypusApplicationBase application = _applicationInstaller.InstallApplication(newGuid, parameter.DllFilePath);
            LoadApplication(application, newGuid);

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.AfterInstallApplication, eventEnv, (exception) => throw exception);
        }

        public UninstallApplicationDetails UninstallApplication(UninstallApplicationParameter parameter)
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

            UninstallApplicationDetails details = _applicationInstaller.UninstallApplication(application, parameter.ApplicationGuid);

            details.EventEnv = eventEnv;

            return details;
        }
    }
}
