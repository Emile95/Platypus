using PlatypusAPI.Exceptions;
using PlatypusAPI.User;
using Core.Network.RestAPI;
using Core.Network;
using Core.Abstract;
using Core.Application.Abstract;
using Core.User.Abstract;
using PlatypusRepository;
using Core.ApplicationAction.Abstract;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;
using Core.Application;
using Core.ApplicationAction;
using Core.Persistance;
using Newtonsoft.Json;
using System.Net.Sockets;
using Core.Persistance.Entity;
using Core.Network.Abstract;
using PlatypusContainer.Service;
using PlatypusAPI.Application;

namespace Core
{
    public partial class ServerInstance : IHostedService
    {
        private readonly ServerConfig _config;
        private readonly ISeverPortListener<RestAPIHandler> _restAPIHandler;
        private readonly ISeverPortListener<PlatypusServerSocketHandler> _tcpServerSocketHandler;
        private readonly IServerStarter<ApplicationsHandler> _applicationsServerStarter;
        private readonly IServerStarter<RunningApplicationActionEntity> _runningApplicationActionsServerStarter;
        private readonly IUserAuthentificator _userAuthentificator;
        private readonly IRepositoryAddOperator<UserCreationParameter> _userAddOperator;
        private readonly IRepositoryUpdateOperator<UserUpdateParameter> _userUpdateOperator;
        private readonly IRepositoryRemoveOperator<RemoveUserParameter, string> _userRemoveOperator;
        private readonly IRepositoryRemoveOperator<ApplicationActionRun, string> _applicationActionRunRemoveOperator;
        private readonly IRepositoryConsumeOperator<ApplicationActionRunInfo> _actionRunInfoConsumer;
        private readonly IRepositoryConsumeOperator<ApplicationActionInfo> _actionInfoConsumer;
        private readonly IRepositoryConsumeOperator<ApplicationInfo> _applicationInfoConsumer;
        private readonly IApplicationInstaller _applicationPackageInstaller;
        private readonly IApplicationUninstaller _applicationPackageUninstaller;
        private readonly IApplicationActionRunner _applicationActionRunner;

        public ServerInstance(
            IServerStarter<ApplicationsHandler> applicationsServerStarter,
            IServerStarter<RunningApplicationActionEntity> runningApplicationActionsServerStarter,
            IUserAuthentificator userAuthentificator,
            IRepositoryAddOperator<UserCreationParameter> userAddOperator,
            IRepositoryUpdateOperator<UserUpdateParameter> userUpdateOperator,
            IRepositoryRemoveOperator<RemoveUserParameter, string> userRemoveOperator,
            IRepositoryRemoveOperator<ApplicationActionRun, string> applicationActionRunRemoveOperator,
            IRepositoryConsumeOperator<ApplicationActionRunInfo> actionRunInfoConsumer,
            IRepositoryConsumeOperator<ApplicationActionInfo> actionInfoConsumer,
            IRepositoryConsumeOperator<ApplicationInfo> applicationInfoConsumer,
            IApplicationInstaller applicationPackageInstaller,
            IApplicationUninstaller applicationPackageUninstaller,
            IApplicationActionRunner applicationActionRunner
        )
        {
            string json = File.ReadAllText(ApplicationPaths.CONFIGFILEPATH);
            _config = JsonConvert.DeserializeObject<ServerConfig>(json);
            _restAPIHandler = new RestAPIHandler(this, _config.RestAPIUserTokenTimeout);
            _tcpServerSocketHandler = new PlatypusServerSocketHandler(this, ProtocolType.Tcp);

            _applicationsServerStarter = applicationsServerStarter;
            _runningApplicationActionsServerStarter = runningApplicationActionsServerStarter;
            _userAuthentificator = userAuthentificator;
            _userAddOperator = userAddOperator;
            _userUpdateOperator = userUpdateOperator;
            _userRemoveOperator = userRemoveOperator;
            _applicationActionRunRemoveOperator = applicationActionRunRemoveOperator;
            _actionRunInfoConsumer = actionRunInfoConsumer;
            _actionInfoConsumer = actionInfoConsumer;
            _applicationInfoConsumer = applicationInfoConsumer;
            _applicationPackageInstaller = applicationPackageInstaller;
            _applicationPackageUninstaller = applicationPackageUninstaller;
            _applicationActionRunner = applicationActionRunner;
        }

        public Task RunAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                _tcpServerSocketHandler.InitializeServerPortListener(_config.TcpSocketPort);
                _restAPIHandler.InitializeServerPortListener(_config.HttpPort);

                _applicationsServerStarter.Start();
                _runningApplicationActionsServerStarter.Start();
            });
        }

        private void ValidateUserForPermission(UserAccount userAccount, UserPermissionFlag userPermissionFlag)
        {
            if (userAccount.UserPermissionFlags == UserPermissionFlag.Admin) return;

            if (userAccount.UserPermissionFlags.HasFlag(userPermissionFlag) == false)
                throw new UserPermissionException(userAccount);
        }
    }
}
