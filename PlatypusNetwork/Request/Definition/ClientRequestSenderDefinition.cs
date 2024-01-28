using PlatypusNetwork.Exceptions;
using PlatypusNetwork.Request.Data;
using PlatypusUtils;

namespace PlatypusNetwork.Request.Definition
{
    public class ClientRequestSenderDefinition<ExceptionEnumType, RequestTypeEnum, ClientRequestType, ServerResponseType> : ClientRequestSenderDefinitionBase<ExceptionEnumType, RequestTypeEnum>
        where ExceptionEnumType : Enum
        where RequestTypeEnum : Enum
        where ClientRequestType : ClientRequestBase, new()
        where ServerResponseType : ServerResponseBase<ExceptionEnumType>
    {
        private Dictionary<string, ServerResponseWaiter<ServerResponseType>> _serverResponseWaiters;

        public ClientRequestSenderDefinition(
            RequestTypeEnum requestType,
            ExceptionFactory<ExceptionEnumType> exceptionFactory
        )
            : base(requestType, exceptionFactory)
        {
            _serverResponseWaiters = new Dictionary<string, ServerResponseWaiter<ServerResponseType>>();
        }

        public ServerResponseType HandleClientRequest(Action<RequestData<RequestTypeEnum>> requestAction, Action<ClientRequestType> consumer = null)
        {
            string guid = Utils.GenerateGuidFromEnumerable(_serverResponseWaiters.Keys);

            ServerResponseWaiter<ServerResponseType> serverResponseWaiter = new ServerResponseWaiter<ServerResponseType>();

            _serverResponseWaiters.Add(guid, serverResponseWaiter);

            ClientRequestType clientRequest = new ClientRequestType()
            {
                RequestKey = guid
            };

            if (consumer != null)
                consumer(clientRequest);

            RequestData<RequestTypeEnum> clientRequestData = new RequestData<RequestTypeEnum>()
            {
                RequestType = _requestType,
                Data = Utils.GetBytesFromObject(clientRequest)
            };

            requestAction(clientRequestData);

            while (serverResponseWaiter.Received == false) Thread.Sleep(200);

            if (serverResponseWaiter.Response.ExceptionThrowed)
                throw _exceptionFactory.CreateException(serverResponseWaiter.Response.FactorisableExceptionType, serverResponseWaiter.Response.FactorisableExceptionParameters);

            _serverResponseWaiters.Remove(guid);

            return serverResponseWaiter.Response;
        }

        public override void ServerResponseCallBack(byte[] bytes)
        {
            ServerResponseType serverResponse = Utils.GetObjectFromBytes<ServerResponseType>(bytes);

            if (_serverResponseWaiters.ContainsKey(serverResponse.RequestKey) == false) return;

            ServerResponseWaiter<ServerResponseType> serverResponseWaiter = _serverResponseWaiters[serverResponse.RequestKey];

            serverResponseWaiter.Received = true;
            serverResponseWaiter.Response = serverResponse;
        }

        private class ServerResponseWaiter<ResponseType>
            where ResponseType : ServerResponseBase<ExceptionEnumType>
        {
            public bool Received { get; set; }
            public ResponseType Response { get; set; }
        }
    }
}
