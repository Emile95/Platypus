using Core.Application;
using Core.ApplicationAction;
using Core.Event;
using Core.User;
using Newtonsoft.Json;
using PlatypusAPI.Exceptions;
using PlatypusAPI.User;
using Core.Network.RestAPI;
using Core.Network;
using System.Net.Sockets;
using Core.Persistance.Repository;
using Core.Persistance;

namespace Core
{
    public partial class ServerInstance
    {
        private ServerConfig _config;

        private ApplicationsHandler _applicationsHandler;
        private ApplicationActionsHandler _applicationActionsHandler;
        private EventsHandler _eventsHandler;
        private UsersHandler _usersHandler;

        private ISeverPortListener _restAPIHandler;
        private ISeverPortListener _tcpServerSocketHandler;

        public void Initialize(string[] args)
        {
            string json = File.ReadAllText(ApplicationPaths.CONFIGFILEPATH);
            _config = JsonConvert.DeserializeObject<ServerConfig>(json);

            ApplicationRepository applicationRepository = new ApplicationRepository();
            ApplicationActionRepository applicationActionRepository = new ApplicationActionRepository();
            RunningApplicationActionRepository runningApplicationActionRepository = new RunningApplicationActionRepository();
            UserRepository userRepository = new UserRepository();

            _eventsHandler = new EventsHandler();

            _usersHandler = new UsersHandler(userRepository);

            _applicationActionsHandler = new ApplicationActionsHandler(
                runningApplicationActionRepository,
                _eventsHandler
            );
     
            ApplicationInstaller applicationInstaller = new ApplicationInstaller(
                applicationRepository,
                applicationActionRepository,
                userRepository
            );

            ApplicationResolver applicationResolver = new ApplicationResolver(
                _applicationActionsHandler,
                _eventsHandler,
                _usersHandler
            );

            _applicationsHandler = new ApplicationsHandler(
                applicationRepository,
                applicationResolver,
                applicationInstaller,
                _eventsHandler
            );

            _restAPIHandler = new RestAPIHandler(this, args, _config.RestAPIUserTokenTimeout);
            _tcpServerSocketHandler = new PlatypusServerSocketHandler(this, ProtocolType.Tcp);

            _usersHandler.AddBuiltInConnectionMethod(new PlatypusUserConnectionMethod(), BuiltInUserConnectionMethodGuid.PlatypusUser);
        }

        public void Start()
        {
            _applicationsHandler.LoadApplications();
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
