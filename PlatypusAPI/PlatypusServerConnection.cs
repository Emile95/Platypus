using Common.SocketHandler;
using Common.SocketHandler.State;

namespace PlatypusAPI
{
    public class PlatypusServerConnection
    {
        private BaseSocketHandler<ServerReceivedState> _socketHandler;

        public PlatypusServerApplication Connect(string protocol = "tcp", string host = null, int port = 2000)
        {
            switch(protocol)
            {
                case "tcp":
                    _socketHandler = new PlatypusTCPServerSocketHandler();
                    break;
            }
            if (_socketHandler == null) return null;

            _socketHandler.Initialize(port);

            return new PlatypusServerApplication(_socketHandler);
        }

        public void Disconnect()
        {

        }
    }
}
