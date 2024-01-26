namespace PlatypusNetwork.SocketHandler.State
{
    public class ServerReceivedState : ReceivedState
    {
        public override ReceivedState CreateCopy()
        {
            ServerReceivedState serverReceivedState = new ServerReceivedState();
            serverReceivedState.BufferSize = BufferSize;
            serverReceivedState.Buffer = new byte[BufferSize];
            serverReceivedState.WorkSocket = WorkSocket;
            return serverReceivedState;
        }
    }
}
