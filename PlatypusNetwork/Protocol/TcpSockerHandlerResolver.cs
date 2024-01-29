﻿using System.Net.Sockets;

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

            ResolveReceivedBuffer(state, nBbytesRead, 0);

            ReceivedState nextState = state.CreateCopy();
            socket.BeginReceive(nextState.Buffer, 0, _receivedBufferSize, 0, ReadCallBack, nextState);
        }

        private void ResolveReceivedBuffer(ReceivedStateType state, int nBbytesReceived, int currentReceivedBufferOffset)
        {
            if (_currentRequestBufferCurrentOffset == 0)
                currentReceivedBufferOffset += SetCurrentRequestBuffer(state.Buffer, currentReceivedBufferOffset);

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

        private int SetCurrentRequestBuffer(byte[] buffer, int getLengthOffset)
        {
            _currentRequestBufferLength = GetIntFromBuffer(buffer, getLengthOffset);
            _currentRequestBuffer = new byte[_currentRequestBufferLength];
            return sizeOfInt;
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
