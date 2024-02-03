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
using Core.ApplicationAction.Abstract;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;

namespace Core
{
    public partial class ServerInstance
    {
        private ServerConfig _config;
        private List<IServerStarter> _serverStarters;
        private ISeverPortListener _restAPIHandler;
        private ISeverPortListener _tcpServerSocketHandler;
        private IUserAuthentificator _userAuthentificator;
        private IRepositoryAddOperator<UserCreationParameter> _userAddOperator;
        private IRepositoryUpdateOperator<UserUpdateParameter> _userUpdateOperator;
        private IRepositoryRemoveOperator<RemoveUserParameter> _userRemoveOperator;
        private IRepositoryRemoveOperator<CancelRunningActionParameter> _cancelRunningActionOperator;
        private IRepositoryConsumeOperator<ApplicationActionRunInfo> _actionRunInfoConsumer;
        private IRepositoryConsumeOperator<ApplicationActionInfo> _actionInfoConsumer;
        private IApplicationPackageInstaller<PlatypusApplicationBase> _applicationPackageInstaller;
        private IApplicationPackageUninstaller<string> _applicationPackageUninstaller;
        private IApplicationActionRunner _applicationActionRunner;
        
        private EventsHandler _eventsHandler;

        public void Initialize(string[] args)
        {
            string json = File.ReadAllText(ApplicationPaths.CONFIGFILEPATH);
            _config = JsonConvert.DeserializeObject<ServerConfig>(json);

            _serverStarters = new List<IServerStarter>();

            ApplicationRepository applicationRepository = new ApplicationRepository();
            ApplicationActionRepository applicationActionRepository = new ApplicationActionRepository();
            RunningApplicationActionRepository runningApplicationActionRepository = new RunningApplicationActionRepository();
            IRepository<UserEntity> userRepository = new JsonRepository<UserEntity>(ApplicationPaths.USERSDIRECTORYPATH);

            UserAuthentificationHandler userAuthentificationHandler = new UserAuthentificationHandler(userRepository);
            userAuthentificationHandler.AddBuiltInConnectionMethod(new PlatypusUserConnectionMethod(), BuiltInUserConnectionMethodGuid.PlatypusUser);

            _eventsHandler = new EventsHandler();

            _userAuthentificator = userAuthentificationHandler;

            UsersHandler usersHandler = new UsersHandler(
                userRepository,
                userAuthentificationHandler
             );

            _userAddOperator = usersHandler;
            _userUpdateOperator = usersHandler;
            _userRemoveOperator = usersHandler;

            ApplicationActionsHandler applicationActionsHandler = new ApplicationActionsHandler(
                runningApplicationActionRepository,
                runningApplicationActionRepository,
                runningApplicationActionRepository,
                applicationActionRepository,
                applicationActionRepository,
                _eventsHandler
            );

            _cancelRunningActionOperator = applicationActionsHandler;
            _actionRunInfoConsumer = applicationActionsHandler;
            _actionInfoConsumer = applicationActionsHandler;
            _applicationActionRunner = applicationActionsHandler;

            ApplicationInstaller applicationInstaller = new ApplicationInstaller(
                applicationRepository,
                applicationRepository,
                applicationActionRepository
            );

            ApplicationResolver applicationResolver = new ApplicationResolver(
                applicationActionsHandler,
                _eventsHandler,
                userAuthentificationHandler
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
            _serverStarters.Add(applicationActionsHandler);
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
