using System.Net.Sockets;

namespace Common.SocketHandler.Protocol
{
    public class TcpSockerHandlerResolver<ReceivedStateType> : SocketHandlerResolver<ReceivedStateType>
        where ReceivedStateType : ReceivedState
    {
        public TcpSockerHandlerResolver(
            int receivedBufferSize,
            Action<ReceivedStateType> onLostSocket,
            Action<ReceivedStateType> onReceive
        ) : base(receivedBufferSize, onLostSocket, onReceive) { }
        
        public override void Send(Socket socket, byte[] bytes)
        {
            byte[] sendLengthBytes = BitConverter.GetBytes(bytes.Length);
            int newSendLength = bytes.Length + sendLengthBytes.Length;
            byte[] newBytes = new byte[newSendLength];

            for (int i = 0; i < sendLengthBytes.Length; i++)
                newBytes[i] = sendLengthBytes[i];

            for (int i = sendLengthBytes.Length; i < newSendLength; i++)
                newBytes[i] = bytes[i - sendLengthBytes.Length];

            socket.Send(newBytes);
        }

        public override Socket CreateSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public override void ReadCallBack(IAsyncResult ar)
        {
            ReceivedStateType state = (ReceivedStateType)ar.AsyncState;
            Socket socket = state.WorkSocket;

            int nBbytesRead = 0;
            try
            {
                nBbytesRead = socket.EndReceive(ar);

            }
            catch (SocketException e)
            {
                _onLostSocket(state);
                return;
            }

            //Handle multiple call in the same buffer
            int currentBufferIndex = 0;
            while (currentBufferIndex < nBbytesRead)
            {
                byte[] sendLengthData = new byte[sizeOfInt];
                for (int i = 0; i < sendLengthData.Length; i++)
                    sendLengthData[i] = state.Buffer[currentBufferIndex + i];
                int sendLength = BitConverter.ToInt32(sendLengthData, 0);

                currentBufferIndex += sendLengthData.Length;

                state.BytesRead = new byte[sendLength];

                for (int i = 0; i < sendLength; i++)
                    state.BytesRead[i] = state.Buffer[i + currentBufferIndex];

                currentBufferIndex += sendLength;

                _onReceive(state);
            }

            ReceivedState nextState = state.CreateCopy();
            socket.BeginReceive(nextState.Buffer, 0, _receivedBufferSize, 0, ReadCallBack, nextState);
        }
    }
}
