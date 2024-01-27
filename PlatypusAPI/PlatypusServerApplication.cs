using PlatypusAPI.User;
using PlatypusUtils;
using PlatypusAPI.Network;
using PlatypusAPI.Network.ServerResponse;
using PlatypusAPI.Network.ClientRequest;

namespace PlatypusAPI
{
    public partial class PlatypusServerApplication
    {
        private readonly PlatypusClientSocketHandler _socketHandler;
        public UserAccount ConnectedUser { get; private set; }

        private readonly Dictionary<string, PlatypusServerResponseWaiter<StartActionServerResponse>> _startActionServerResponseWaiters;
        private readonly Dictionary<string, PlatypusServerResponseWaiter<AddUserServerResponse>> _addUserServerResponseWaiters;
        private readonly Dictionary<string, PlatypusServerResponseWaiter<UpdateUserServerResponse>> _updateUserServerResponseWaiters;
        private readonly Dictionary<string, PlatypusServerResponseWaiter<RemoveUserServerResponse>> _removeUserServerResponseWaiters;
        private readonly Dictionary<string, PlatypusServerResponseWaiter<GetRunningApplicationActionsServerResponse>> _getRunningApplicationActionsServerResponseWaiters;
        private readonly Dictionary<string, PlatypusServerResponseWaiter<GetApplicationActionInfosServerResponse>> _getApplicationActionInfosServerResponseWaiter;
        private readonly Dictionary<string, PlatypusServerResponseWaiter<PlatypusServerResponse>> _cancelRunningApplicationActionServerResponseWaiters;

        public PlatypusServerApplication(
            PlatypusClientSocketHandler socketHandler,
            UserAccount connectedUser
        )
        {
            _socketHandler = socketHandler;
            ConnectedUser = connectedUser;

            _startActionServerResponseWaiters = new Dictionary<string, PlatypusServerResponseWaiter<StartActionServerResponse>>();
            _addUserServerResponseWaiters = new Dictionary<string, PlatypusServerResponseWaiter<AddUserServerResponse>>();
            _updateUserServerResponseWaiters = new Dictionary<string, PlatypusServerResponseWaiter<UpdateUserServerResponse>>();
            _removeUserServerResponseWaiters = new Dictionary<string, PlatypusServerResponseWaiter<RemoveUserServerResponse>>();
            _getRunningApplicationActionsServerResponseWaiters = new Dictionary<string, PlatypusServerResponseWaiter<GetRunningApplicationActionsServerResponse>>();
            _getApplicationActionInfosServerResponseWaiter = new Dictionary<string, PlatypusServerResponseWaiter<GetApplicationActionInfosServerResponse>>();
            _cancelRunningApplicationActionServerResponseWaiters = new Dictionary<string, PlatypusServerResponseWaiter<PlatypusServerResponse>>();

            _socketHandler.ServerResponseCallBacks[SocketDataType.RunApplicationAction].Add(StartApplicationServerResponseCallBack);
            _socketHandler.ServerResponseCallBacks[SocketDataType.AddUser].Add(AddUserServerResponseCallBack);
            _socketHandler.ServerResponseCallBacks[SocketDataType.UpdateUser].Add(UpdateUserServerResponseCallBack);
            _socketHandler.ServerResponseCallBacks[SocketDataType.RemoveUser].Add(RemoveUserServerResponseCallBack);
            _socketHandler.ServerResponseCallBacks[SocketDataType.GetRunningActions].Add(GetRunningApplicationActionsServerResponseCallBack);
            _socketHandler.ServerResponseCallBacks[SocketDataType.GetActionInfos].Add(GetApplicationActionInfosServerResponseCallBack);
            _socketHandler.ServerResponseCallBacks[SocketDataType.CancelRunningAction].Add(CancelRunningApplicationActionServerResponseCallBack);
            
        }

        public void Disconnect()
        {

        }

        private void RunClientRequest<ServerResponseType, ClientRequestType>(Dictionary<string, PlatypusServerResponseWaiter<ServerResponseType>> serverResponseWaiters, SocketDataType socketDataType, Action<ClientRequestType> consumer = null, Action<ServerResponseType> callBack = null)
            where ServerResponseType : PlatypusServerResponse
            where ClientRequestType : PlatypusClientRequest, new()
        {
            string guid = Utils.GenerateGuidFromEnumerable(serverResponseWaiters.Keys);

            PlatypusServerResponseWaiter<ServerResponseType> serverResponseWaiter = new PlatypusServerResponseWaiter<ServerResponseType>();

            serverResponseWaiters.Add(guid, serverResponseWaiter);

            ClientRequestType clientRequest = new ClientRequestType()
            {
                RequestKey = guid,
                UserAccount = ConnectedUser
            };

            if(consumer != null)
                consumer(clientRequest);

            SocketData clientRequestData = new SocketData()
            {
                SocketDataType = socketDataType,
                Data = Utils.GetBytesFromObject(clientRequest)
            };

            _socketHandler.SendToServer(Utils.GetBytesFromObject(clientRequestData));

            while (serverResponseWaiter.Received == false) Thread.Sleep(200);

            if (serverResponseWaiter.Exception != null)
                throw serverResponseWaiter.Exception;

            if (callBack != null)
                callBack(serverResponseWaiter.Response);

            serverResponseWaiters.Remove(guid);
        }

        private void ServerResponseCallBack<ServerResponseType>(Dictionary<string, PlatypusServerResponseWaiter<ServerResponseType>> serverResponseWaiters, byte[] bytes, Action<PlatypusServerResponseWaiter<ServerResponseType>, ServerResponseType> consumer = null)
            where ServerResponseType : PlatypusServerResponse
        {
            ServerResponseType serverResponse = Utils.GetObjectFromBytes<ServerResponseType>(bytes);
            if (serverResponseWaiters.ContainsKey(serverResponse.RequestKey) == false) return;

            PlatypusServerResponseWaiter<ServerResponseType> serverResponseWaiter = serverResponseWaiters[serverResponse.RequestKey];
            //serverResponseWaiter.Exception = ExceptionFactory.CreateException(serverResponse.FactorisableExceptionType, serverResponse.FactorisableExceptionParameters);
            serverResponseWaiter.Received = true;

            if(consumer != null)
                consumer(serverResponseWaiter, serverResponse);
        }

        /*private class ServerResponseWaiter<ResponseType>
            where ResponseType : ServerResponseBase
        {
            public bool Received { get; set; }
            public FactorisableException Exception { get; set; }
            public ResponseType Response { get; set; }
        }*/
    }
}
