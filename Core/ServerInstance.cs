using PlatypusAPI.Exceptions;
using PlatypusAPI.User;
using Core.Network.RestAPI;
using Core.Network;
using Core.Abstract;
using Core.Application.Abstract;
using PlatypusFramework.Configuration.Application;
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
using Microsoft.Extensions.Hosting;
using Core.Persistance.Entity;

namespace Core
{
    public partial class ServerInstance : BackgroundService
    {
        private readonly ServerConfig _config;
        private readonly IServerStarter<ApplicationsHandler> _applicationsServerStarter;
        private readonly IServerStarter<RunningApplicationActionEntity> _runningApplicationActionsServerStarter;
        private readonly ISeverPortListener<RestAPIHandler> _restAPIHandler;
        private readonly ISeverPortListener<PlatypusServerSocketHandler> _tcpServerSocketHandler;
        private readonly IUserAuthentificator _userAuthentificator;
        private readonly IRepositoryAddOperator<UserCreationParameter> _userAddOperator;
        private readonly IRepositoryUpdateOperator<UserUpdateParameter> _userUpdateOperator;
        private readonly IRepositoryRemoveOperator<RemoveUserParameter, string> _userRemoveOperator;
        private readonly IRepositoryRemoveOperator<ApplicationActionRun, string> _applicationActionRunRemoveOperator;
        private readonly IRepositoryConsumeOperator<ApplicationActionRunInfo> _actionRunInfoConsumer;
        private readonly IRepositoryConsumeOperator<ApplicationActionInfo> _actionInfoConsumer;
        private readonly IRepositoryAddOperator<PlatypusApplicationBase> _applicationAddOperator;
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
            IRepositoryAddOperator<PlatypusApplicationBase> applicationAddOperator,
            IApplicationInstaller applicationPackageInstaller,
            IApplicationUninstaller applicationPackageUninstaller,
            IApplicationActionRunner applicationActionRunner
        )
        {
            string json = File.ReadAllText(ApplicationPaths.CONFIGFILEPATH);
            _config = JsonConvert.DeserializeObject<ServerConfig>(json);

            _applicationsServerStarter = applicationsServerStarter;
            _runningApplicationActionsServerStarter = runningApplicationActionsServerStarter;
            _userAuthentificator = userAuthentificator;
            _userAddOperator = userAddOperator;
            _userUpdateOperator = userUpdateOperator;
            _userRemoveOperator = userRemoveOperator;
            _applicationActionRunRemoveOperator = applicationActionRunRemoveOperator;
            _actionRunInfoConsumer = actionRunInfoConsumer;
            _actionInfoConsumer = actionInfoConsumer;
            _applicationAddOperator = applicationAddOperator;
            _applicationPackageInstaller = applicationPackageInstaller;
            _applicationPackageUninstaller = applicationPackageUninstaller;
            _applicationActionRunner = applicationActionRunner;
            _restAPIHandler = new RestAPIHandler(this, _config.RestAPIUserTokenTimeout);
            _tcpServerSocketHandler = new PlatypusServerSocketHandler(this, ProtocolType.Tcp);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
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
