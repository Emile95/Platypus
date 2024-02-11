using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.User;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusAPI.Application;

namespace Core
{
    public partial class ServerInstance
    {
        public UserAccount UserConnect(UserConnectionParameter parameter)
        {
            return _userAuthentificator.Authentify(parameter.ConnectionMethodGuid, parameter.Credential);
        }

        public bool InstallApplication(UserAccount userAccount, InstallApplicationParameter parameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.InstallAndUninstallApplication);
            _applicationPackageInstaller.Install(parameter.DllFilePath);
            return true;
        }

        public void UninstalApplication(UserAccount userAccount, UninstallApplicationParameter parameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.InstallAndUninstallApplication);
            _applicationPackageUninstaller.Uninstall(parameter.ApplicationGuid);
        }

        public ApplicationActionRunResult RunAction(UserAccount userAccount, ApplicationActionRunParameter runActionParameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.RunAction);
            return _applicationActionRunner.Run(runActionParameter);
        }

        public UserAccount AddUser(UserAccount userAccount, UserCreationParameter parameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.UserCRUD);
            _userAddOperator.Add(parameter);

            return new UserAccount()
            {
                FullName = parameter.FullName,
                Email = parameter.Email,
                UserPermissionFlags = parameter.UserPermissionFlags
            };
        }

        public UserAccount UpdateUser(UserAccount userAccount, UserUpdateParameter parameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.UserCRUD);
            _userUpdateOperator.Update(parameter);

            return new UserAccount()
            {
                Guid = parameter.Guid,
                FullName = parameter.FullName,
                Email = parameter.Email,
                UserPermissionFlags = parameter.UserPermissionFlags
            };
        }

        public void RemoveUser(UserAccount userAccount, RemoveUserParameter parameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.UserCRUD);
            _userRemoveOperator.Remove(parameter.Guid);
        }

        public void CancelRunningApplicationAction(UserAccount userAccount, CancelRunningActionParameter parameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.CancelRunningAction);
            _applicationActionRunRemoveOperator.Remove(parameter.Guid);
        }

        public IEnumerable<ApplicationInfo> GetApplicationInfos(UserAccount userAccount)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.GetApplicationInfos);
            List<ApplicationInfo> infos = new List<ApplicationInfo>();
            _applicationInfoConsumer.Consume((applicationInfo) => infos.Add(applicationInfo));
            return infos;
        }

        public IEnumerable<ApplicationActionRunInfo> GetRunningApplicationActions(UserAccount userAccount)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.GetRunningActions);
            List<ApplicationActionRunInfo> runInfos = new List<ApplicationActionRunInfo>();
            _actionRunInfoConsumer.Consume((runIfo) => runInfos.Add(runIfo));
            return runInfos;
        }

        public IEnumerable<ApplicationActionInfo> GetApplicationActionInfos(UserAccount userAccount)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.GetActionsInfo);
            List<ApplicationActionInfo> actionInfos = new List<ApplicationActionInfo>();
            _actionInfoConsumer.Consume((actionInfo) => actionInfos.Add(actionInfo));
            return actionInfos;
        }
    }
}
