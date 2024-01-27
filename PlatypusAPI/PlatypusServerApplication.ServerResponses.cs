using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.Network.ServerResponse;
using PlatypusAPI.User;

namespace PlatypusAPI
{
    public partial class PlatypusServerApplication
    {
        private void StartApplicationServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack<StartActionServerResponse, StartActionServerResponseWaiter>(
                _startActionServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.Result = serverResponse.Result;
                }
            );
        }

        private void AddUserServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack<AddUserServerResponse, AddUserServerResponseWaiter>(
                _addUserServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.UserAccount = serverResponse.UserAccount;
                }
            );
        }

        private void UpdateUserServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack<UpdateUserServerResponse, UpdateUserServerResponseWaiter>(
                _updateUserServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.UserAccount = serverResponse.UserAccount;
                }
            );
        }

        private void RemoveUserServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack<RemoveUserServerResponse, RemoveUserServerResponseWaiter>(
                _removeUserServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => { }
            );
        }

        private void GetRunningApplicationActionsServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack<GetRunningApplicationActionsServerResponse, GetRunningApplicationActionsServerResponseWaiter>(
                _getRunningApplicationActionsServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.ApplicationActionRunInfos = serverResponse.ApplicationActionRunInfos;
                }
            );
        }

        private void GetApplicationActionInfosServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack<GetApplicationActionInfosServerResponse, GetApplicationActionInfosServerResponseWaiter>(
                _getApplicationActionInfosServerResponseWaiter, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.ApplicationActionInfos = serverResponse.ApplicationActionInfos;
                }
            );
        }

        private void CancelRunningApplicationActionServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack<ServerResponseBase, ServerResponseWaiter>(
                _cancelRunningApplicationActionServerResponseWaiters, bytes
            );
        }

        private class StartActionServerResponseWaiter : ServerResponseWaiter
        {
            public ApplicationActionRunResult Result { get; set; }
        }

        private class AddUserServerResponseWaiter : ServerResponseWaiter
        {
            public UserAccount UserAccount { get; set; }
        }

        private class UpdateUserServerResponseWaiter : ServerResponseWaiter
        {
            public UserAccount UserAccount { get; set; }
        }

        private class RemoveUserServerResponseWaiter : ServerResponseWaiter {}

        private class GetRunningApplicationActionsServerResponseWaiter : ServerResponseWaiter
        {
            public List<ApplicationActionRunInfo> ApplicationActionRunInfos { get; set; }
        }

        private class GetApplicationActionInfosServerResponseWaiter : ServerResponseWaiter
        {
            public List<ApplicationActionInfo> ApplicationActionInfos { get; set; }
        }
    }
}
