namespace PlatypusNetwork.SocketHandler
{
    public interface ISocketHandlerInitiator
    {
        void Initialize(int port, string host = null);
    }
}
