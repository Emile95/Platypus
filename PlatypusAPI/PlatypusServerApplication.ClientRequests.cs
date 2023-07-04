using Common.Sockets;
using Common.Sockets.ClientRequest;
using Common.Sockets.ServerResponse;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.Sockets.ClientRequest;
using PlatypusAPI.Sockets.ServerResponse;
using PlatypusAPI.User;

namespace PlatypusAPI
{
    public partial class PlatypusServerApplication
    {
        public ApplicationActionRunResult RunApplicationAction(ApplicationActionRunParameter applicationActionRunparameter)
        {
            ApplicationActionRunResult result = null;
            RunClientRequest<StartActionServerResponse, StartActionServerResponseWaiter, StartActionClientRequest>(
                 _startActionServerResponseWaiters, SocketDataType.StartApplicationAction,
                 (clientRequest) => {
                     clientRequest.Parameters = applicationActionRunparameter;
                 },
                 (serverResponseWaiter) => {
                     result = serverResponseWaiter.Result;
                 }
            );
            return result;
        }

        public UserAccount AddUser(string credentialMethodGUID, string fullName, string email, Dictionary<string, object> data)
        {
            UserAccount userAccount = null;
            RunClientRequest<AddUserServerResponse, AddUserServerResponseWaiter, AddUserClientRequest>(
                 _addUserServerResponseWaiters, SocketDataType.AddUser,
                 (clientRequest) => {
                     clientRequest.CredentialMethodGUID = credentialMethodGUID;
                     clientRequest.FullName = fullName;
                     clientRequest.Email = email;
                     clientRequest.Data = data;
                 },
                 (serverResponseWaiter) => {
                     userAccount = serverResponseWaiter.UserAccount;
                 }
            );
            return userAccount;
        }

        public List<ApplicationActionRunInfo> GetRunningApplicationActions()
        {
            List<ApplicationActionRunInfo> result = new List<ApplicationActionRunInfo>();
            RunClientRequest<GetRunningApplicationActionsServerResponse, GetRunningApplicationActionsServerResponseWaiter, ClientRequestBase>(
                 _getRunningApplicationActionsServerResponseWaiters, SocketDataType.GetRunningActions, null,
                 (serverResponseWaiter) => {
                     result = serverResponseWaiter.ApplicationActionRunInfos;
                 }
            );
            return result;
        }

        public void CancelRunningApplicationAction(string applicationRunGuid)
        {
            RunClientRequest<ServerResponseBase, ServerResponseWaiter, CancelRunningApplicationRunClientRequest>(
                 _cancelRunningApplicationActionServerResponseWaiters, SocketDataType.CancelRunningAction,
                 (clientRequest) => {
                     clientRequest.ApplicationRunGuid = applicationRunGuid;
                 }
            );
        }
    }
}
