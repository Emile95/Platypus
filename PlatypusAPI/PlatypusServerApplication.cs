using Common.Exceptions;
using Common.Sockets;
using Common.Sockets.ClientRequest;
using Common.Sockets.ServerResponse;
using PlatypusAPI.Exceptions;
using PlatypusAPI.User;
using Utils.GuidGeneratorHelper;

namespace PlatypusAPI
{
    public partial class PlatypusServerApplication
    {
        private readonly PlatypusClientSocketHandler _socketHandler;
        public UserAccount ConnectedUser { get; private set; }

        private readonly Dictionary<string, StartActionServerResponseWaiter> _startActionServerResponseWaiters;
        private readonly Dictionary<string, AddUserServerResponseWaiter> _addUserServerResponseWaiters;
        private readonly Dictionary<string, GetRunningApplicationActionsServerResponseWaiter> _getRunningApplicationActionsServerResponseWaiters;
        private readonly Dictionary<string, ServerResponseWaiter> _cancelRunningApplicationActionServerResponseWaiters;

        public PlatypusServerApplication(
            PlatypusClientSocketHandler socketHandler,
            UserAccount connectedUser
        )
        {
            _socketHandler = socketHandler;
            ConnectedUser = connectedUser;

            _startActionServerResponseWaiters = new Dictionary<string, StartActionServerResponseWaiter>();
            _addUserServerResponseWaiters = new Dictionary<string, AddUserServerResponseWaiter>();
            _getRunningApplicationActionsServerResponseWaiters = new Dictionary<string, GetRunningApplicationActionsServerResponseWaiter>();
            _cancelRunningApplicationActionServerResponseWaiters = new Dictionary<string, ServerResponseWaiter>();

            _socketHandler.ServerResponseCallBacks[SocketDataType.RunApplicationAction].Add(StartApplicationServerResponseCallBack);
            _socketHandler.ServerResponseCallBacks[SocketDataType.AddUser].Add(AddUserServerResponseCallBack);
            _socketHandler.ServerResponseCallBacks[SocketDataType.GetRunningActions].Add(GetRunningApplicationActionsServerResponseCallBack);
            _socketHandler.ServerResponseCallBacks[SocketDataType.CancelRunningAction].Add(CancelRunningApplicationActionServerResponseCallBack);
        }

        public void Disconnect()
        {

        }

        private void RunClientRequest<ServerResponseType, ServerResponseWaiterType, ClientRequestType>(Dictionary<string, ServerResponseWaiterType> serverResponseWaiters, SocketDataType socketDataType, Action<ClientRequestType> consumer = null, Action<ServerResponseWaiterType> callBack = null)
            where ServerResponseType : ServerResponseBase
            where ServerResponseWaiterType : ServerResponseWaiter, new()
            where ClientRequestType : ClientRequestBase, new()
        {
            string guid = GuidGenerator.GenerateFromEnumerable(serverResponseWaiters.Keys);

            ServerResponseWaiterType serverResponseWaiter = new ServerResponseWaiterType();

            serverResponseWaiters.Add(guid, serverResponseWaiter);

            ClientRequestType clientRequest = new ClientRequestType()
            {
                RequestKey = guid
            };

            if(consumer != null)
                consumer(clientRequest);

            SocketData clientRequestData = new SocketData()
            {
                SocketDataType = socketDataType,
                Data = Common.Utils.GetBytesFromObject(clientRequest)
            };

            _socketHandler.SendToServer(Common.Utils.GetBytesFromObject(clientRequestData));

            while (serverResponseWaiter.Received == false) ;

            if(callBack != null)
                callBack(serverResponseWaiter);

            serverResponseWaiters.Remove(guid);
        }

        private void ServerResponseCallBack<ServerResponseType, ServerResponseWaiterType>(Dictionary<string, ServerResponseWaiterType> serverResponseWaiters, byte[] bytes, Action<ServerResponseWaiterType, ServerResponseType> consumer = null)
            where ServerResponseType : ServerResponseBase
            where ServerResponseWaiterType : ServerResponseWaiter
        {
            ServerResponseType serverResponse = Common.Utils.GetObjectFromBytes<ServerResponseType>(bytes);
            if (serverResponseWaiters.ContainsKey(serverResponse.RequestKey) == false) return;
            
            ServerResponseWaiterType serverResponseWaiter = serverResponseWaiters[serverResponse.RequestKey];
            serverResponseWaiter.Exception = ExceptionFactory.CreateException(serverResponse.FactorisableExceptionType, serverResponse.FactorisableExceptionParameters);
            serverResponseWaiter.Received = true;

            if(consumer != null)
                consumer(serverResponseWaiter, serverResponse);
        }

        private class ServerResponseWaiter
        {
            public bool Received { get; set; }
            public FactorisableException Exception { get; set; }
        }
    }
}
