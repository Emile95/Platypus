﻿using System.Net.Sockets;

namespace PlatypusNetwork.SocketHandler.Protocol
{
    public abstract class ProtocolRequestHandler<ReceivedStateType>
        where ReceivedStateType : ReceivedState
    {
        public int SizeOfRequestHeader { get; protected set; }
        protected readonly Action<ReceivedStateType> _onLostSocket;
        protected readonly Action<ReceivedStateType> _onReceive;

        public ProtocolRequestHandler(
            int sizeOfRequestHeader,
            Action<ReceivedStateType> onLostSocket,
            Action<ReceivedStateType> onReceive
        )
        {
            SizeOfRequestHeader = sizeOfRequestHeader;
            _onLostSocket = onLostSocket;
            _onReceive = onReceive;
        }

        protected byte[] CreateSendData(byte[] bytes)
        {
            byte[] sendLengthBytes = BitConverter.GetBytes(bytes.Length);
            int newSendLength = bytes.Length + sendLengthBytes.Length;
            byte[] newBytes = new byte[newSendLength];

            for (int i = 0; i < sendLengthBytes.Length; i++)
                newBytes[i] = sendLengthBytes[i];

            for (int i = sendLengthBytes.Length; i < newSendLength; i++)
                newBytes[i] = bytes[i - sendLengthBytes.Length];

            return newBytes;
        }

        public abstract Socket CreateSocket();
        public abstract void Send(Socket socket, byte[] bytes);
        public abstract void SendMultiple(Socket socket, byte[][] bytesList);
        public abstract void ReadCallBack(IAsyncResult ar);
        
    }
}
