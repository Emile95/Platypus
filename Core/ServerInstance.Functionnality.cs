using Core.Application;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.User;
using PlatypusApplicationFramework.Core.ApplicationAction;
using PlatypusApplicationFramework.Core.Event;

namespace Core
{
    public partial class ServerInstance
    {
        public UserAccount UserConnect(Dictionary<string, object> credential, string connectionMethodGuid)
        {
            return _usersHandler.Connect(credential, connectionMethodGuid);
        }

        public void InstallApplication(UserAccount userAccount, string applicationPath)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.InstallAndUninstallApplication);

            _applicationsHandler.InstallApplication(applicationPath);
        }

        public void UninstalApplication(UserAccount userAccount, string applicationGuid)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.InstallAndUninstallApplication);

            UninstallApplicationDetails details = _applicationsHandler.UninstallApplication(applicationGuid);
            foreach (string actionGuid in details.ActionGuids)
                _applicationActionsHandler.RemoveAction(actionGuid);
            foreach (string userConnectionMethodGuid in details.UserConnectionMethodGuids)
                _usersHandler.RemoveConnectionMethod(userConnectionMethodGuid);

            _eventsHandler.RunEventHandlers<object>(EventHandlerType.AfterUninstallApplication, details.EventEnv, (exception) => throw exception);
        }

        public ApplicationActionRunResult RunAction(UserAccount userAccount, ApplicationActionRunParameter runActionParameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.RunAction);

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

            return _applicationActionsHandler.RunAction(runActionParameter, env);
        }

        public UserAccount AddUser(UserAccount userAccount, UserCreationParameter userCreationParameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.AddUser);
            return _usersHandler.AddUser(userCreationParameter);
        }

        public void CancelRunningApplicationAction(UserAccount userAccount, string guid)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.CancelRunningAction);
            _applicationActionsHandler.CancelRunningAction(guid);
        }

        public IEnumerable<ApplicationActionRunInfo> GetRunningApplicationActions(UserAccount userAccount)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.GetRunningActions);
            return _applicationActionsHandler.GetRunningApplicationActionInfos();
        }

        public IEnumerable<ApplicationActionInfo> GetApplicationActionInfos(UserAccount userAccount)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.GetActionsInfo);
            return _applicationActionsHandler.GetApplicationActionInfos();
        }
    }
}
