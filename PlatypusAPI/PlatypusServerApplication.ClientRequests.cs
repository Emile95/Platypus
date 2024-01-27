using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.Network;
using PlatypusAPI.Network.ClientRequest;
using PlatypusAPI.Network.ServerResponse;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusAPI.User;

namespace PlatypusAPI
{
    public partial class PlatypusServerApplication
    {
        public ApplicationActionRunResult RunApplicationAction(ApplicationActionRunParameter applicationActionRunparameter)
        {
            StartActionServerResponse response = _socketHandler.HandleClientRequest<StartActionClientRequest, StartActionServerResponse>(
                RequestType.RunApplicationAction,
                (clientRequest) => clientRequest.Parameters = applicationActionRunparameter
            );

            return response.Result;
        }

        public UserAccount AddUser(UserCreationParameter parameter)
        {
            AddUserServerResponse response = _socketHandler.HandleClientRequest<AddUserClientRequest, AddUserServerResponse>(
                RequestType.AddUser,
                (clientRequest) => {
                    clientRequest.ConnectionMethodGuid = parameter.ConnectionMethodGuid;
                    clientRequest.FullName = parameter.FullName;
                    clientRequest.Email = parameter.Email;
                    clientRequest.Data = parameter.Data;
                    clientRequest.UserPermissionFlags = parameter.UserPermissionFlags;
                }
            );

            return response.UserAccount;
        }

        public UserAccount UpdateUser(UserUpdateParameter parameter)
        {
            UpdateUserServerResponse response = _socketHandler.HandleClientRequest<UpdateUserClientRequest, UpdateUserServerResponse>(
                RequestType.UpdateUser,
                (clientRequest) => {
                    clientRequest.UserID = parameter.ID;
                    clientRequest.ConnectionMethodGuid = parameter.ConnectionMethodGuid;
                    clientRequest.FullName = parameter.FullName;
                    clientRequest.Email = parameter.Email;
                    clientRequest.Data = parameter.Data;
                    clientRequest.UserPermissionFlags = parameter.UserPermissionFlags;
                }
            );

            return response.UserAccount;
        }

        public void RemoveUser(RemoveUserParameter parameter)
        {
            RemoveUserServerResponse response = _socketHandler.HandleClientRequest<RemoveUserClientRequest, RemoveUserServerResponse>(
                RequestType.RemoveUser,
                (clientRequest) => {
                    clientRequest.ID = parameter.ID;
                    clientRequest.ConnectionMethodGuid = parameter.ConnectionMethodGuid;
                }
            );
        }

        public List<ApplicationActionRunInfo> GetRunningApplicationActions()
        {
            GetRunningApplicationActionsServerResponse response = _socketHandler.HandleClientRequest<PlatypusClientRequest, GetRunningApplicationActionsServerResponse>(RequestType.GetRunningActions);
            return response.ApplicationActionRunInfos;
        }

        public List<ApplicationActionInfo> GetApplicationActionInfos()
        {
            GetApplicationActionInfosServerResponse response = _socketHandler.HandleClientRequest<PlatypusClientRequest, GetApplicationActionInfosServerResponse>(RequestType.GetActionInfos);
            return response.ApplicationActionInfos;
        }

        public void CancelRunningApplicationAction(string applicationRunGuid)
        {
            PlatypusServerResponse response = _socketHandler.HandleClientRequest<CancelRunningApplicationRunClientRequest, PlatypusServerResponse>(
                RequestType.CancelRunningAction, 
                (clientRequest) => {
                    clientRequest.ApplicationRunGuid = applicationRunGuid;
                }
            );
        }
    }
}
