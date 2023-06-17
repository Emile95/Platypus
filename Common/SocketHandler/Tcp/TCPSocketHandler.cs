using System.Net.Sockets;

namespace Common.SocketHandler.Tcp
{
    public abstract class TCPSocketHandler<ReceivedStateType> : BaseSocketHandler<ReceivedStateType>
        where ReceivedStateType : ReceivedState, new()
    {
        private readonly int sizeOfInt = sizeof(int);

        protected TCPSocketHandler() 
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _receivedBufferSize = 1000000;
        }

        protected override void ReadCallBack(IAsyncResult ar)
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
                OnLostSocket(state);
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

                OnReceive(state);
            }

            ReceivedState nextState = state.CreateCopy();
            socket.BeginReceive(nextState.Buffer, 0, _receivedBufferSize, 0, ReadCallBack, nextState);
        }

        public override void Send(Socket socket, byte[] bytes)
        {
            //Send with the send length at the beginning
            byte[] sendLengthBytes = BitConverter.GetBytes(bytes.Length);
            int newSendLength = bytes.Length + sendLengthBytes.Length;
            byte[] newBytes = new byte[newSendLength];

            for (int i = 0; i < sendLengthBytes.Length; i++)
                newBytes[i] = sendLengthBytes[i];

            for (int i = sendLengthBytes.Length; i < newSendLength; i++)
                newBytes[i] = bytes[i - sendLengthBytes.Length];

            socket.Send(newBytes);
        }
    }
}
