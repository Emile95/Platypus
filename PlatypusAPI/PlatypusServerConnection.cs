using PlatypusAPI.Exceptions;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusAPI.User;
using PlatypusUtils;
using PlatypusAPI.Network;
using PlatypusAPI.Network.ServerResponse;

namespace PlatypusAPI
{
    public class PlatypusServerConnection
    {
        private UserConnectionParameter _userConnectionData;

        private UserAccount _connectedUser;

        private bool _isConnecting;
        private FactorisableException _exception;

        public void SetUserConnectionData(string connectionMethodGuid, Dictionary<string, object> credential)
        {
            _userConnectionData = new UserConnectionParameter()
            {
                ConnectionMethodGuid = connectionMethodGuid,
                Credential = credential
            };
        }

        public PlatypusServerApplication Connect(string protocol = "tcp", string host = null, int port = 2000)
        {
            PlatypusClientSocketHandler socketHandler = new PlatypusClientSocketHandler(protocol);
            socketHandler.Initialize(port, host);

            socketHandler.ServerResponseCallBacks[SocketDataType.UserConnection].Add(ReceiveUserConnectionServerResponse);

            SocketData requestData = new SocketData()
            {
                SocketDataType = SocketDataType.UserConnection,
                Data = Utils.GetBytesFromObject(_userConnectionData)
            };

            _isConnecting = true;
            socketHandler.SendToServer(Utils.GetBytesFromObject(requestData));

            while (_isConnecting) Thread.Sleep(200);

            if (_exception != null)
                throw _exception;

            socketHandler.ServerResponseCallBacks[SocketDataType.UserConnection].Clear();

            return new PlatypusServerApplication(
                socketHandler,
                _connectedUser
            );
        }

        private void ReceiveUserConnectionServerResponse(byte[] bytes)
        {
            UserConnectionServerResponse response = Utils.GetObjectFromBytes<UserConnectionServerResponse>(bytes);
            _connectedUser = response.UserAccount;
            _exception = ExceptionFactory.CreateException(response.FactorisableExceptionType, response.FactorisableExceptionParameters);
            _isConnecting = false;
        }
    }
}
