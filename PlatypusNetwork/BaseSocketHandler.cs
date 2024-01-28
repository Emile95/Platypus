﻿using PlatypusNetwork.SocketHandler.Protocol;
using System.Net;
using System.Net.Sockets;

namespace PlatypusNetwork.SocketHandler
{
    public abstract class BaseSocketHandler<ExceptionEnumType, RequestType, ReceivedStateType>
        where ExceptionEnumType : Enum
        where ReceivedStateType : ReceivedState, new()
        where RequestType : Enum
    {
        protected Socket _socket;
        protected int _receivedBufferSize;
        protected readonly SocketHandlerResolver<ReceivedStateType> _socketResolver;
        
        public BaseSocketHandler(ProtocolType protocol, int receivedBufferSize)
        {
            _receivedBufferSize = receivedBufferSize;
            switch (protocol)
            {
                case ProtocolType.Tcp:
                    _socketResolver = new TcpSockerHandlerResolver<ReceivedStateType>(_receivedBufferSize, OnLostSocket, OnReceive);
                    break;
            }
            _socket = _socketResolver.CreateSocket();
        }

        public void Send(Socket socket, byte[] bytes)
        {
            _socketResolver.Send(socket, bytes);
        }

        protected IPEndPoint GetEndPoint(IPAddress ipAddress, int port)
        {
            return new IPEndPoint(ipAddress, port);
        }

        public abstract void OnReceive(ReceivedStateType receivedState);

        public abstract void OnLostSocket(ReceivedStateType receivedState);
    }
}