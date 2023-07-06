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
                 _startActionServerResponseWaiters, SocketDataType.RunApplicationAction,
                 (clientRequest) => {
                     clientRequest.Parameters = applicationActionRunparameter;
                 },
                 (serverResponseWaiter) => {
                     result = serverResponseWaiter.Result;
                 }
            );
            return result;
        }

        public UserAccount AddUser(UserCreationParameter userCreationParameter)
        {
            UserAccount userAccount = null;
            RunClientRequest<AddUserServerResponse, AddUserServerResponseWaiter, AddUserClientRequest>(
                 _addUserServerResponseWaiters, SocketDataType.AddUser,
                 (clientRequest) => {
                     clientRequest.ConnectionMethodGuid = userCreationParameter.ConnectionMethodGuid;
                     clientRequest.FullName = userCreationParameter.FullName;
                     clientRequest.Email = userCreationParameter.Email;
                     clientRequest.Data = userCreationParameter.Data;
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
