using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.User;
using PlatypusFramework.Core.ApplicationAction;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusUtils;
using Core.Ressource;
using Core.Persistance.Entity;

namespace Core
{
    public partial class ServerInstance
    {
        public UserAccount UserConnect(UserConnectionParameter parameter)
        {
            return _serverConnector.Connect(parameter.ConnectionMethodGuid, parameter.Credential);
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

            if (_applicationActionsHandler.HasActionWithGuid(runActionParameter.Guid) == false)
            {
                string message = Utils.GetString(Strings.ResourceManager, "ApplicationActionNotFound", runActionParameter.Guid);
                return new ApplicationActionRunResult()
                {
                    Message = message,
                    Status = ApplicationActionRunResultStatus.Failed,
                };
            }
            ApplicationActionEnvironmentBase env = _applicationActionsHandler.CreateStartActionEnvironment(runActionParameter.Guid);

            return _applicationActionsHandler.RunAction(runActionParameter, env);
        }

        public UserAccount AddUser(UserAccount userAccount, UserCreationParameter parameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.UserCRUD);
            UserEntity entity = _usersHandler.Add(new UserEntity()
            {
                ConnectionMethodGuid = parameter.ConnectionMethodGuid,
                Data = parameter.Data,
                Email = parameter.Email,
                FullName = parameter.FullName,
                UserPermissionBits = parameter.UserPermissionFlags
            });
            return new UserAccount()
            {
                Guid = entity.Guid,
                FullName = parameter.FullName,
                Email = parameter.Email,
                UserPermissionFlags = parameter.UserPermissionFlags
            };
        }

        public UserAccount UpdateUser(UserAccount userAccount, UserUpdateParameter parameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.UserCRUD);
            UserEntity entity = _usersHandler.Update(new UserEntity()
            {
                Guid = parameter.Guid,
                ConnectionMethodGuid = parameter.ConnectionMethodGuid,
                Data = parameter.Data,
                Email = parameter.Email,
                FullName = parameter.FullName,
                UserPermissionBits = parameter.UserPermissionFlags
            });
            return new UserAccount()
            {
                Guid = entity.Guid,
                FullName = parameter.FullName,
                Email = parameter.Email,
                UserPermissionFlags = parameter.UserPermissionFlags
            };
        }

        public void RemoveUser(UserAccount userAccount, RemoveUserParameter parameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.UserCRUD);
            _usersHandler.Remove(new UserEntity()
            {
                Guid = parameter.Guid
            });
        }

        public void CancelRunningApplicationAction(UserAccount userAccount, CancelRunningActionParameter parameter)
        {
            ValidateUserForPermission(userAccount, UserPermissionFlag.CancelRunningAction);
            _applicationActionsHandler.CancelRunningAction(parameter);
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
