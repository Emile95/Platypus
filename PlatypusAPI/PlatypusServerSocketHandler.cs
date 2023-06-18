using Common.SocketHandler;
using Common.SocketHandler.State;

namespace PlatypusAPI
{
    public class PlatypusServerSocketHandler : ClientSocketHandler
    {
        public PlatypusServerSocketHandler(string protocol) 
            : base(protocol) { }

        public override void OnConnect(ServerReceivedState state)
        {
            
        }

        public override void OnLostSocket(ServerReceivedState receivedState)
        {
            
        }

        public override void OnReceive(ServerReceivedState receivedState)
        {
            
        }
    }
}
