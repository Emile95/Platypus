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
            ApplicationActionRunResult result = null;
            RunClientRequest<StartActionServerResponse, StartActionClientRequest>(
                 _startActionServerResponseWaiters, SocketDataType.RunApplicationAction,
                 (clientRequest) => {
                     clientRequest.Parameters = applicationActionRunparameter;
                 },
                 (serverResponse) => {
                     result = serverResponse.Result;
                 }
            );
            return result;
        }

        public UserAccount AddUser(UserCreationParameter parameter)
        {
            UserAccount userAccount = null;
            RunClientRequest<AddUserServerResponse, AddUserClientRequest>(
                 _addUserServerResponseWaiters, SocketDataType.AddUser,
                 (clientRequest) => {
                     clientRequest.ConnectionMethodGuid = parameter.ConnectionMethodGuid;
                     clientRequest.FullName = parameter.FullName;
                     clientRequest.Email = parameter.Email;
                     clientRequest.Data = parameter.Data;
                     clientRequest.UserPermissionFlags = parameter.UserPermissionFlags;
                 },
                 (serverResponse) => {
                     userAccount = serverResponse.UserAccount;
                 }
            );
            return userAccount;
        }

        public UserAccount UpdateUser(UserUpdateParameter parameter)
        {
            UserAccount userAccount = null;
            RunClientRequest<UpdateUserServerResponse, UpdateUserClientRequest>(
                 _updateUserServerResponseWaiters, SocketDataType.UpdateUser,
                 (clientRequest) => {
                     clientRequest.UserID = parameter.ID;
                     clientRequest.ConnectionMethodGuid = parameter.ConnectionMethodGuid;
                     clientRequest.FullName = parameter.FullName;
                     clientRequest.Email = parameter.Email;
                     clientRequest.Data = parameter.Data;
                     clientRequest.UserPermissionFlags = parameter.UserPermissionFlags;
                 },
                 (serverResponse) => {
                     userAccount = serverResponse.UserAccount;
                 }
            );
            return userAccount;
        }

        public void RemoveUser(RemoveUserParameter parameter)
        {
            RunClientRequest<RemoveUserServerResponse, RemoveUserClientRequest>(
                 _removeUserServerResponseWaiters, SocketDataType.RemoveUser,
                 (clientRequest) => {
                     clientRequest.ID = parameter.ID;
                     clientRequest.ConnectionMethodGuid = parameter.ConnectionMethodGuid;
                 },
                 (serverResponse) => {}
            );
        }

        public List<ApplicationActionRunInfo> GetRunningApplicationActions()
        {
            List<ApplicationActionRunInfo> result = new List<ApplicationActionRunInfo>();
            RunClientRequest<GetRunningApplicationActionsServerResponse, PlatypusClientRequest>(
                 _getRunningApplicationActionsServerResponseWaiters, SocketDataType.GetRunningActions, null,
                 (serverResponse) => {
                     result = serverResponse.ApplicationActionRunInfos;
                 }
            );
            return result;
        }

        public List<ApplicationActionInfo> GetApplicationActionInfos()
        {
            List<ApplicationActionInfo> result = new List<ApplicationActionInfo>();
            RunClientRequest<GetApplicationActionInfosServerResponse, PlatypusClientRequest>(
                 _getApplicationActionInfosServerResponseWaiter, SocketDataType.GetActionInfos, null,
                 (serverResponse) => {
                     result = serverResponse.ApplicationActionInfos;
                 }
            );
            return result;
        }

        public void CancelRunningApplicationAction(string applicationRunGuid)
        {
            RunClientRequest<PlatypusServerResponse, CancelRunningApplicationRunClientRequest>(
                 _cancelRunningApplicationActionServerResponseWaiters, SocketDataType.CancelRunningAction,
                 (clientRequest) => {
                     clientRequest.ApplicationRunGuid = applicationRunGuid;
                 }
            );
        }
    }
}
