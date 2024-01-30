using Core.Application;
using Core.ApplicationAction;
using Core.Event;
using Core.User;
using PlatypusLogging;
using PlatypusLogging.Loggers;
using Newtonsoft.Json;
using Persistance;
using Persistance.Repository;
using PlatypusAPI.Exceptions;
using PlatypusAPI.User;
using Core.Network.RestAPI;
using Core.Network;
using System.Net.Sockets;

namespace Core
{
    public partial class ServerInstance
    {
        private ServerConfig _config;

        private ApplicationsHandler _applicationsHandler;
        private ApplicationActionsHandler _applicationActionsHandler;
        private EventsHandler _eventsHandler;
        private UsersHandler _usersHandler;

        private ApplicationRepository _applicationRepository;
        private LoggerManager _loggerManager;

        private ISeverPortListener _restAPIHandler;
        private ISeverPortListener _tcpServerSocketHandler;

        public void Initialize(string[] args)
        {
            string json = File.ReadAllText(ApplicationPaths.CONFIGFILEPATH);
            _config = JsonConvert.DeserializeObject<ServerConfig>(json);

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
            _loggerManager.CreateLogger<ConsoleLogger>();

            _restAPIHandler = new RestAPIHandler(this, args, _config.RestAPIUserTokenTimeout);
            _tcpServerSocketHandler = new PlatypusServerSocketHandler(this, ProtocolType.Tcp);
        }

        public void Start()
        {
            _applicationsHandler.LoadApplications();

            _applicationActionsHandler.ReRunStopedApplicationActions(_applicationRepository);

            _tcpServerSocketHandler.InitializeServerPortListener(_config.TcpSocketPort);
            _restAPIHandler.InitializeServerPortListener(_config.HttpPort);
        }

        private void ValidateUserForPermission(UserAccount userAccount, UserPermissionFlag userPermissionFlag)
        {
            if (userAccount.UserPermissionFlags == UserPermissionFlag.Admin) return;

            if (userAccount.UserPermissionFlags.HasFlag(userPermissionFlag) == false)
                throw new UserPermissionException(userAccount);
        }
    }
}
