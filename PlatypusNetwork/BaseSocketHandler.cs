using PlatypusNetwork.Request;
using PlatypusNetwork.SocketHandler.Protocol;
using System.Net;
using System.Net.Sockets;

namespace PlatypusNetwork.SocketHandler
{
    public abstract class BaseSocketHandler<ExceptionEnumType, RequestType, ReceivedStateType> : ISocketEventHandler<ReceivedStateType>
        where ExceptionEnumType : Enum
        where ReceivedStateType : ReceivedState, new()
        where RequestType : Enum
    {
        protected Socket _socket;
        protected int _receivedBufferSize;
        protected readonly SocketHandlerResolver<ReceivedStateType> _socketResolver;
        protected Dictionary<RequestType, RequestDefinitionBase<ExceptionEnumType>> requestDefinitions;

        public BaseSocketHandler(ProtocolType protocol, RequestsProfile<ExceptionEnumType, RequestType> profile = null)
        {
            _receivedBufferSize = 1000;
            switch (protocol)
            {
                case ProtocolType.Tcp:
                    _socketResolver = new TcpSockerHandlerResolver<ReceivedStateType>(_receivedBufferSize, OnLostSocket, OnReceive);
                    break;
            }
            _socket = _socketResolver.CreateSocket();

            requestDefinitions = profile?.Requests;

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