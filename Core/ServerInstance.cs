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
using Core.Abstract;
using Core.Application.Abstract;
using PlatypusFramework.Configuration.Application;

namespace Core
{
    public partial class ServerInstance
    {
        private ServerConfig _config;

        private ApplicationActionsHandler _applicationActionsHandler;
        private EventsHandler _eventsHandler;
        private UsersHandler _usersHandler;

        private ISeverPortListener _restAPIHandler;
        private ISeverPortListener _tcpServerSocketHandler;

        private IApplicationPackageInstaller<PlatypusApplicationBase> _applicationPackageInstaller;
        private IApplicationPackageUninstaller<string> _applicationPackageUninstaller;

        private List<IServerStarter> _serverStarters;

        public void Initialize(string[] args)
        {
            string json = File.ReadAllText(ApplicationPaths.CONFIGFILEPATH);
            _config = JsonConvert.DeserializeObject<ServerConfig>(json);

            _serverStarters = new List<IServerStarter>();

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
                applicationRepository,
                applicationActionRepository,
                userRepository
            );

            ApplicationResolver applicationResolver = new ApplicationResolver(
                _applicationActionsHandler,
                _eventsHandler,
                _usersHandler
            );

            ApplicationsHandler applicationsHandler = new ApplicationsHandler(
                applicationRepository,
                applicationRepository,
                applicationResolver,
                applicationInstaller,
                applicationActionRepository,
                _eventsHandler
            );

            _restAPIHandler = new RestAPIHandler(this, args, _config.RestAPIUserTokenTimeout);
            _tcpServerSocketHandler = new PlatypusServerSocketHandler(this, ProtocolType.Tcp);

            _applicationPackageInstaller = applicationsHandler;
            _applicationPackageUninstaller = applicationsHandler;

            _serverStarters.Add(applicationsHandler);

            _usersHandler.AddBuiltInConnectionMethod(new PlatypusUserConnectionMethod(), BuiltInUserConnectionMethodGuid.PlatypusUser);
        }

        public void Start()
        {
            foreach(IServerStarter serverStarter in _serverStarters)
                serverStarter.Start();

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
