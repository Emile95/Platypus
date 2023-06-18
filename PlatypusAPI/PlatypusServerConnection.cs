using Common.SocketHandler;

namespace PlatypusAPI
{
    public class PlatypusServerConnection
    {
        private ClientSocketHandler _socketHandler;

        public PlatypusServerApplication Connect(string protocol = "tcp", string host = null, int port = 2000)
        {
            _socketHandler = new PlatypusServerSocketHandler(protocol);
            _socketHandler.Initialize(port);
            return new PlatypusServerApplication(_socketHandler);
        }

        public void Disconnect()
        {

        }
    }
}
