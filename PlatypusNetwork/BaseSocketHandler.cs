using PlatypusNetwork.SocketHandler.Protocol;
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
        protected int _sizeOfRequestHeader = sizeof(int);
        protected readonly SocketHandlerResolver<ReceivedStateType> _socketResolver;
        
        public BaseSocketHandler(ProtocolType protocol)
        {
            switch (protocol)
            {
                case ProtocolType.Tcp:
                    _socketResolver = new TcpSockerHandlerResolver<ReceivedStateType>(_sizeOfRequestHeader, OnLostSocket, OnReceive);
                    break;
            }
            _socket = _socketResolver.CreateSocket();
        }

        public void Send(Socket socket, byte[] bytes)
        {
            _socketResolver.Send(socket, bytes);
        }

        public void SendMultiple(Socket socket, byte[][] bytesList)
        {
            _socketResolver.SendMultiple(socket, bytesList);
        }

        protected IPEndPoint GetEndPoint(IPAddress ipAddress, int port)
        {
            return new IPEndPoint(ipAddress, port);
        }

        public abstract void OnReceive(ReceivedStateType receivedState);

        public abstract void OnLostSocket(ReceivedStateType receivedState);
    }
}