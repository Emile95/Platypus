using Common.Exceptions;
using Common.SocketData.ClientRequest;
using Common.SocketData.ServerResponse;
using PlatypusAPI.Exceptions;
using PlatypusAPI.SocketData.ServerResponse;
using PlatypusAPI.User;

namespace PlatypusAPI
{
    public class PlatypusServerConnection
    {
        private UserConnectionData _userConnectionData;

        private UserAccount _connectedUser;

        private bool _isConnecting;
        private PlatypusException _exception;

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
            socketHandler.Initialize(port);


            socketHandler.ServerResponseCallBacks[ServerResponseType.UserConnection].Add(ReceiveUserConnectionServerResponse);

            ClientRequestData requestData = new ClientRequestData()
            {
                ClientRequestype = ClientRequestype.UserConnection,
                Data = Common.Utils.GetBytesFromObject(_userConnectionData)
            };

            _isConnecting = true;
            socketHandler.SendToServer(Common.Utils.GetBytesFromObject(requestData));

            while (_isConnecting)
                Thread.Sleep(1000);

            if (_exception != null)
                throw _exception;

            socketHandler.ServerResponseCallBacks[ServerResponseType.UserConnection].Clear();

            return new PlatypusServerApplication(
                socketHandler,
                _connectedUser
            );
        }

        private void ReceiveUserConnectionServerResponse(byte[] bytes)
        {
            UserConnectionServerResponse response = Common.Utils.GetObjectFromBytes<UserConnectionServerResponse>(bytes);
            _connectedUser = response.UserAccount;
            _exception = ExceptionFactory.CreateException(response.ExceptionType, response.ExcetionStringParameter);
            _isConnecting = false;
        }
    }
}
