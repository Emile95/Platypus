﻿using PlatypusNetwork.Request;
using PlatypusNetwork.Request.Data;
using PlatypusNetwork.Request.Definition;
using PlatypusNetwork.SocketHandler.State;
using PlatypusUtils;
using System.Net;
using System.Net.Sockets;

namespace PlatypusNetwork.SocketHandler
{
    public class ClientSocketHandler<ExceptionEnumType, RequestType> : BaseSocketHandler<ExceptionEnumType, RequestType, ServerReceivedState>
        where ExceptionEnumType : Enum
        where RequestType : Enum
    {
        protected Dictionary<RequestType, ClientRequestSenderDefinitionBase<ExceptionEnumType, RequestType>> _requestDefinitions;

        public ClientSocketHandler(ProtocolType protocol, RequestsProfile<ExceptionEnumType, RequestType> profile)
            : base(protocol) 
        {
            _requestDefinitions = new Dictionary<RequestType, ClientRequestSenderDefinitionBase<ExceptionEnumType, RequestType>>();

            if(profile != null)
                foreach (KeyValuePair<RequestType, RequestDefinitionBase<ExceptionEnumType, RequestType>> requestDefinition in profile.RequestDefinitions)
                    _requestDefinitions.Add(requestDefinition.Key, requestDefinition.Value as ClientRequestSenderDefinitionBase<ExceptionEnumType, RequestType>);
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
            _socket.Connect(GetEndPoint(hostIpAdress, port));

            ServerReceivedState state = new ServerReceivedState();
            state.BufferSize = _protocolRequestHandler.SizeOfRequestHeader;
            state.Buffer = new byte[_protocolRequestHandler.SizeOfRequestHeader];
            state.WorkSocket = _socket;

            _socket.BeginReceive(state.Buffer, 0, _protocolRequestHandler.SizeOfRequestHeader, SocketFlags.None, new AsyncCallback(_protocolRequestHandler.ReadCallBack), state);
            OnConnect(state);
        }

        public void SendToServer(byte[] data)
        {
            Send(_socket, data);
        }

        public void SendToServer(byte[][] datas)
        {
            SendMultiple(_socket, datas);
        }

        public override void OnReceive(ServerReceivedState receivedState)
        {
            RequestData<RequestType> serverResponse = Utils.GetObjectFromBytes<RequestData<RequestType>>(receivedState.BytesRead);
            _requestDefinitions[serverResponse.RequestType].ServerResponseCallBack(serverResponse.Data);
        }

        public virtual void OnConnect(ServerReceivedState state) { }

        public ServerResponseType HandleClientRequest<ClientRequestType, ServerResponseType>(RequestType requestType, Action<ClientRequestType> consumer = null)
            where ServerResponseType : ServerResponseBase<ExceptionEnumType>
            where ClientRequestType : ClientRequestBase, new()
        {
            var requestDefinition = _requestDefinitions[requestType] as ClientRequestSenderDefinition<ExceptionEnumType, RequestType, ClientRequestType, ServerResponseType>;

            return requestDefinition.HandleClientRequest(
                (clientRequestData) => Send(_socket, Utils.GetBytesFromObject(clientRequestData)), 
                consumer
            );
        }

        public override void OnLostSocket(ServerReceivedState receivedState) { }
    }
}
