﻿using PlatypusNetwork.Request;
using PlatypusNetwork.SocketHandler.State;
using PlatypusUtils;
using System.Net;
using System.Net.Sockets;

namespace PlatypusNetwork.SocketHandler
{
    public class ClientSocketHandler<ExceptionEnumType, RequestType> : BaseSocketHandler<ExceptionEnumType, RequestType, ServerReceivedState>, IClientSocketEventHandler, ISocketHandlerInitiator
        where ExceptionEnumType : Enum
        where RequestType : Enum
    {
        

        public ClientSocketHandler(ProtocolType protocol, RequestsProfile<ExceptionEnumType, RequestType> profile = null)
            : base(protocol, profile) {}

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
            _socket.Connect(GetEndPoint(hostIpAdress, port));
            ServerReceivedState state = new ServerReceivedState();
            state.BufferSize = _receivedBufferSize;
            state.Buffer = new byte[_receivedBufferSize];
            state.WorkSocket = _socket;
            _socket.BeginReceive(state.Buffer, 0, _receivedBufferSize, 0, new AsyncCallback(_socketResolver.ReadCallBack), state);
            OnConnect(state);
        }

        public override void OnReceive(ServerReceivedState receivedState)
        {
            RequestData<RequestType> serverResponse = Utils.GetObjectFromBytes<RequestData<RequestType>>(receivedState.BytesRead);
            requestDefinitions[serverResponse.RequestType].ServerResponseCallBack(serverResponse.Data);
        }

        public virtual void OnConnect(ServerReceivedState state) { }

        public ServerResponseType HandleClientRequest<ClientRequestType, ServerResponseType>(RequestType requestType, Action<ClientRequestType> consumer = null)
            where ServerResponseType : ServerResponseBase<ExceptionEnumType>
            where ClientRequestType : ClientRequestBase, new()
        {
            var requestDefinition = requestDefinitions[requestType] as RequestDefinition<ExceptionEnumType, RequestType, ClientRequestType, ServerResponseType>;

            return requestDefinition.HandleClientRequest(
                (clientRequestData) => Send(_socket, Utils.GetBytesFromObject(clientRequestData)), 
                consumer
            );
        }

        public override void OnLostSocket(ServerReceivedState receivedState) { }
    }
}