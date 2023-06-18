using Common.SocketHandler.State;

namespace Common.SocketHandler
{
    public interface IClientSocketEventHandler : ISocketEventHandler<ServerReceivedState>
    {
        void OnConnect(ServerReceivedState state);
    }
}
