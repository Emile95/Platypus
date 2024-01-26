namespace PlatypusNetwork.SocketHandler
{
    public interface ISocketEventHandler<ReceivedStateType>
        where ReceivedStateType : ReceivedState, new()
    {
        void OnReceive(ReceivedStateType receivedState);
        void OnLostSocket(ReceivedStateType receivedState);
    }
}
