using PlatypusAPI.Exceptions;
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

        private readonly Dictionary<string, ServerResponseWaiter<StartActionServerResponse>> _startActionServerResponseWaiters;
        private readonly Dictionary<string, ServerResponseWaiter<AddUserServerResponse>> _addUserServerResponseWaiters;
        private readonly Dictionary<string, ServerResponseWaiter<UpdateUserServerResponse>> _updateUserServerResponseWaiters;
        private readonly Dictionary<string, ServerResponseWaiter<RemoveUserServerResponse>> _removeUserServerResponseWaiters;
        private readonly Dictionary<string, ServerResponseWaiter<GetRunningApplicationActionsServerResponse>> _getRunningApplicationActionsServerResponseWaiters;
        private readonly Dictionary<string, ServerResponseWaiter<GetApplicationActionInfosServerResponse>> _getApplicationActionInfosServerResponseWaiter;
        private readonly Dictionary<string, ServerResponseWaiter<ServerResponseBase>> _cancelRunningApplicationActionServerResponseWaiters;

        public PlatypusServerApplication(
            PlatypusClientSocketHandler socketHandler,
            UserAccount connectedUser
        )
        {
            _socketHandler = socketHandler;
            ConnectedUser = connectedUser;

            _startActionServerResponseWaiters = new Dictionary<string, ServerResponseWaiter<StartActionServerResponse>>();
            _addUserServerResponseWaiters = new Dictionary<string, ServerResponseWaiter<AddUserServerResponse>>();
            _updateUserServerResponseWaiters = new Dictionary<string, ServerResponseWaiter<UpdateUserServerResponse>>();
            _removeUserServerResponseWaiters = new Dictionary<string, ServerResponseWaiter<RemoveUserServerResponse>>();
            _getRunningApplicationActionsServerResponseWaiters = new Dictionary<string, ServerResponseWaiter<GetRunningApplicationActionsServerResponse>>();
            _getApplicationActionInfosServerResponseWaiter = new Dictionary<string, ServerResponseWaiter<GetApplicationActionInfosServerResponse>>();
            _cancelRunningApplicationActionServerResponseWaiters = new Dictionary<string, ServerResponseWaiter<ServerResponseBase>>();

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

        private void RunClientRequest<ServerResponseType, ClientRequestType>(Dictionary<string, ServerResponseWaiter<ServerResponseType>> serverResponseWaiters, SocketDataType socketDataType, Action<ClientRequestType> consumer = null, Action<ServerResponseType> callBack = null)
            where ServerResponseType : ServerResponseBase
            where ClientRequestType : ClientRequestBase, new()
        {
            string guid = Utils.GenerateGuidFromEnumerable(serverResponseWaiters.Keys);

            ServerResponseWaiter<ServerResponseType> serverResponseWaiter = new ServerResponseWaiter<ServerResponseType>();

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

        private void ServerResponseCallBack<ServerResponseType>(Dictionary<string, ServerResponseWaiter<ServerResponseType>> serverResponseWaiters, byte[] bytes, Action<ServerResponseWaiter<ServerResponseType>, ServerResponseType> consumer = null)
            where ServerResponseType : ServerResponseBase
        {
            ServerResponseType serverResponse = Utils.GetObjectFromBytes<ServerResponseType>(bytes);
            if (serverResponseWaiters.ContainsKey(serverResponse.RequestKey) == false) return;

            ServerResponseWaiter<ServerResponseType> serverResponseWaiter = serverResponseWaiters[serverResponse.RequestKey];
            serverResponseWaiter.Exception = ExceptionFactory.CreateException(serverResponse.FactorisableExceptionType, serverResponse.FactorisableExceptionParameters);
            serverResponseWaiter.Received = true;

            if(consumer != null)
                consumer(serverResponseWaiter, serverResponse);
        }

        private class ServerResponseWaiter<ResponseType>
            where ResponseType : ServerResponseBase
        {
            public bool Received { get; set; }
            public FactorisableException Exception { get; set; }
            public ResponseType Response { get; set; }
        }
    }
}
