namespace Common.SocketHandler.State
{
    public class ClientReceivedState<ClientKeyType> : ReceivedState
    {
        public ClientKeyType ClientKey { get; set; }

        public override ReceivedState CreateCopy()
        {
            ClientReceivedState<ClientKeyType> clientReceivedState = new ClientReceivedState<ClientKeyType>();
            clientReceivedState.BufferSize = BufferSize;
            clientReceivedState.Buffer = new byte[BufferSize];
            clientReceivedState.WorkSocket = WorkSocket;
            clientReceivedState.ClientKey = ClientKey;
            return clientReceivedState;
        }
    }
}
