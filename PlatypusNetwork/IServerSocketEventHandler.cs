using PlatypusNetwork.SocketHandler.State;

namespace PlatypusNetwork.SocketHandler
{
    public interface IServerSocketEventHandler
    {
        void OnAccept(ClientReceivedState receivedState);
    }
}
