using PlatypusNetwork.SocketHandler.State;

namespace PlatypusNetwork.SocketHandler
{
    public interface IServerSocketEventHandler<ClientKeyType>
        where ClientKeyType : class
    {
        void OnAccept(ClientReceivedState<ClientKeyType> receivedState);
    }
}
