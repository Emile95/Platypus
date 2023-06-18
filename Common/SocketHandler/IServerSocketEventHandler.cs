using Common.SocketHandler.State;

namespace Common.SocketHandler
{
    public interface IServerSocketEventHandler<ClientKeyType> : ISocketEventHandler<ClientReceivedState<ClientKeyType>>
        where ClientKeyType : class
    {
        void OnAccept(ClientReceivedState<ClientKeyType> receivedState);
    }
}
