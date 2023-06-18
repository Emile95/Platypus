using Common.SocketHandler.State;
using System.Net;
using System.Net.Sockets;

namespace Common.SocketHandler.Tcp
{
    public abstract class TCPServerSocketHandler<ClientSocketKeyType> : TCPSocketHandler<ClientReceivedState<ClientSocketKeyType>>, IServerSocketHandler<ClientSocketKeyType>
        where ClientSocketKeyType : class
    {
        public Dictionary<ClientSocketKeyType, Socket> ClientSockets { get; set; }

        public TCPServerSocketHandler()
        {
            ClientSockets = new Dictionary<ClientSocketKeyType, Socket>();
        }

        public sealed override void Initialize(int port, string host = null)
        {
            IPAddress hostIpAdress = null;
            if (host == null)
            {
                foreach(IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if(address.AddressFamily == AddressFamily.InterNetwork)
                        hostIpAdress = address;
                }
            }
            else hostIpAdress = IPAddress.Parse(host);
            _socket.Bind(GetEndPoint(hostIpAdress, port));
            _socket.Listen(100);
            _socket.AcceptAsync();
            _socket.BeginAccept(new AsyncCallback(AcceptCallBack), _socket);
        }
        private void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = (Socket)ar.AsyncState;
            Socket clientSocket = serverSocket.EndAccept(ar);
            ClientSocketKeyType clientKey = GenerateClientKey(ClientSockets.Keys.ToList());
            ClientSockets.Add(clientKey, clientSocket);

            ClientReceivedState<ClientSocketKeyType> state = new ClientReceivedState<ClientSocketKeyType>();
            state.BufferSize = _receivedBufferSize;
            state.Buffer = new byte[_receivedBufferSize];
            state.WorkSocket = clientSocket;
            state.ClientKey = clientKey;

            OnAccept(state);

            clientSocket.BeginReceive(state.Buffer, 0, _receivedBufferSize, 0, new AsyncCallback(ReadCallBack), state);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), _socket);
        }

        public void SendToClient(ClientSocketKeyType clientSocketKey, byte[] bytes)
        {
            Send(ClientSockets[clientSocketKey], bytes);
        }

        public void SendToAllClients(byte[] bytes)
        {
            foreach(Socket clientSocket in ClientSockets.Values)
                Send(clientSocket, bytes);
        }

        protected abstract ClientSocketKeyType GenerateClientKey(List<ClientSocketKeyType> currentKeys);

        public abstract void OnAccept(ClientReceivedState<ClientSocketKeyType> receivedState);
    }
}
