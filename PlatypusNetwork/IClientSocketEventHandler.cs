using PlatypusNetwork.SocketHandler.State;

namespace PlatypusNetwork.SocketHandler
{
    public interface IClientSocketEventHandler
    {
        void OnConnect(ServerReceivedState state);
    }
}
