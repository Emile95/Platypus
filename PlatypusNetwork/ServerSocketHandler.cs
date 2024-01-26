using PlatypusNetwork.SocketHandler.State;
using System.Net;
using System.Net.Sockets;

namespace PlatypusNetwork.SocketHandler
{
    public abstract class ServerSocketHandler<ClientSocketKeyType> : BaseSocketHandler<ClientReceivedState<ClientSocketKeyType>>, ISocketHandlerInitiator, IServerSocketEventHandler<ClientSocketKeyType>
        where ClientSocketKeyType : class
    {
        protected readonly Dictionary<ClientSocketKeyType, Socket> _clientSockets;

        public ServerSocketHandler(string protocol)
            : base(protocol)
        {
            _clientSockets = new Dictionary<ClientSocketKeyType, Socket>();
        }

        public void Initialize(int port, string host = null)
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
            _socket.Bind(GetEndPoint(hostIpAdress, port));
            _socket.Listen(100);
            _socket.AcceptAsync();
            _socket.BeginAccept(new AsyncCallback(AcceptCallBack), _socket);
        }

        public void SendToClient(ClientSocketKeyType clientSocketKey, byte[] bytes)
        {
            Send(_clientSockets[clientSocketKey], bytes);
        }

        public void SendToAllClients(byte[] bytes)
        {
            foreach (Socket clientSocket in _clientSockets.Values)
                Send(clientSocket, bytes);
        }

        public abstract void OnAccept(ClientReceivedState<ClientSocketKeyType> receivedState);

        protected abstract ClientSocketKeyType GenerateClientKey(List<ClientSocketKeyType> allKeys);

        private void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = (Socket)ar.AsyncState;
            Socket clientSocket = serverSocket.EndAccept(ar);
            ClientSocketKeyType clientKey = GenerateClientKey(_clientSockets.Keys.ToList());
            _clientSockets.Add(clientKey, clientSocket);

            ClientReceivedState<ClientSocketKeyType> state = new ClientReceivedState<ClientSocketKeyType>();
            state.BufferSize = _receivedBufferSize;
            state.Buffer = new byte[_receivedBufferSize];
            state.WorkSocket = clientSocket;
            state.ClientKey = clientKey;

            OnAccept(state);

            clientSocket.BeginReceive(state.Buffer, 0, _receivedBufferSize, 0, new AsyncCallback(_socketResolver.ReadCallBack), state);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), _socket);
        }
    }
}
