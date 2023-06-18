using Common.SocketHandler.State;
using System.Net;
using System.Net.Sockets;

namespace Common.SocketHandler.Tcp
{
    public abstract class TCPClientSocketHandler : TCPSocketHandler<ServerReceivedState>, IClientSocketEventHandler
    {
        public sealed override void Initialize(int port, string host = null)
        {
            IPAddress hostIpAdress = null;
            if (host == null)
            {
                foreach (IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                        hostIpAdress = address;
                }
            }
            else hostIpAdress = IPAddress.Parse(host);
            _socket.Connect(GetEndPoint(hostIpAdress, port));
            ServerReceivedState state = new ServerReceivedState();
            state.BufferSize = _receivedBufferSize;
            state.Buffer = new byte[_receivedBufferSize];
            state.WorkSocket = _socket;
            _socket.BeginReceive(state.Buffer, 0, _receivedBufferSize, 0, new AsyncCallback(ReadCallBack), state);
            OnConnect(state);
        }

        public void SendToServer(byte[] bytes)
        {
            Send(_socket, bytes);
        }

        public abstract void OnConnect(ServerReceivedState state);
    }
}
