namespace PlatypusAPI
{
    public partial class PlatypusServerApplication
    {
        private void StartApplicationServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack(
                _startActionServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.Response.Result = serverResponse.Result;
                }
            );
        }

        private void AddUserServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack(
                _addUserServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.Response.UserAccount = serverResponse.UserAccount;
                }
            );
        }

        private void UpdateUserServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack(
                _updateUserServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.Response.UserAccount = serverResponse.UserAccount;
                }
            );
        }

        private void RemoveUserServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack(
                _removeUserServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => { }
            );
        }

        private void GetRunningApplicationActionsServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack(
                _getRunningApplicationActionsServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.Response.ApplicationActionRunInfos = serverResponse.ApplicationActionRunInfos;
                }
            );
        }

        private void GetApplicationActionInfosServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack(
                _getApplicationActionInfosServerResponseWaiter, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.Response.ApplicationActionInfos = serverResponse.ApplicationActionInfos;
                }
            );
        }

        private void CancelRunningApplicationActionServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack(
                _cancelRunningApplicationActionServerResponseWaiters, bytes
            );
        }
    }
}
