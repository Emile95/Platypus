using System.Net;
using System.Net.Sockets;

namespace Common.SocketHandler
{
    public abstract class BaseSocketHandler<ReceivedStateType>
        where ReceivedStateType : ReceivedState, new()
    {
        protected Socket _socket;
        protected int _receivedBufferSize;

        protected IPEndPoint GetEndPoint(IPAddress ipAddress, int port)
        {
            return new IPEndPoint(ipAddress, port);
        }

        public abstract void Initialize(int port, string host = null);
        public abstract void Send(Socket socket, byte[] bytes);
        protected abstract void ReadCallBack(IAsyncResult ar);
    }
}