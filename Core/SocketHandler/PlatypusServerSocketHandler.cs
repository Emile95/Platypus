using Common.Exceptions;
using Common.SocketData.ClientRequest;
using Common.SocketData.ServerResponse;
using Common.SocketHandler;
using Common.SocketHandler.State;
using PlatypusAPI.SocketData.ClientRequest;
using PlatypusAPI.SocketData.ServerResponse;
using PlatypusAPI.User;
using Utils.GuidGeneratorHelper;
using Utils.Json;

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
            ClientRequestData clientRequest = Common.Utils.GetObjectFromBytes<ClientRequestData>(receivedState.BytesRead);

            if (clientRequest == null) return;

            switch(clientRequest.ClientRequestType)
            {
                case ClientRequestType.UserConnection: ReceiveUserConnectionClientRequest(receivedState.ClientKey, clientRequest); break;
                case ClientRequestType.StartApplicationAction: ReceiveStartApplicationActionClientRequest(receivedState.ClientKey, clientRequest); break;
            }
        }

        public void Initialize(string host)
        {
            Initialize(_port, host);
        }

        private void ReceiveUserConnectionClientRequest(string clientKey, ClientRequestData clientRequestData)
        {
            HandleClientRequest<UserConnectionData, UserConnectionServerResponse>(
                clientKey, clientRequestData, ServerResponseType.UserConnection,
                (clientRequest, serverResponse) =>
                {
                    serverResponse.UserAccount = _serverInstance.UserConnect(clientRequest.Credential, clientRequest.ConnectionMethodGuid);
                }
            );
        }

        private void ReceiveStartApplicationActionClientRequest(string clientKey, ClientRequestData clientRequestData)
        {
            HandleClientRequest<StartActionClientRequest, StartActionServerResponse>(
                clientKey, clientRequestData, ServerResponseType.ApplicationActionRunResult,
                (clientRequest, serverResponse) =>
                {
                    serverResponse.StartActionKey = clientRequest.StartActionKey;
                    serverResponse.Result = _serverInstance.RunAction(clientRequest.Parameters);
                }
            );
        }

        private void HandleClientRequest<RequestType, ResponseType>(string clientKey, ClientRequestData clientRequestData, ServerResponseType serverResponseType, Action<RequestType, ResponseType> action)
            where ResponseType : ServerResponseBase, new()
            where RequestType : class, new()
        {
            ServerResponseData serverResponseData = new ServerResponseData()
            {
                ServerResponseType = serverResponseType
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
