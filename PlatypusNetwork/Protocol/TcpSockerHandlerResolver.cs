using System.Net.Sockets;

namespace PlatypusNetwork.SocketHandler.Protocol
{
    public class TcpSockerHandlerResolver<ReceivedStateType> : SocketHandlerResolver<ReceivedStateType>
        where ReceivedStateType : ReceivedState
    {
        private byte[] _currentRequestBuffer;
        private int _currentRequestBufferLength = 0;
        private int _currentRequestBufferCurrentOffset = 0;
        
        public TcpSockerHandlerResolver(
            int receivedBufferSize,
            Action<ReceivedStateType> onLostSocket,
            Action<ReceivedStateType> onReceive
        ) : base(receivedBufferSize, onLostSocket, onReceive) { }
        
        public override void Send(Socket socket, byte[] bytes)
        {
            socket.Send(CreateSendData(bytes));
        }

        public override void SendMultiple(Socket socket, byte[][] bytesList)
        {
            List<byte> bytesToReturn = new List<byte>();
            foreach (byte[] bytes in bytesList)
            {
                byte[] newBytes = CreateSendData(bytes);

                foreach (byte b in newBytes)
                    bytesToReturn.Add(b);
            }

            socket.Send(bytesToReturn.ToArray());
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

            ResolveReceivedBuffer(state, nBbytesRead, 0);

            ReceivedState nextState = state.CreateCopy();
            socket.BeginReceive(nextState.Buffer, 0, _receivedBufferSize, SocketFlags.None, ReadCallBack, nextState);
        }

        private void ResolveReceivedBuffer(ReceivedStateType state, int nBbytesReceived, int currentReceivedBufferOffset)
        {
            if (_currentRequestBufferCurrentOffset == 0)
            {
                SetCurrentRequestBuffer(state.Buffer, currentReceivedBufferOffset);
                currentReceivedBufferOffset += sizeOfInt;
            }
                
            while (currentReceivedBufferOffset < nBbytesReceived && _currentRequestBufferCurrentOffset < _currentRequestBufferLength)
                _currentRequestBuffer[_currentRequestBufferCurrentOffset++] = state.Buffer[currentReceivedBufferOffset++];

            if (_currentRequestBufferCurrentOffset == _currentRequestBufferLength)
                OnCurrentRequestDataRetreived(state);

            if (currentReceivedBufferOffset < nBbytesReceived)
                ResolveReceivedBuffer(state, nBbytesReceived, currentReceivedBufferOffset);
        }

        private void OnCurrentRequestDataRetreived(ReceivedStateType state)
        {
            state.BytesRead = _currentRequestBuffer;
            _currentRequestBufferCurrentOffset = 0;
            _onReceive(state);
        }

        private void SetCurrentRequestBuffer(byte[] buffer, int getLengthOffset)
        {
            _currentRequestBufferLength = GetIntFromBuffer(buffer, getLengthOffset);
            _currentRequestBuffer = new byte[_currentRequestBufferLength];
        }

        private int GetIntFromBuffer(byte[] buffer, int startOffset)
        {
            byte[] requestLengthData = new byte[sizeOfInt];
            for (int i = 0; i < sizeOfInt; i++)
                requestLengthData[i] = buffer[startOffset+i];
            return BitConverter.ToInt32(requestLengthData, 0);
        }
    }
}
