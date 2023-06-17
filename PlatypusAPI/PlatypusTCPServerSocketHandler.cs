using Common.SocketHandler.State;
using Common.SocketHandler.Tcp;

namespace PlatypusAPI
{
    internal class PlatypusTCPServerSocketHandler : TCPClientSocketHandler
    {
        public PlatypusTCPServerSocketHandler()
        {

        }

        protected override void OnLostSocket(ServerReceivedState receivedState)
        {
            
        }

        protected override void OnReceive(ServerReceivedState receivedState)
        {
            
        }
    }
}
