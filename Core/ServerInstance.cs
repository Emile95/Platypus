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
using Core.User.Abstract;
using PlatypusRepository;
using Core.Persistance.Entity;
using PlatypusRepository.Json;

namespace Core
{
    public partial class ServerInstance
    {
        private ServerConfig _config;

        private ApplicationActionsHandler _applicationActionsHandler;
        private EventsHandler _eventsHandler;
        private UsersHandler _usersHandler;

        private List<IServerStarter> _serverStarters;
        private IUserAuthentificator _serverConnector;
        private ISeverPortListener _restAPIHandler;
        private ISeverPortListener _tcpServerSocketHandler;
        private IApplicationPackageInstaller<PlatypusApplicationBase> _applicationPackageInstaller;
        private IApplicationPackageUninstaller<string> _applicationPackageUninstaller;
        

        public void Initialize(string[] args)
        {
            string json = File.ReadAllText(ApplicationPaths.CONFIGFILEPATH);
            _config = JsonConvert.DeserializeObject<ServerConfig>(json);

            _serverStarters = new List<IServerStarter>();

            ApplicationRepository applicationRepository = new ApplicationRepository();
            ApplicationActionRepository applicationActionRepository = new ApplicationActionRepository();
            RunningApplicationActionRepository runningApplicationActionRepository = new RunningApplicationActionRepository();
            IRepository<UserEntity> userRepository = new JsonRepository<UserEntity>(ApplicationPaths.USERSDIRECTORYPATH);

            UserAuthentificationHandler serverConnectionHandler = new UserAuthentificationHandler(userRepository);
            serverConnectionHandler.AddBuiltInConnectionMethod(new PlatypusUserConnectionMethod(), BuiltInUserConnectionMethodGuid.PlatypusUser);

            _eventsHandler = new EventsHandler();

            _serverConnector = serverConnectionHandler;

            _usersHandler = new UsersHandler(
                userRepository,
                serverConnectionHandler
             );

            _applicationActionsHandler = new ApplicationActionsHandler(
                runningApplicationActionRepository,
                _eventsHandler
            );
     
            ApplicationInstaller applicationInstaller = new ApplicationInstaller(
                applicationRepository,
                applicationRepository,
                applicationActionRepository
            );

            ApplicationResolver applicationResolver = new ApplicationResolver(
                _applicationActionsHandler,
                _eventsHandler,
                serverConnectionHandler
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
            _serverStarters.Add(_applicationActionsHandler);
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
