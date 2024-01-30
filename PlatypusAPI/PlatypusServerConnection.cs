using PlatypusAPI.ServerFunctionParameter;
using PlatypusAPI.Network;
using PlatypusAPI.Network.ServerResponse;
using System.Net.Sockets;
using PlatypusAPI.Network.ClientRequest;
using PlatypusNetwork.SocketHandler;
using PlatypusAPI.Exceptions;

namespace PlatypusAPI
{
    public class PlatypusServerConnection
    {
        private UserConnectionParameter _userConnectionData;


        public void SetUserConnectionData(string connectionMethodGuid, Dictionary<string, object> credential)
        {
            _userConnectionData = new UserConnectionParameter()
            {
                ConnectionMethodGuid = connectionMethodGuid,
                Credential = credential
            };
        }

        public PlatypusServerApplication Connect(ProtocolType protocol = ProtocolType.Tcp, string host = null, int port = 2000)
        {
            ClientSocketHandler<FactorisableExceptionType, RequestType> socketHandler = new ClientSocketHandler<FactorisableExceptionType, RequestType>(protocol, new PlatypusRequestsProfile());
            socketHandler.Initialize(port, host);

            UserConnectionServerResponse serverResponse = socketHandler.HandleClientRequest<UserConnectionClientRequest, UserConnectionServerResponse>(
                RequestType.UserConnection,
                (clientRequest) => {
                    clientRequest.ConnectionMethodGuid = _userConnectionData.ConnectionMethodGuid;
                    clientRequest.Credential = _userConnectionData.Credential;
                }
            );

            return new PlatypusServerApplication(socketHandler, serverResponse.UserAccount);
        }
    }
}
