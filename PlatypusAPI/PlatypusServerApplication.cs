using Common.Exceptions;
using Common.Sockets;
using Common.Sockets.ClientRequest;
using Common.Sockets.ServerResponse;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.Exceptions;
using PlatypusAPI.Sockets.ClientRequest;
using PlatypusAPI.Sockets.ServerResponse;
using PlatypusAPI.User;
using Utils.GuidGeneratorHelper;

namespace PlatypusAPI
{
    public class PlatypusServerApplication
    {
        private readonly PlatypusClientSocketHandler _socketHandler;
        public UserAccount ConnectedUser { get; private set; }

        private readonly Dictionary<string, StartActionServerResponseWaiter> _startActionServerResponseWaiters;

        public PlatypusServerApplication(
            PlatypusClientSocketHandler socketHandler,
            UserAccount connectedUser
        )
        {
            _socketHandler = socketHandler;
            ConnectedUser = connectedUser;
            _startActionServerResponseWaiters = new Dictionary<string, StartActionServerResponseWaiter>();

            _socketHandler.ServerResponseCallBacks[SocketDataType.StartApplicationAction].Add(StartApplicationServerResponseCallBack);
        }

        public void Disconnect()
        {

        }

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

        private void StartApplicationServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack<StartActionServerResponse, StartActionServerResponseWaiter>(
                _startActionServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.Result = serverResponse.Result;
                }
            );
        }

        private void RunClientRequest<ServerResponseType, ServerResponseWaiterType, ClientRequestType>(Dictionary<string, ServerResponseWaiterType> serverResponseWaiters, SocketDataType socketDataType, Action<ClientRequestType> consumer, Action<ServerResponseWaiterType> callBack)
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
            consumer(clientRequest);

            SocketData clientRequestData = new SocketData()
            {
                SocketDataType = socketDataType,
                Data = Common.Utils.GetBytesFromObject(clientRequest)
            };

            _socketHandler.SendToServer(Common.Utils.GetBytesFromObject(clientRequestData));

            while (serverResponseWaiter.Received == false)
                Thread.Sleep(1000);

            callBack(serverResponseWaiter);

            serverResponseWaiters.Remove(guid);
        }

        private void ServerResponseCallBack<ServerResponseType, ServerResponseWaiterType>(Dictionary<string, ServerResponseWaiterType> serverResponseWaiters, byte[] bytes, Action<ServerResponseWaiterType, ServerResponseType> consumer)
            where ServerResponseType : ServerResponseBase
            where ServerResponseWaiterType : ServerResponseWaiter
        {
            
            ServerResponseType serverResponse = Common.Utils.GetObjectFromBytes<ServerResponseType>(bytes);
            if (_startActionServerResponseWaiters.ContainsKey(serverResponse.RequestKey) == false) return;
            ServerResponseWaiterType serverResponseWaiter = serverResponseWaiters[serverResponse.RequestKey];
            serverResponseWaiter.Exception = ExceptionFactory.CreateException(serverResponse.FactorisableExceptionType, serverResponse.FactorisableExceptionParameters);
            serverResponseWaiter.Received = true;
            consumer(serverResponseWaiter, serverResponse);
        }

        private abstract class ServerResponseWaiter
        {
            public bool Received { get; set; }
            public FactorisableException Exception { get; set; }
        }

        private class StartActionServerResponseWaiter : ServerResponseWaiter
        {
            public ApplicationActionRunResult Result { get; set; }
        }
    }
}
