﻿using Common.Logger;
using Core.Application;
using Core.ApplicationAction;
using Core.Event;
using Core.SocketHandler;
using Core.User;
using Logging;
using Newtonsoft.Json;
using Persistance;
using Persistance.Repository;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.User;
using PlatypusApplicationFramework.Core.ApplicationAction;

namespace Core
{
    public class ServerInstance
    {
        private ServerConfig _config;

        private readonly ApplicationsHandler _applicationsHandler;
        private readonly ApplicationActionsHandler _applicationActionsHandler;
        private readonly EventsHandler _eventsHandlers;
        private readonly UsersHandler _usersHandler;

        private readonly ApplicationRepository _applicationRepository;
        private readonly LoggerManager _loggerManager;

        private readonly PlatypusSocketsHandler _socketsHandler;

        public ServerInstance()
        {
            ApplicationActionRepository applicationActionRepository = new ApplicationActionRepository();

            _eventsHandlers = new EventsHandler();

            _applicationActionsHandler = new ApplicationActionsHandler(
                applicationActionRepository,
                _eventsHandlers
            );

            UserRepository userRepository = new UserRepository();
            _usersHandler = new UsersHandler(
                userRepository
             );

            _usersHandler.AddBuiltInConnectionMethod(new PlatypusUserConnectionMethod(userRepository), BuiltInUserConnectionMethodGuid.PlatypusUser);

            _applicationRepository = new ApplicationRepository();

            ApplicationInstaller applicationInstaller = new ApplicationInstaller(
                _applicationRepository,
                applicationActionRepository,
                userRepository
            );

            ApplicationResolver applicationResolver = new ApplicationResolver(
                _applicationRepository,
                _applicationActionsHandler,
                _eventsHandlers,
                _usersHandler
            );

            _applicationsHandler = new ApplicationsHandler(
                _applicationRepository,
                applicationResolver,
                applicationInstaller
            );

            _loggerManager = new LoggerManager();
            _loggerManager.CreateLogger<PlatypusServerConsoleLogger>();

            _socketsHandler = new PlatypusSocketsHandler(
                _applicationActionsHandler,
                _usersHandler
            );
        }

        public void LoadConfiguration()
        {
            string json = File.ReadAllText(ApplicationPaths.CONFIGFILEPATH);
            _config = JsonConvert.DeserializeObject<ServerConfig>(json);
        }

        public void LoadApplications()
        {
            _applicationsHandler.LoadApplications();
        }

        public void InstallApplication(string applicationPath)
        {
            _applicationsHandler.InstallApplication(applicationPath);
        }

        public void UninstalApplication(string applicationGuid)
        {
            UninstallApplicationDetails details = _applicationsHandler.UninstallApplication(applicationGuid);
            foreach(string actionGuid in details.ActionGuids)
                _applicationActionsHandler.RemoveAction(actionGuid);
            foreach (string userConnectionMethodGuid in details.UserConnectionMethodGuids)
                _usersHandler.RemoveConnectionMethod(userConnectionMethodGuid);
        }

        public void InitializeServerSocketHandlers()
        {
            _socketsHandler.InitializeSocketHandlers(_config);
        }

        public ApplicationActionRunResult RunAction(ApplicationActionRunParameter runActionParameter)
        {
            if (_applicationActionsHandler.HasActionWithGuid(runActionParameter.Guid) == false)
            {
                string message = Common.Utils.GetString("ApplicationActionNotFound", runActionParameter.Guid);
                return new ApplicationActionRunResult()
                {
                    Message = message,
                    Status = ApplicationActionRunResultStatus.Failed,
                };
            }
            ApplicationActionEnvironmentBase env = _applicationActionsHandler.CreateStartActionEnvironment(runActionParameter.Guid);
            env.ApplicationRepository = _applicationRepository;
            env.ActionLoggers = new LoggerManager();

            return _applicationActionsHandler.RunAction(runActionParameter, env);
        }

        public void AddUser(string credentialMethodGUID, string fullName, string email, Dictionary<string,object> data)
        {
            _usersHandler.AddUser(credentialMethodGUID, fullName, email, data);
        }

        public void CancelRunningApplicationAction(string guid)
        {
            _applicationActionsHandler.CancelRunningAction(guid);
        }

        public UserAccount UserConnect(Dictionary<string, object> credential, string connectionMethodGuid)
        {
            return _usersHandler.Connect(credential, connectionMethodGuid);
        }

        public IEnumerable<ApplicationActionRunInfo> GetRunningApplicationActions()
        {
            return _applicationActionsHandler.GetRunningApplicationActionInfos();
        }
    }
}
