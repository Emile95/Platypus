using System.Net.Sockets;

namespace PlatypusNetwork.SocketHandler.Protocol
{
    public class TcpSockerHandlerResolver<ReceivedStateType> : SocketHandlerResolver<ReceivedStateType>
        where ReceivedStateType : ReceivedState
    {
        private bool _isBeginingOfRequest = true;
        private int _requestBufferIndex = 0;
        private byte[] _requestBuffer;
        private int _requestSize = 0;

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

            int currentBufferLengthToRead = 0;
            int currentBufferIndex = 0;
            int lengthOfCurrentRead = nBbytesRead;
            bool requestWillBeCompleted = false;
            bool otherRequestAfterEnd = false;

            if (_isBeginingOfRequest)
            {
                _requestSize = GetLengthOfTheRequest(state);
                _requestBuffer = new byte[_requestSize];
                lengthOfCurrentRead -= sizeOfInt;
                currentBufferIndex = sizeOfInt;
                if (_requestSize == lengthOfCurrentRead)
                {
                    currentBufferLengthToRead = _requestSize;
                    requestWillBeCompleted = true;
                } 
                else
                {
                    _isBeginingOfRequest = false;
                    if(_requestSize < lengthOfCurrentRead)
                    {
                        otherRequestAfterEnd = true;
                        currentBufferLengthToRead = lengthOfCurrentRead - (lengthOfCurrentRead - _requestSize);
                    }
                    else currentBufferLengthToRead = lengthOfCurrentRead;
                }

                currentBufferLengthToRead += sizeOfInt;
            }
            else
            {
                int currentLengthOfRequestBuffer = _requestBufferIndex + 1;
                requestWillBeCompleted = (currentLengthOfRequestBuffer + lengthOfCurrentRead) >= _requestSize;

                int nbBytesToReadToCompleteRequest = _requestSize - currentLengthOfRequestBuffer;

                currentBufferLengthToRead = lengthOfCurrentRead - (lengthOfCurrentRead - nbBytesToReadToCompleteRequest);

                otherRequestAfterEnd = nbBytesToReadToCompleteRequest < lengthOfCurrentRead;

                currentBufferLengthToRead = nbBytesToReadToCompleteRequest < lengthOfCurrentRead ?
                    lengthOfCurrentRead - (lengthOfCurrentRead - nbBytesToReadToCompleteRequest) :
                    lengthOfCurrentRead;

               if (requestWillBeCompleted)
                    _requestBuffer[_requestSize - 1] = state.Buffer[currentBufferLengthToRead];
            }

            while(currentBufferIndex < currentBufferLengthToRead)
                _requestBuffer[_requestBufferIndex++] = state.Buffer[currentBufferIndex++];
            
            if(requestWillBeCompleted)
            {
                state.BytesRead = _requestBuffer;
                _requestBufferIndex = 0;
                _onReceive(state);
            }

            ReceivedState nextState = state.CreateCopy();
            socket.BeginReceive(nextState.Buffer, 0, _receivedBufferSize, 0, ReadCallBack, nextState);
        }

        private int GetLengthOfTheRequest(ReceivedStateType state)
        {
            byte[] requestLengthData = new byte[sizeOfInt];
            for (int i = 0; i < requestLengthData.Length; i++)
                requestLengthData[i] = state.Buffer[i];
            return BitConverter.ToInt32(requestLengthData, 0);
        }
    }
}
