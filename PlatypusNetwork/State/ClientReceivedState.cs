namespace PlatypusNetwork.SocketHandler.State
{
    public class ClientReceivedState : ReceivedState
    {
        public string ClientKey { get; set; }

        public override ReceivedState CreateCopy()
        {
            ClientReceivedState clientReceivedState = new ClientReceivedState();
            clientReceivedState.BufferSize = BufferSize;
            clientReceivedState.Buffer = new byte[BufferSize];
            clientReceivedState.WorkSocket = WorkSocket;
            clientReceivedState.ClientKey = ClientKey;
            return clientReceivedState;
        }
    }
}
