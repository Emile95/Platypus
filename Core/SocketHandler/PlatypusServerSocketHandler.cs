using Common.Exceptions;
using Common.SocketData.ClientRequest;
using Common.SocketData.ServerResponse;
using Common.SocketHandler;
using Common.SocketHandler.State;
using Core.ApplicationAction;
using Core.User;
using PlatypusAPI.SocketData.ServerResponse;
using PlatypusAPI.User;
using Utils.GuidGeneratorHelper;

namespace Core.Sockethandler
{
    internal class PlatypusServerSocketHandler : ServerSocketHandler<string>
    {
        private readonly ApplicationActionsHandler _applicationActionsHandler;
        private readonly UsersHandler _usersHandler;
        private readonly int _port;

        public PlatypusServerSocketHandler(
            string protocol,
            int port,
            ApplicationActionsHandler applicationActionsHandler,
            UsersHandler usersHandler
        ) : base(protocol)
        {
            _applicationActionsHandler = applicationActionsHandler;
            _usersHandler = usersHandler;
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

            switch(clientRequest.ClientRequestype)
            {
                case ClientRequestype.UserConnection: ReceiveUserConnectionClientRequest(receivedState.ClientKey, clientRequest); break;
            }
        }

        public void Initialize(string host)
        {
            Initialize(_port, host);
        }

        private void ReceiveUserConnectionClientRequest(string clientKey, ClientRequestData clientRequest)
        {
            ServerResponseData serverResponseData = new ServerResponseData()
            {
                ServerResponseType = ServerResponseType.UserConnection
            };
            UserConnectionServerResponse serverResponse = new UserConnectionServerResponse();
            UserConnectionData userConnectionData = Common.Utils.GetObjectFromBytes<UserConnectionData>(clientRequest.Data);
            try
            {
                serverResponse.UserAccount = _usersHandler.Connect(userConnectionData.Credential, userConnectionData.ConnectionMethodGuid);
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
