using Common.Exceptions;
using Common.Sockets;
using PlatypusAPI.Exceptions;
using PlatypusAPI.Sockets.ServerResponse;
using PlatypusAPI.User;

namespace PlatypusAPI
{
    public class PlatypusServerConnection
    {
        private UserConnectionData _userConnectionData;

        private UserAccount _connectedUser;

        private bool _isConnecting;
        private FactorisableException _exception;

        public void SetUserConnectionData(string connectionMethodGuid, Dictionary<string, object> credential)
        {
            _userConnectionData = new UserConnectionData()
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
                Data = Common.Utils.GetBytesFromObject(_userConnectionData)
            };

            _isConnecting = true;
            socketHandler.SendToServer(Common.Utils.GetBytesFromObject(requestData));

            while (_isConnecting);

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
            UserConnectionServerResponse response = Common.Utils.GetObjectFromBytes<UserConnectionServerResponse>(bytes);
            _connectedUser = response.UserAccount;
            _exception = ExceptionFactory.CreateException(response.FactorisableExceptionType, response.FactorisableExceptionParameters);
            _isConnecting = false;
        }
    }
}
