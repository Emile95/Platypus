namespace Common.SocketHandler
{
    public interface IClientSocketHandler : IClientSocketEventHandler
    {
        void SendToServer(byte[] bytes);
    }
}
