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
        protected readonly ProtocolRequestHandler<ReceivedStateType> _protocolRequestHandler;
        
        public BaseSocketHandler(ProtocolType protocol)
        {
            switch (protocol)
            {
                case ProtocolType.Tcp:
                    _protocolRequestHandler = new TcpProtocolRequestHandler<ReceivedStateType>(OnLostSocket, OnReceive);
                    break;
            }
            _socket = _protocolRequestHandler.CreateSocket();
        }

        public void Send(Socket socket, byte[] bytes)
        {
            _protocolRequestHandler.Send(socket, bytes);
        }

        public void SendMultiple(Socket socket, byte[][] bytesList)
        {
            _protocolRequestHandler.SendMultiple(socket, bytesList);
        }

        protected IPEndPoint GetEndPoint(IPAddress ipAddress, int port)
        {
            return new IPEndPoint(ipAddress, port);
        }

        public abstract void OnReceive(ReceivedStateType receivedState);

        public abstract void OnLostSocket(ReceivedStateType receivedState);
    }
}