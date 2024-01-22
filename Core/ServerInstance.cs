using Common.Logger;
using Core.Application;
using Core.ApplicationAction;
using Core.Event;
using Core.RestAPI;
using Core.SocketHandler;
using Core.User;
using Logging;
using Newtonsoft.Json;
using Persistance;
using Persistance.Repository;
using PlatypusAPI.Exceptions;
using PlatypusAPI.User;

namespace Core
{
    public partial class ServerInstance
    {
        private ServerConfig _config;

        private readonly ApplicationsHandler _applicationsHandler;
        private readonly ApplicationActionsHandler _applicationActionsHandler;
        private readonly EventsHandler _eventsHandler;
        private readonly UsersHandler _usersHandler;

        private readonly ApplicationRepository _applicationRepository;
        private readonly LoggerManager _loggerManager;

        private readonly RestAPIHandler _restAPIHandler;
        private readonly PlatypusSocketsHandler _socketsHandler;

        public ServerInstance()
        {
            ApplicationActionRepository applicationActionRepository = new ApplicationActionRepository();

            _eventsHandler = new EventsHandler();

            _applicationActionsHandler = new ApplicationActionsHandler(
                applicationActionRepository,
                _eventsHandler
            );

            UserRepository userRepository = new UserRepository();
            _usersHandler = new UsersHandler(
                userRepository
             );

            _usersHandler.AddBuiltInConnectionMethod(new PlatypusUserConnectionMethod(), BuiltInUserConnectionMethodGuid.PlatypusUser);

            _applicationRepository = new ApplicationRepository();

            ApplicationInstaller applicationInstaller = new ApplicationInstaller(
                _applicationRepository,
                applicationActionRepository,
                userRepository
            );

            ApplicationResolver applicationResolver = new ApplicationResolver(
                _applicationRepository,
                _applicationActionsHandler,
                _eventsHandler,
                _usersHandler
            );

            _applicationsHandler = new ApplicationsHandler(
                _applicationRepository,
                applicationResolver,
                applicationInstaller,
                _eventsHandler
            );

            _loggerManager = new LoggerManager();
            _loggerManager.CreateLogger<PlatypusServerConsoleLogger>();

            _restAPIHandler = new RestAPIHandler(this);
            _socketsHandler = new PlatypusSocketsHandler(this);
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

        public void InitializeServerSocketHandlers()
        {
            _socketsHandler.Initialize(_config);
        }

        public void InitializeRestAPIHandler(string[] args)
        {
            _restAPIHandler.Initialize(args, _config.HttpPort, _config.RestAPIUserTokenTimeout);
        }

        private void ValidateUserForPermission(UserAccount userAccount, UserPermissionFlag userPermissionFlag)
        {
            if (userAccount.UserPermissionFlags == UserPermissionFlag.Admin) return;

            if (userAccount.UserPermissionFlags.HasFlag(userPermissionFlag) == false)
                throw new UserPermissionException(userAccount);
        }
    }
}
