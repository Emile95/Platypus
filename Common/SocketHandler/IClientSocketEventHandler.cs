using Common.SocketHandler.State;

namespace Common.SocketHandler
{
    public interface IClientSocketEventHandler
    {
        void OnConnect(ServerReceivedState state);
    }
}
