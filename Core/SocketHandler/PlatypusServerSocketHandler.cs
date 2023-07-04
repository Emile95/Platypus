using Common.Exceptions;
using Common.SocketHandler;
using Common.SocketHandler.State;
using Common.Sockets;
using Common.Sockets.ClientRequest;
using Common.Sockets.ServerResponse;
using PlatypusAPI.Sockets.ClientRequest;
using PlatypusAPI.Sockets.ServerResponse;
using PlatypusAPI.User;
using Utils.GuidGeneratorHelper;

namespace Core.Sockethandler
{
    internal class PlatypusServerSocketHandler : ServerSocketHandler<string>
    {
        private readonly ServerInstance _serverInstance;
        private readonly int _port;

        public PlatypusServerSocketHandler(
            ServerInstance serverInstance,
            string protocol,
            int port
        ) : base(protocol)
        {
            _serverInstance = serverInstance;
            _port = port;
        }

        protected override string GenerateClientKey(List<string> currentKeys)
        {
            return GuidGenerator.GenerateFromEnumerable(currentKeys);
        }

        public override void OnAccept(ClientReceivedState<string> receivedState)
        {
            Console.WriteLine($"new client connected, key='{receivedState.ClientKey}'");
        }

        public override void OnLostSocket(ClientReceivedState<string> receivedState)
        {
            Console.WriteLine($"client lost, key='{receivedState.ClientKey}'");
        }

        public override void OnReceive(ClientReceivedState<string> receivedState)
        {
            SocketData clientRequest = Common.Utils.GetObjectFromBytes<SocketData>(receivedState.BytesRead);

            if (clientRequest == null) return;

            switch(clientRequest.SocketDataType)
            {
                case SocketDataType.UserConnection: ReceiveUserConnectionClientRequest(receivedState.ClientKey, clientRequest); break;
                case SocketDataType.StartApplicationAction: ReceiveStartApplicationActionClientRequest(receivedState.ClientKey, clientRequest); break;
                case SocketDataType.AddUser: ReceiveAddUserClientRequest(receivedState.ClientKey, clientRequest); break;
                case SocketDataType.GetRunningActions: ReceiveGetRunningApplicationActionsClientRequest(receivedState.ClientKey, clientRequest); break;
                case SocketDataType.CancelRunningAction: ReceiveCancelRunningApplicationActionRunClientRequest(receivedState.ClientKey, clientRequest); break;
            }
        }

        public void Initialize(string host)
        {
            Initialize(_port, host);
        }

        private void ReceiveUserConnectionClientRequest(string clientKey, SocketData clientRequestData)
        {
            HandleClientRequest<UserConnectionData, UserConnectionServerResponse>(
                clientKey, clientRequestData, SocketDataType.UserConnection,
                (clientRequest, serverResponse) =>
                {
                    serverResponse.UserAccount = _serverInstance.UserConnect(clientRequest.Credential, clientRequest.ConnectionMethodGuid);
                }
            );
        }

        private void ReceiveStartApplicationActionClientRequest(string clientKey, SocketData clientRequestData)
        {
            HandleClientRequest<StartActionClientRequest, StartActionServerResponse>(
                clientKey, clientRequestData, SocketDataType.StartApplicationAction,
                (clientRequest, serverResponse) =>
                {
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    serverResponse.Result = _serverInstance.RunAction(clientRequest.Parameters);
                }
            );
        }

        private void ReceiveAddUserClientRequest(string clientKey, SocketData clientRequestData)
        {
            HandleClientRequest<AddUserClientRequest, AddUserServerResponse>(
                clientKey, clientRequestData, SocketDataType.AddUser,
                (clientRequest, serverResponse) =>
                {
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    serverResponse.UserAccount = _serverInstance.AddUser(
                        clientRequest.CredentialMethodGUID,
                        clientRequest.FullName,
                        clientRequest.Email,
                        clientRequest.Data
                    );   
                }
            );
        }

        private void ReceiveGetRunningApplicationActionsClientRequest(string clientKey, SocketData clientRequestData)
        {
            HandleClientRequest<ClientRequestBase, GetRunningApplicationActionsServerResponse>(
                clientKey, clientRequestData, SocketDataType.GetRunningActions,
                (clientRequest, serverResponse) =>
                {
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    serverResponse.ApplicationActionRunInfos = _serverInstance.GetRunningApplicationActions().ToList();
                }
            );
        }

        private void ReceiveCancelRunningApplicationActionRunClientRequest(string clientKey, SocketData clientRequestData)
        {
            HandleClientRequest<CancelRunningApplicationRunClientRequest, AddUserServerResponse>(
                clientKey, clientRequestData, SocketDataType.CancelRunningAction,
                (clientRequest, serverResponse) =>
                {
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    _serverInstance.CancelRunningApplicationAction(clientRequest.ApplicationRunGuid);
                }
            );
        }

        private void HandleClientRequest<RequestType, ResponseType>(string clientKey, SocketData clientRequestData, SocketDataType serverResponseType, Action<RequestType, ResponseType> action)
            where ResponseType : ServerResponseBase, new()
            where RequestType : class, new()
        {
            SocketData serverResponseData = new SocketData()
            {
                SocketDataType = serverResponseType
            };
            ResponseType serverResponse = new ResponseType();
            RequestType clientRequest = Common.Utils.GetObjectFromBytes<RequestType>(clientRequestData.Data);
            try
            {
                action(clientRequest, serverResponse);
            }
            catch (FactorisableException e)
            {
                serverResponse.FactorisableExceptionType = e.FactorisableExceptionType;
                serverResponse.FactorisableExceptionParameters = e.GetParameters();
            }
            serverResponseData.Data = Common.Utils.GetBytesFromObject(serverResponse);
            SendToClient(clientKey, Common.Utils.GetBytesFromObject(serverResponseData));
        }
    }
}
