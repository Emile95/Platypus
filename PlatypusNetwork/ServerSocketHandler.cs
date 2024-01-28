using PlatypusNetwork.Request;
using PlatypusNetwork.Request.Data;
using PlatypusNetwork.Request.Definition;
using PlatypusNetwork.SocketHandler.State;
using PlatypusUtils;
using System.Net;
using System.Net.Sockets;

namespace PlatypusNetwork.SocketHandler
{
    public abstract class ServerSocketHandler<ExceptionEnumType, RequestTypeEnum> : BaseSocketHandler<ExceptionEnumType, RequestTypeEnum, ClientReceivedState>
        where ExceptionEnumType : Enum
        where RequestTypeEnum : Enum
    {
        protected readonly Dictionary<string, Socket> _clientSockets;

        protected Dictionary<RequestTypeEnum, ClientRequestReceiverDefinitionBase<ExceptionEnumType, RequestTypeEnum>> _requestDefinitions;
        protected RequestsProfile<ExceptionEnumType, RequestTypeEnum> _requestsProfile;
        public ServerSocketHandler(ProtocolType protocol, int receivedBufferSize, RequestsProfile<ExceptionEnumType, RequestTypeEnum> profile)
            : base(protocol, receivedBufferSize)
        {
            _clientSockets = new Dictionary<string, Socket>();

            _requestDefinitions = new Dictionary<RequestTypeEnum, ClientRequestReceiverDefinitionBase<ExceptionEnumType, RequestTypeEnum>>();

            _requestsProfile = profile;

            if (profile != null)
                foreach (KeyValuePair<RequestTypeEnum, RequestDefinitionBase<ExceptionEnumType, RequestTypeEnum>> requestDefinition in profile.RequestDefinitions)
                    _requestDefinitions.Add(requestDefinition.Key, requestDefinition.Value as ClientRequestReceiverDefinitionBase<ExceptionEnumType, RequestTypeEnum>);
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

        public void SendToClient(string clientSocketKey, byte[] bytes)
        {
            Send(_clientSockets[clientSocketKey], bytes);
        }

        public void SendToAllClients(byte[] bytes)
        {
            foreach (Socket clientSocket in _clientSockets.Values)
                Send(clientSocket, bytes);
        }

        public abstract void OnAccept(ClientReceivedState receivedState);

        protected abstract string GenerateClientKey(List<string> allKeys);

        public override void OnReceive(ClientReceivedState receivedState)
        {
            RequestData<RequestTypeEnum> clientRequest = Utils.GetObjectFromBytes<RequestData<RequestTypeEnum>>(receivedState.BytesRead);

            if (clientRequest == null) return;

            _requestDefinitions[clientRequest.RequestType].HandleClientRequest(receivedState.ClientKey, clientRequest, (serverResponse) => {
                SendToClient(receivedState.ClientKey, Utils.GetBytesFromObject(serverResponse));
            });
        }

        private void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = (Socket)ar.AsyncState;
            Socket clientSocket = serverSocket.EndAccept(ar);
            string clientKey = GenerateClientKey(_clientSockets.Keys.ToList());
            _clientSockets.Add(clientKey, clientSocket);

            ClientReceivedState state = new ClientReceivedState();
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
